namespace Clems.Application.Services;

public abstract record TransactionCreateDto(Guid WalletId, decimal In, decimal Out, string Description = "");