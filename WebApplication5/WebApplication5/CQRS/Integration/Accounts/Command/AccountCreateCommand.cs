using MediatR;

namespace WebApplication5.CQRS.Accounts.Command
{
    public class AccountCreateCommand : IRequest<AccountCreateCommandResult>
    {
        /// <summary>
        /// Account create dto
        /// </summary>
        public AccountCommandDTO accountDto { get; set; }

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="accountDto"></param>
        public AccountCreateCommand(AccountCommandDTO accountDto)
        {
            this.accountDto = accountDto;
        }
    }

    public class AccountCommandDTO
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
    }
}

