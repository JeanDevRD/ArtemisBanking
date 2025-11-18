using System;
using System.Collections.Generic;
using ArtemisBanking.Core.Application.Dtos.Transaction;

namespace ArtemisBanking.Core.Application.ViewModels.Transacciones
{
    public class TransactionFilterViewModel
    {
        public List<TransactionDto> Transactions { get; set; } = new();
        public string? Type { get; set; } // Deposit, withdrawal, transfer, etc.
        public string? Product { get; set; } // Account, loan, card
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Search { get; set; } // Reference, amount or description
    }
}

