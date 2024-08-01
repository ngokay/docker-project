namespace WebApplication5.CQRS.Accounts.Command
{
    public class AccountCreateCommandResult
    {
        /// <summary>
        /// Total create account success
        /// </summary>
        public int CountSuccess { get; set; }
        
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="countSuccess"></param>
        public AccountCreateCommandResult(int countSuccess)
        {
            CountSuccess = countSuccess;
        }
    }
}
