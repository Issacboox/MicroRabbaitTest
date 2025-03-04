
using MicroRabbit.Banking.Application.Interfaces;
using MicroRabbit.Banking.Application.Models;
using MicroRabbit.Banking.Domain.Commands;
using MicroRabbit.Banking.Domain.Interfaces;
using MicroRabbit.Banking.Domain.Model;
using MicroRabbit.Domain.Core.Bus;

namespace MicroRabbit.Banking.Application.Services
{
    public class AccountService(IAccountRepository accountRepository, IEventBus eventBus) : IAccountService
    {
        private readonly IAccountRepository _accountRepository = accountRepository;
        private readonly IEventBus _eventBus = eventBus;

        public IEnumerable<Account> GetAccount()
        {
            return _accountRepository.GetAccounts();
        }

        public void TransferFund(AccountTransfer accountTransfer)
        {
            var createTrsnsferCommand = new CreateTransferCommand(
                accountTransfer.FromAccount,
                accountTransfer.ToAccount,
                accountTransfer.TransferAmount
            );

            _eventBus.SendCommand(createTrsnsferCommand);
        }
    }
}