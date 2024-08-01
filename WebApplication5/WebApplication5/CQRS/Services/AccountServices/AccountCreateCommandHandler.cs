using Mapster;
using MediatR;
using WebApplication5.CQRS.Accounts.Command;
using WebApplication5.CQRS.Models;
using WebApplication5.CQRS.Repositories.Accounts;

namespace WebApplication5.CQRS.Services.AccountServices
{
    public class AccountCreateCommandHandler : IRequestHandler<AccountCreateCommand, AccountCreateCommandResult>
    {
        /// <summary>
        /// Account repository
        /// </summary>
        private readonly IAccounts account;
        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<AccountCreateCommandHandler> logger;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="account"></param>
        /// <param name="logger"></param>
        public AccountCreateCommandHandler(IAccounts account, ILogger<AccountCreateCommandHandler> logger)
        {
            this.account = account;
            this.logger = logger;
        }
        /// <summary>
        /// Handler
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<AccountCreateCommandResult> Handle(AccountCreateCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Start create account");
            // xử lý logic 
            int result = await account.AccountCreate(request.accountDto.Adapt<AccountCreateDTO>());
            // result
            logger.LogInformation("End create account");
            return new AccountCreateCommandResult(result);
        }
    }
}
