using ArtemisBanking.Core.Application.Dtos.Common;
using ArtemisBanking.Core.Application.Dtos.CreditCard;
using ArtemisBanking.Core.Application.Dtos.Email;
using ArtemisBanking.Core.Application.Dtos.Loan;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Infraestructure.Persistence.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;


namespace ArtemisBanking.Core.Application.Services
{
    public class CreditCardService : GenericService<CreditCardDto, CreditCard>, ICreditCardService
    {
        private readonly ICreditCardRepository _creditCardRepository;
        private readonly IEmailService _emailService;
        private readonly IAccountServiceForApp _userForApp;
        private readonly IMapper _mapper;
        public CreditCardService(ICreditCardRepository genericRepository, IMapper mapper, IAccountServiceForApp userForAp, ICardTransactionRepository cardTransactionRepository, 
            IEmailService emailService) : base(genericRepository, mapper)
        {
            _creditCardRepository = genericRepository;
            _mapper = mapper;
            _userForApp = userForAp;
            _emailService = emailService;
        }
        public override async Task<CreditCardDto> GetByIdAsync(int id)
        {
            try
            {
                var entities = await _creditCardRepository.GetAllListIncluideAsync(["CardTransactions"]);
               
                var entity = entities.FirstOrDefault(e => e.Id == id);
                if (entity == null)
                {
                    return null!;
                }
                return _mapper.Map<CreditCardDto>(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null!;
            }
        }

        public async Task<List<CreditCardDto>> GetAllWithInclude()
        {
            try
            {
                var creditCards = await _creditCardRepository.GetAllListIncluideAsync(["CardTransactions"]);
                if (creditCards == null)
                {
                    return new List<CreditCardDto>();
                }
                return _mapper.Map<List<CreditCardDto>>(creditCards);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving credit cards with included data: " + ex.Message);
            }
        }

        public async Task<ResultDto<List<CreditCardListDto>>> GetAllActiveCreditCard()
        {
            var result = new ResultDto<List<CreditCardListDto>>();

            try
            {
                var creditCards = await _creditCardRepository.GetAllQueryAsync()
                    .Where(x => x.IsActive).OrderByDescending(x => x.CreateAt).ToListAsync();

                var dtoList = _mapper.Map<List<CreditCardListDto>>(creditCards);

                for (int i = 0; i < creditCards.Count; i++)
                {
                    var card = creditCards[i];
                    var dto = dtoList[i];

                    var user = await _userForApp.GetUserById(card.UserId);
                    var fullName = $"{user!.FirstName} {user.LastName}";
                    if (user != null)
                    {
                        dto.FullNameUser = fullName;
                    }
                }

                result.IsError = false;
                result.Result = dtoList;
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<ResultDto<List<CreditCardListDto>>> GetAllCreditCardByIdentityUser(string identificationNumber, bool? isActive = null)
        {
            var result = new ResultDto<List<CreditCardListDto>>();

            try
            {
                var user = await _userForApp.GetUserByIdentificationNumber(identificationNumber);

                if (user == null)
                {
                    result.IsError = true;
                    result.Message = "Usuario no encontrado";
                    return result;
                }

                var query = _creditCardRepository.GetAllQueryAsync()
                    .Where(x => x.UserId == user.Id);

                if (isActive.HasValue)
                {
                    query = query.Where(x => x.IsActive == isActive.Value);
                }

                var creditCards = await query
                    .OrderByDescending(x => x.IsActive)   
                    .ThenByDescending(x => x.CreateAt)   
                    .ToListAsync();

                var dtoList = _mapper.Map<List<CreditCardListDto>>(creditCards);
                var fullName = $"{user.FirstName} {user.LastName}";

                foreach (var dto in dtoList)
                {
                    dto.FullNameUser = fullName;
                }

                result.IsError = false;
                result.Result = dtoList;
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<bool> CancelatedCreditCard(string creditCardId)
        {
            var cards = await _creditCardRepository.GetAllListAsync();

            var creditCard = cards.FirstOrDefault(c => c.CardNumber == creditCardId);

            if (creditCard == null)
                return false; 

            if (creditCard.CurrentDebt > 0)
                return false; 

            creditCard.IsActive = false;
            await _creditCardRepository.UpdateAsync(creditCard.Id, creditCard);

            return true; 
        }


        public async Task<ResultDto<List<ElegibleUserForCreditCardDto>>> GetElegibleUserForCreditCard()
        {
            var result = new ResultDto<List<ElegibleUserForCreditCardDto>>();
            try
            {
                var allUsers = await _userForApp.GetAllUser();

                if (allUsers == null || !allUsers.Any())
                {
                    result.IsError = true;
                    result.Message = "No hay usuarios registrados";
                    return result;
                }

                var activeUsers = allUsers.Where(u => u.IsActive).ToList();
                var elegibleUsers = new List<ElegibleUserForCreditCardDto>();

                foreach (var user in activeUsers)
                {
                    var hasActiveCard = await _creditCardRepository.GetAllQueryAsync().AnyAsync(c => c.UserId == user.Id && c.IsActive);

                    if (!hasActiveCard)
                    {
                        var totalDebt = await _creditCardRepository.GetAllQueryAsync()
                            .Where(c => c.UserId == user.Id).SumAsync(c => c.CurrentDebt);

                        var dto = _mapper.Map<ElegibleUserForCreditCardDto>(user);
                        dto.FullName = $"{user.FirstName} {user.LastName}";
                        dto.MonthlyIncome = totalDebt;

                        elegibleUsers.Add(dto);
                    }
                }

                result.IsError = false;
                result.Result = elegibleUsers;
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<ResultDto<ElegibleUserForCreditCardDto>> GetElegibleUserByIdentityForCreditCard(string identificationNumber)
        {
            var result = new ResultDto<ElegibleUserForCreditCardDto>();
            try
            {
                var user = await _userForApp.GetUserByIdentificationNumber(identificationNumber);

                if (user == null)
                {
                    result.IsError = true;
                    result.Message = "No se encontró el usuario";
                    return result;
                }

                var hasActiveCard = await _creditCardRepository
                    .GetAllQueryAsync()
                    .AnyAsync(c => c.UserId == user.Id && c.IsActive);

                if (hasActiveCard)
                {
                    result.IsError = true;
                    result.Message = "El cliente ya tiene una tarjeta activa";
                    return result;
                }

                var totalDebt = await _creditCardRepository
                    .GetAllQueryAsync()
                    .Where(c => c.UserId == user.Id)
                    .SumAsync(c => c.CurrentDebt);

                var dto = _mapper.Map<ElegibleUserForCreditCardDto>(user);
                dto.FullName = $"{user.FirstName} {user.LastName}";
                dto.MonthlyIncome = totalDebt;

                result.IsError = false;
                result.Result = dto;
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return result;
        }


        public async Task<ResultDto<CreditCardDto>> AddCreditCardAsync(CreditCardRequestDto request, string adminUserId)
        {
            var result = new ResultDto<CreditCardDto>();

            try
            {
                var cliente = await _userForApp.GetUserById(request.IdCliente);
                if (cliente == null || !cliente.IsActive)
                {
                    result.IsError = true;
                    result.Message = "Cliente no válido o inactivo.";
                    return result;
                }

                var hasActiveCard = await _creditCardRepository.GetAllQueryAsync()
                    .AnyAsync(c => c.UserId == request.IdCliente && c.IsActive);

                if (hasActiveCard)
                {
                    result.IsError = true;
                    result.Message = "El cliente ya tiene una tarjeta activa.";
                    return result;
                }

                string cardNumber;

                do
                {
                    cardNumber = GenerarNumeroTarjeta();
                } 
                while 
                (
                    await _creditCardRepository.GetAllQueryAsync().AnyAsync(c => c.CardNumber == cardNumber)
                );

                var expirationDate = DateTime.UtcNow.AddYears(3);
                var cvc = new Random().Next(100, 999).ToString();
                var cvcHash = HashSHA256(cvc);

                var creditCard = new CreditCard
                {
                    Id = 0,
                    UserId = request.IdCliente,
                    CreditLimit = request.CreditLimit,
                    CurrentDebt = 0,
                    ExpirationDate = expirationDate,
                    CardNumber = cardNumber,
                    CvcHash = cvcHash,
                    AssignedByUserId = adminUserId,
                    CreateAt = DateTime.UtcNow,
                    IsActive = true
                };

                await _creditCardRepository.AddAsync(creditCard);

                var dto = _mapper.Map<CreditCardDto>(creditCard);

                result.IsError = false;
                result.Result = dto;
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return result;
        }


        public async Task<ResultDto<CreditCardDto>> UpdateCard(UpdateCreditCardDto dto, string creditCardId)
        {
            var result = new ResultDto<CreditCardDto>();

            try 
            {

                var creditCards = await _creditCardRepository.GetAllListAsync();
                var creditCard = creditCards.FirstOrDefault(c => c.CardNumber == creditCardId);

                if (creditCard == null) 
                { 
                    result.IsError = true;
                    result.Message = "Tarjeta no encontrada.";
                    return result;
                }

                if (dto.NewCreditLimit < creditCard.CurrentDebt)
                {
                    result.IsError = true;
                    result.Message = "El nuevo límite no puede ser inferior a la deuda actual.";
                    return result;
                }

                creditCard.CreditLimit = dto.NewCreditLimit;
                var update = await _creditCardRepository.UpdateAsync(creditCard.Id, creditCard);
                var user = await _userForApp.GetUserById(creditCard.UserId);
                var lastDigits = update!.CardNumber.Substring(update.CardNumber.Length - 4);

                await _emailService.SendAsync(new EmailRequestDto
                {
                    To = user!.Email,
                    Subject = "Actualizacion de limite de credito aprobado - Artemis Banking",
                    HtmlBody = $@"
                                 <h3>Estimado {user.FirstName} {user.LastName},</h3>
                                 <p>El límite de su tarje xxx-xxx-{lastDigits} ha sido modificado.
                                    El nuevo límite aprobado es {update.CreditLimit:C}.
                                </p>
                                <p>Gracias por confiar en Artemis Banking.</p>"
                });


                result.IsError = false;
                result.Result = _mapper.Map<CreditCardDto>(update);

            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return result;

        }

        public async Task<ResultDto<CreditCardDetailDto>> DetailCard(string idCreditCard)
        {
            var result = new ResultDto<CreditCardDetailDto>();

            try 
            {

                var includes = new List<string> { nameof(CreditCard.CardTransactions) };

                var query = _creditCardRepository.GetQueryWithIncluide(includes);

                var card = await query.FirstOrDefaultAsync(c => c.CardNumber == idCreditCard);

                if (card == null)
                {
                    result.IsError = true;
                    result.Message = "Tarjeta no encontrada.";
                }

                var resultDto = _mapper.Map<CreditCardDetailDto>(card);

                resultDto.consumptions = card.CardTransactions?.OrderByDescending(t => t.Date)
                .Select(t => new ConsumptionDto
                 {
                 consumptionDate = t.Date,
                 amount = t.Amount,
                 merchant = string.IsNullOrEmpty(t.Merchant) ? "AVANCE" : t.Merchant,
                 status = t.Status
                }).ToList() ?? new List<ConsumptionDto>();

                result.Result = resultDto;
            }
            catch(Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return result;
        }



        #region Pirvate Methods
        private string GenerarNumeroTarjeta()
        {
            var rnd = new Random();
            return string.Concat(Enumerable.Range(0, 16).Select(_ => rnd.Next(0, 10).ToString()));
        }

        private string HashSHA256(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        #endregion

    }
}
