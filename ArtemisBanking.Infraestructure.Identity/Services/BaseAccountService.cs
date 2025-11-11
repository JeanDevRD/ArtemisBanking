using ArtemisBanking.Core.Application.Dtos.Email;
using ArtemisBanking.Core.Application.Dtos.User;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Infraestructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Text;
namespace ArtemisBanking.Infraestructure.Identity.Services
{
    public class BaseAccountService : IBaseAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly ISavingsAccountService _savingsAccountService;

        protected BaseAccountService(
            UserManager<User> userManager,
            IEmailService emailService,
            ISavingsAccountService savingsAccountService)
        {
            _userManager = userManager;
            _emailService = emailService;
            _savingsAccountService = savingsAccountService;
        }

        public virtual async Task<RegisterResponseDto> RegisterUser(SaveUserDto saveDto, string? origin, bool? isApi = false)
        {
            RegisterResponseDto response = new()
            {
                Email = "",
                Id = "",
                LastName = "",
                FirstName = "",
                UserName = "",
                HasError = false
            };

            var userWithSameUserName = await _userManager.FindByNameAsync(saveDto.UserName);
            if (userWithSameUserName != null)
            {
                response.HasError = true;
                response.ErrorMessage = $"El nombre de usuario: {saveDto.UserName} ya está en uso.";
                return response;
            }

            var userWithSameEmail = await _userManager.FindByEmailAsync(saveDto.Email);
            if (userWithSameEmail != null)
            {
                response.HasError = true;
                response.ErrorMessage = $"El correo: {saveDto.Email} ya está en uso.";
                return response;
            }

            User user = new()
            {
                FirstName = saveDto.FirstName,
                LastName = saveDto.LastName,
                IdentificationNumber = saveDto.IdentificationNumber,
                Email = saveDto.Email,
                UserName = saveDto.UserName,
                EmailConfirmed = false,
                PhoneNumber = saveDto.Phone,
                IsActive = false // Se crea inactivo
            };

            var result = await _userManager.CreateAsync(user, saveDto.Password);

            if (!result.Succeeded)
            {
                response.HasError = true;
                response.ErrorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                return response;
            }

            await _userManager.AddToRoleAsync(user, saveDto.Role);

            // Si es cliente o comercio, crear cuenta de ahorro principal
            if (saveDto.Role == "Client" || saveDto.Role == "Merchant")
            {
                var accountNumber = await GenerateUniqueAccountNumber();
                var savingsAccount = new ArtemisBanking.Core.Application.Dtos.SavingsAccount.SavingsAccountDto
                {
                    Id = 0,
                    AccountNumber = accountNumber,
                    Balance = saveDto.InitialAmount,
                    Type = (int)ArtemisBanking.Core.Domain.Common.Enum.TypeSavingAccount.Main,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UserId = user.Id,
                    AssignedByUserId = null
                };

                await _savingsAccountService.AddAsync(savingsAccount);
            }

            // Enviar correo de confirmación
            if (isApi != null && !isApi.Value)
            {
                string verificationUri = await GetVerificationEmailUri(user, origin ?? "");
                await _emailService.SendAsync(new EmailRequestDto
                {
                    To = saveDto.Email,
                    HtmlBody = $"<p>Por favor confirma tu cuenta visitando esta URL: <a href='{verificationUri}'>Confirmar cuenta</a></p>",
                    Subject = "Confirmar registro - Artemis Banking"
                });
            }
            else
            {
                string? verificationToken = await GetVerificationEmailToken(user);
                await _emailService.SendAsync(new EmailRequestDto
                {
                    To = saveDto.Email,
                    HtmlBody = $"<p>Por favor confirma tu cuenta usando este token: <strong>{verificationToken}</strong></p>",
                    Subject = "Confirmar registro - Artemis Banking"
                });
            }

            var rolesList = await _userManager.GetRolesAsync(user);

            response.Id = user.Id;
            response.Email = user.Email ?? "";
            response.UserName = user.UserName ?? "";
            response.FirstName = user.FirstName;
            response.LastName = user.LastName;
            response.IsVerified = user.EmailConfirmed;
            response.Roles = rolesList.ToList();

            return response;
        }

        public virtual async Task<EditResponseDto> EditUser(SaveUserDto saveDto, string? origin, bool? isCreated = false, bool? isApi = false)
        {
            bool isNotcreated = !isCreated ?? false;
            EditResponseDto response = new()
            {
                Email = "",
                Id = "",
                LastName = "",
                FirstName = "",
                UserName = "",
                HasError = false,
                Errors = []
            };

            var userWithSameUserName = await _userManager.Users.FirstOrDefaultAsync(w => w.UserName == saveDto.UserName && w.Id != saveDto.Id);
            if (userWithSameUserName != null)
            {
                response.HasError = true;
                response.Errors.Add($"El nombre de usuario: {saveDto.UserName} ya está en uso.");
                return response;
            }

            var userWithSameEmail = await _userManager.Users.FirstOrDefaultAsync(w => w.Email == saveDto.Email && w.Id != saveDto.Id);
            if (userWithSameEmail != null)
            {
                response.HasError = true;
                response.Errors.Add($"El correo: {saveDto.Email} ya está en uso.");
                return response;
            }

            var user = await _userManager.FindByIdAsync(saveDto.Id!);
            if (user == null)
            {
                response.HasError = true;
                response.Errors.Add($"No existe una cuenta registrada con este usuario");
                return response;
            }

            user.FirstName = saveDto.FirstName;
            user.LastName = saveDto.LastName;
            user.Email = saveDto.Email;
            user.UserName = saveDto.UserName;
            user.PhoneNumber = saveDto.Phone;
            user.IdentificationNumber = saveDto.IdentificationNumber;
            user.EmailConfirmed = user.EmailConfirmed && user.Email == saveDto.Email;

            if (!string.IsNullOrWhiteSpace(saveDto.Password) && isNotcreated)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resultChange = await _userManager.ResetPasswordAsync(user, token, saveDto.Password);

                if (resultChange != null && !resultChange.Succeeded)
                {
                    response.HasError = true;
                    response.Errors.AddRange(resultChange.Errors.Select(s => s.Description).ToList());
                    return response;
                }
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                var rolesList = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, rolesList.ToList());
                await _userManager.AddToRoleAsync(user, saveDto.Role);

                // Si es cliente y hay monto adicional, agregarlo a cuenta principal
                if (saveDto.Role == "Client" && saveDto.InitialAmount > 0)
                {
                    var accounts = await _savingsAccountService.GetAllAsync();
                    var mainAccount = accounts.FirstOrDefault(a => a.UserId == user.Id && a.Type == (int)ArtemisBanking.Core.Domain.Common.Enum.TypeSavingAccount.Main);
                    if (mainAccount != null)
                    {
                        mainAccount.Balance += saveDto.InitialAmount;
                        await _savingsAccountService.UpdateAsync(mainAccount.Id, mainAccount);
                    }
                }

                if (!user.EmailConfirmed && isNotcreated)
                {
                    if (isApi != null && !isApi.Value)
                    {
                        string verificationUri = await GetVerificationEmailUri(user, origin ?? "");
                        await _emailService.SendAsync(new EmailRequestDto()
                        {
                            To = saveDto.Email,
                            HtmlBody = $"<p>Por favor confirma tu cuenta visitando esta URL: <a href='{verificationUri}'>Confirmar cuenta</a></p>",
                            Subject = "Confirmar registro - Artemis Banking"
                        });
                    }
                    else
                    {
                        string? verificationToken = await GetVerificationEmailToken(user);
                        await _emailService.SendAsync(new EmailRequestDto()
                        {
                            To = saveDto.Email,
                            HtmlBody = $"<p>Por favor confirma tu cuenta usando este token: <strong>{verificationToken}</strong></p>",
                            Subject = "Confirmar registro - Artemis Banking"
                        });
                    }
                }

                var updatedRolesList = await _userManager.GetRolesAsync(user);
                response.Id = user.Id;
                response.Email = user.Email ?? "";
                response.UserName = user.UserName ?? "";
                response.FirstName = user.FirstName;
                response.LastName = user.LastName;
                response.IsVerified = user.EmailConfirmed;
                response.Roles = updatedRolesList.ToList();

                return response;
            }
            else
            {
                response.HasError = true;
                response.Errors.AddRange(result.Errors.Select(s => s.Description).ToList());
                return response;
            }
        }

        public virtual async Task<UserResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto request, bool? isApi = false)
        {
            UserResponseDto response = new() { HasError = false, Errors = [] };

            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                response.HasError = true;
                response.Errors.Add($"No existe una cuenta registrada con el usuario {request.UserName}");
                return response;
            }

            user.EmailConfirmed = false;
            user.IsActive = false;
            await _userManager.UpdateAsync(user);

            if (isApi != null && !isApi.Value)
            {
                var resetUri = await GetResetPasswordUri(user, request.Origin ?? "");
                await _emailService.SendAsync(new EmailRequestDto()
                {
                    To = user.Email,
                    HtmlBody = $"<p>Por favor resetea tu contraseña visitando esta URL: <a href='{resetUri}'>Resetear contraseña</a></p>",
                    Subject = "Resetear contraseña - Artemis Banking"
                });
            }
            else
            {
                string? resetToken = await GetResetPasswordToken(user);
                await _emailService.SendAsync(new EmailRequestDto()
                {
                    To = user.Email,
                    HtmlBody = $"<p>Por favor resetea tu contraseña usando este token: <strong>{resetToken}</strong></p>",
                    Subject = "Resetear contraseña - Artemis Banking"
                });
            }

            return response;
        }

        public virtual async Task<UserResponseDto> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            UserResponseDto response = new() { HasError = false, Errors = [] };

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                response.HasError = true;
                response.Errors.Add($"No existe una cuenta registrada con este usuario");
                return response;
            }

            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
            var result = await _userManager.ResetPasswordAsync(user, token, request.Password);

            if (!result.Succeeded)
            {
                response.HasError = true;
                response.Errors.AddRange(result.Errors.Select(s => s.Description).ToList());
                return response;
            }

            user.EmailConfirmed = true;
            user.IsActive = true;
            await _userManager.UpdateAsync(user);

            return response;
        }

        public virtual async Task<UserResponseDto> DeleteAsync(string id)
        {
            UserResponseDto response = new() { HasError = false, Errors = [] };
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                response.HasError = true;
                response.Errors.Add($"No existe una cuenta registrada con este usuario");
                return response;
            }

            await _userManager.DeleteAsync(user);
            return response;
        }

        public virtual async Task<UserDto?> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return null;

            var rolesList = await _userManager.GetRolesAsync(user);

            return new UserDto()
            {
                Id = user.Id,
                Email = user.Email ?? "",
                LastName = user.LastName,
                FirstName = user.FirstName,
                UserName = user.UserName ?? "",
                IdentificationNumber = user.IdentificationNumber,
                Phone = user.PhoneNumber,
                IsVerified = user.EmailConfirmed,
                IsActive = user.IsActive,
                Role = rolesList.FirstOrDefault() ?? ""
            };
        }

        public virtual async Task<UserDto?> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return null;

            var rolesList = await _userManager.GetRolesAsync(user);

            return new UserDto()
            {
                Id = user.Id,
                Email = user.Email ?? "",
                LastName = user.LastName,
                FirstName = user.FirstName,
                UserName = user.UserName ?? "",
                IdentificationNumber = user.IdentificationNumber,
                Phone = user.PhoneNumber,
                IsVerified = user.EmailConfirmed,
                IsActive = user.IsActive,
                Role = rolesList.FirstOrDefault() ?? ""
            };
        }

        public virtual async Task<UserDto?> GetUserByUserName(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) return null;

            var rolesList = await _userManager.GetRolesAsync(user);

            return new UserDto()
            {
                Id = user.Id,
                Email = user.Email ?? "",
                LastName = user.LastName,
                FirstName = user.FirstName,
                UserName = user.UserName ?? "",
                IdentificationNumber = user.IdentificationNumber,
                Phone = user.PhoneNumber,
                IsVerified = user.EmailConfirmed,
                IsActive = user.IsActive,
                Role = rolesList.FirstOrDefault() ?? ""
            };
        }

        public virtual async Task<List<UserDto>> GetAllUser(bool? isActive = true)
        {
            List<UserDto> listUsersDtos = [];

            var users = _userManager.Users;

            if (isActive != null && isActive == true)
            {
                users = users.Where(w => w.EmailConfirmed && w.IsActive);
            }

            var listUser = await users.ToListAsync();

            foreach (var item in listUser)
            {
                var roleList = await _userManager.GetRolesAsync(item);

                listUsersDtos.Add(new UserDto()
                {
                    Id = item.Id,
                    Email = item.Email ?? "",
                    LastName = item.LastName,
                    FirstName = item.FirstName,
                    UserName = item.UserName ?? "",
                    IdentificationNumber = item.IdentificationNumber,
                    Phone = item.PhoneNumber,
                    IsVerified = item.EmailConfirmed,
                    IsActive = item.IsActive,
                    Role = roleList.FirstOrDefault() ?? ""
                });
            }

            return listUsersDtos;
        }

        public virtual async Task<UserResponseDto> ConfirmAccountAsync(string userId, string token)
        {
            UserResponseDto response = new() { HasError = false, Errors = [] };

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                response.Message = "No existe una cuenta registrada con este usuario";
                response.HasError = true;
                return response;
            }

            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                user.IsActive = true;
                await _userManager.UpdateAsync(user);

                response.Message = $"Cuenta confirmada para {user.Email}. Ya puedes usar la aplicación";
                response.HasError = false;
                return response;
            }
            else
            {
                response.Message = $"Ocurrió un error al confirmar el correo {user.Email}";
                response.HasError = true;
                return response;
            }
        }

        #region "Protected methods"
        protected async Task<string> GetVerificationEmailUri(User user, string origin)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var route = "Login/ConfirmEmail";
            var completeUrl = new Uri(string.Concat(origin, "/", route));
            var verificationUri = QueryHelpers.AddQueryString(completeUrl.ToString(), "userId", user.Id);
            verificationUri = QueryHelpers.AddQueryString(verificationUri.ToString(), "token", token);
            return verificationUri;
        }

        protected async Task<string?> GetVerificationEmailToken(User user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            return token;
        }

        protected async Task<string> GetResetPasswordUri(User user, string origin)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var route = "Login/ResetPassword";
            var completeUrl = new Uri(string.Concat(origin, "/", route));
            var resetUri = QueryHelpers.AddQueryString(completeUrl.ToString(), "userId", user.Id);
            resetUri = QueryHelpers.AddQueryString(resetUri.ToString(), "token", token);
            return resetUri;
        }

        protected async Task<string?> GetResetPasswordToken(User user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            return token;
        }

        private async Task<string> GenerateUniqueAccountNumber()
        {
            string accountNumber;
            do
            {
                accountNumber = new Random().Next(100000000, 999999999).ToString();
                var existingAccount = await _savingsAccountService.GetAllAsync();
                if (!existingAccount.Any(a => a.AccountNumber == accountNumber))
                    break;
            } while (true);

            return accountNumber;
        }
        #endregion
    }
}

