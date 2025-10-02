namespace Clems.Application.Exceptions;

public class WalletNotFoundException(Guid eventWalletId) : Exception($"Wallet with id {eventWalletId} not found");