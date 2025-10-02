using Clems.Application.Services;

namespace Clems.Web.Models;

public record DashboardViewModel(
    List<WalletDto> Wallets,
    List<DebtDto> Debts,
    List<TransactionSummaryDto> Summaries
);