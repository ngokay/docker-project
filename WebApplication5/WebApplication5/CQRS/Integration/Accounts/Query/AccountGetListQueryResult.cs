namespace WebApplication5.CQRS.Integration.Accounts.Query
{
    public class AccountGetListQueryResult
    {
        /// <summary>
        /// account info
        /// </summary>
        public IList<AccountResultInfo> accountInfo { get; set; }
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="accountInfo"></param>
        public AccountGetListQueryResult(IList<AccountResultInfo> accountInfo)
        {
            this.accountInfo = accountInfo;
        }
    }

    public class AccountResultInfo
    {
        public int Id { get; set; }
        public string AccountName { get; set; }
        public string FullName { get; set; }

    }
}
