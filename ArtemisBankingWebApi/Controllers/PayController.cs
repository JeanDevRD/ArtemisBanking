using Asp.Versioning;
using ArtemisBanking.Core.Application.Dtos.Email;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Common.Enum;
using ArtemisBanking.Core.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ArtemisBanking.Core.Application.Dtos.Transaction;

namespace ArtemisBankingWebApi.Controllers.v1
{
    [Authorize(Roles = "Admin,Merchant")]
    [ApiVersion("1.0")]
    public class PayController : BaseApiController
    {
        private readonly IMerchantService _merchantService;
        private readonly ICreditCardService _creditCardService;
        private readonly ICardTransactionService _cardTransactionService;
        private readonly ISavingsAccountService _savingsAccountService;
        private readonly IAccountServiceForApi _accountService;
        private readonly IEmailService _emailService;

        public PayController(IMerchantService merchantService,ICreditCardService creditCardService,ICardTransactionService cardTransactionService,
            ISavingsAccountService savingsAccountService,IAccountServiceForApi accountService,IEmailService emailService)
        {
            _merchantService = merchantService;
            _creditCardService = creditCardService;
            _cardTransactionService = cardTransactionService;
            _savingsAccountService = savingsAccountService;
            _accountService = accountService;
            _emailService = emailService;
        }
     
        [HttpGet("get-transactions/{commerceId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTransactions(int commerceId,[FromQuery] int page = 1,[FromQuery] int pageSize = 20)
        {
            try
            {
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var userId = User.FindFirst("uid")?.Value;

                if (userRole == "Merchant")
                {
                    var merchantCommerce = await _merchantService.GetByIdAsync(commerceId);
                    if (merchantCommerce == null || merchantCommerce.UserId != userId)
                    {
                        return Forbid();
                    }
                }

                var commerce = await _merchantService.GetByIdAsync(commerceId);
                if (commerce == null)
                {
                    return NotFound(new { message = "Comercio no encontrado" });
                }

                var accounts = await _savingsAccountService.GetAllAsync();
                var commerceAccount = accounts.FirstOrDefault(a => a.AccountNumber == commerce.AssociatedAccount);

                if (commerceAccount == null)
                {
                    return NoContent();
                }

                var accountDetail = await _savingsAccountService.GetSavingsAccountDetail(commerceAccount.AccountNumber);

                if (accountDetail.IsError || accountDetail.Result?.Transactions == null || !accountDetail.Result.Transactions.Any())
                {
                    return NoContent();
                }

                var transactions = accountDetail.Result.Transactions
                    .Where(t => t.TransactionType == "CRÉDITO")
                    .OrderByDescending(t => t.TransactionDate)
                    .ToList();

                var totalCount = transactions.Count;
                var paginatedTransactions = transactions.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return Ok(new
                {
                    commerceId,
                    commerceName = commerce.Name,
                    page,
                    pageSize,
                    totalCount,
                    totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                    data = paginatedTransactions
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPost("process-payment/{commerceId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ProcessPayment(int commerceId, [FromBody] ProcessPaymentDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var userId = User.FindFirst("uid")?.Value;

                if (userRole == "Merchant")
                {
                    var merchantCommerce = await _merchantService.GetByIdAsync(commerceId);
                    if (merchantCommerce == null || merchantCommerce.UserId != userId)
                    {
                        return Forbid();
                    }
                }

                var commerce = await _merchantService.GetByIdAsync(commerceId);
                if (commerce == null || !commerce.IsActive)
                {
                    return BadRequest(new { message = "Comercio no válido o inactivo" });
                }

                var cards = await _creditCardService.GetAllWithInclude();
                var card = cards.FirstOrDefault(c => c.CardNumber == dto.CardNumber && c.IsActive);

                if (card == null)
                {
                    return BadRequest(new { message = "Tarjeta no válida o inactiva" });
                }
                var expMonth = int.Parse(dto.MonthExpirationCard);
                var expYear = int.Parse(dto.YearExpirationCard);
                var expirationDate = new DateTime(expYear, expMonth, 1).AddMonths(1).AddDays(-1);

                if (card.ExpirationDate < DateTime.UtcNow || expirationDate < DateTime.UtcNow)
                {
                    return BadRequest(new { message = "Tarjeta expirada" });
                }

                var cvcHash = HashSHA256(dto.CVC);
                if (card.CvcHash != cvcHash)
                {
                    return BadRequest(new { message = "CVC incorrecto" });
                }

                var availableCredit = card.CreditLimit - card.CurrentDebt;
                if (dto.TransactionAmount > availableCredit)
                {
                    await RegisterCardTransaction(card.Id, dto.TransactionAmount, commerce.Name, (int)StatusCardTransaction.Rejected);
                    return BadRequest(new { message = "Fondos insuficientes" });
                }

                var accounts = await _savingsAccountService.GetAllAsync();
                var commerceAccount = accounts.FirstOrDefault(a => a.AccountNumber == commerce.AssociatedAccount);

                if (commerceAccount == null)
                {
                    return BadRequest(new { message = "Cuenta del comercio no encontrada" });
                }

                commerceAccount.Balance += dto.TransactionAmount;
                await _savingsAccountService.UpdateAsync(commerceAccount.Id, commerceAccount);

                card.CurrentDebt += dto.TransactionAmount;
                await _creditCardService.UpdateAsync(card.Id, card);

                await RegisterCardTransaction(card.Id, dto.TransactionAmount, commerce.Name, (int)StatusCardTransaction.Approved);

                var client = await _accountService.GetUserById(card.UserId);
                if (client != null)
                {
                    var lastDigits = card.CardNumber.Substring(card.CardNumber.Length - 4);
                    await _emailService.SendAsync(new EmailRequestDto
                    {
                        To = client.Email,
                        Subject = $"Consumo realizado con la tarjeta {lastDigits} - Artemis Banking",
                        HtmlBody = $@"
                            <h3>Estimado {client.FirstName} {client.LastName},</h3>
                            <p>Se ha realizado un consumo con su tarjeta xxx-xxx-{lastDigits}.</p>
                            <ul>
                                <li>Monto: RD${dto.TransactionAmount:N2}</li>
                                <li>Comercio: {commerce.Name}</li>
                                <li>Fecha: {DateTime.UtcNow:dd/MM/yyyy HH:mm:ss}</li>
                            </ul>
                            <p>Gracias por confiar en Artemis Banking.</p>"
                    });
                }
                await _emailService.SendAsync(new EmailRequestDto
                {
                    To = commerce.Email,
                    Subject = $"Pago recibido - Artemis Banking",
                    HtmlBody = $@"
                        <h3>Estimado {commerce.Name},</h3>
                        <p>Se ha recibido un pago a través de tarjeta de crédito.</p>
                        <ul>
                            <li>Monto: RD${dto.TransactionAmount:N2}</li>
                            <li>Fecha: {DateTime.UtcNow:dd/MM/yyyy HH:mm:ss}</li>
                        </ul>
                        <p>Gracias por usar Artemis Banking.</p>"
                });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        private async Task RegisterCardTransaction(int cardId, decimal amount, string merchant, int status)
        {
            var transaction = new ArtemisBanking.Core.Application.Dtos.CardTransaction.CardTransactionDto
            {
                Id = 0,
                Date = DateTime.UtcNow,
                Amount = amount,
                Merchant = merchant,
                Status = status,
                CreditCardId = cardId
            };

            await _cardTransactionService.AddAsync(transaction);
        }

        #region private methods
        private string HashSHA256(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
        #endregion
    }
}
