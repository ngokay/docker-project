namespace WebApplication5.CQRS.Models
{
    public class AccountCreateDTO
    {
        /// <summary>
        /// Account id
        /// </summary>
        public long AccountId { get; set; }
        /// <summary>
        /// Account name
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// Full name
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// Address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="accountName"></param>
        /// <param name="fullName"></param>
        /// <param name="address"></param>
        public AccountCreateDTO(long accountId, string accountName, string fullName, string address)
        {
            AccountId = accountId;
            AccountName = accountName;
            FullName = fullName;
            Address = address;
        }
    }
}
