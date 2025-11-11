using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Infraestructure.Persistence.Contexts;

namespace ArtemisBanking.Infraestructure.Persistence.Repositories
{
    public class CardTransactionRepository : GenericRepository<CardTransaction>, ICardTransactionRepository
    {
        public CardTransactionRepository(ArtemisBankingContextSqlServer context) : base(context)
        {
        }

        public async Task<bool> PaymentCard(int creditCardId, decimal amount)
        {
            var creditCard = await _context.CreditCards.FindAsync(creditCardId);
            
            if (creditCard == null || creditCard.CurrentDebt < amount)
            {
                return false; 
            }

            creditCard.CurrentDebt -= amount;
            _context.CreditCards.Update(creditCard);

            await _context.SaveChangesAsync();
            return true; 
        }
    }
}
