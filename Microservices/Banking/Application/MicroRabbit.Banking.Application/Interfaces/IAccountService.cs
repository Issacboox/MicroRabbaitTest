
using MicroRabbit.Banking.Domain.Model;

namespace MicroRabbit.Banking.Application.Interfaces
{
    public interface IAccountService
    {
        IEnumerable<Account> GetAccount();
    }
}