using Newtonsoft.Json;
using WebApplication5.CQRS.Models;

namespace WebApplication5.CQRS.Repositories.Accounts
{
    public class Accounts : IAccounts
    {
        /// <summary>
        /// Log sql
        /// </summary>
        private readonly ILogger<Accounts> logger;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="logger"></param>
        public Accounts(ILogger<Accounts> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Account create
        /// </summary>
        /// <param name="accountDto"></param>
        /// <returns></returns>
        public async Task<int> AccountCreate(AccountCreateDTO accountDto)
        {
            // account to DB
            var sql = "Select * From abc";
            var param = new { test = "1", note = "2" };
            logger.LogInformation(String.Format("sql={0},param={1}", sql, JsonConvert.SerializeObject(param)));
            return 1;
        }

        public async Task<IList<AccountInfo>> GetListAccounts(long companyId)
        {
            // account to DB
            var sql = "Select * From abc";
            var param = new { test = "select", companyId};
            logger.LogInformation(String.Format("sql={0},param={1}", sql, JsonConvert.SerializeObject(param)));
            return Enumerable.Empty<AccountInfo>().ToList();
        }
    }
}
