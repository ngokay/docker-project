using WebApplication5.CQRS.Models;
using System.Linq;

namespace WebApplication5.CQRS.Repositories.Accounts
{
    public interface IAccounts
    {
        /// <summary>
        /// Create account
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        Task<int> AccountCreate(AccountCreateDTO accountDto);
        Task<IList<AccountInfo>> GetListAccounts(long companyId);
    }
}
