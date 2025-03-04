
using MicroRabbit.Banking.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace MicroRabbit.Banking.Data.Context
{
    public class BankingDbContext(DbContextOptions options) : DbContext(options)
    {
        private readonly DbContextOptions _options = options;

        public DbSet<Account> Accounts { get; set; }
    }
}