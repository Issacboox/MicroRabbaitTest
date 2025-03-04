
using MicroRabbit.Banking.Application.Models;
using MicroRabbit.Banking.Domain.Model;

namespace MicroRabbit.Banking.Application.Interfaces
{
    public interface IAccountService
    {
        IEnumerable<Account> GetAccount();
        void TransferFund(AccountTransfer accountTransfer);
    }
}