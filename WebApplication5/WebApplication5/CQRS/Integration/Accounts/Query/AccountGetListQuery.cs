using MediatR;

namespace WebApplication5.CQRS.Integration.Accounts.Query
{
    public class AccountGetListQuery : IRequest<AccountGetListQueryResult>
    {
        public long CompanyId { get; set; }

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="companyId"></param>
        public AccountGetListQuery(long companyId)
        {
            CompanyId = companyId;
        }
    }
}
