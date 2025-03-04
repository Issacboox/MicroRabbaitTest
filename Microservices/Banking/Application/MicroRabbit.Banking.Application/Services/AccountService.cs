
using MicroRabbit.Banking.Application.Interfaces;
using MicroRabbit.Banking.Domain.Interfaces;
using MicroRabbit.Banking.Domain.Model;

namespace MicroRabbit.Banking.Application.Services
{
    public class AccountService(IAccountRepository accountRepository) : IAccountService
    {
        private readonly IAccountRepository _accountRepository = accountRepository;

        public IEnumerable<Account> GetAccount()
        {
            return _accountRepository.GetAccounts();
        }
    }
}