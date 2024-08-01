using Mapster;
using MediatR;
using WebApplication5.CQRS.Integration.Accounts.Query;
using WebApplication5.CQRS.Repositories.Accounts;

namespace WebApplication5.CQRS.Services.AccountServices
{
    public class AccountGetListQueryHandler : IRequestHandler<AccountGetListQuery, AccountGetListQueryResult>
    {
        private readonly IAccounts accounts;
        private readonly ILogger<AccountGetListQueryHandler> logger;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="accounts"></param>
        /// <param name="logger"></param>
        public AccountGetListQueryHandler(IAccounts accounts, ILogger<AccountGetListQueryHandler> logger)
        {
            this.accounts = accounts;
            this.logger = logger;
        }
        /// <summary>
        /// Handler
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<AccountGetListQueryResult> Handle(AccountGetListQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Start account get list");
            var accountLst = await accounts.GetListAccounts(request.CompanyId);
            logger.LogInformation("End account get list");
            return new AccountGetListQueryResult(accountLst.Adapt<List<AccountResultInfo>>());
        }
    }
}
