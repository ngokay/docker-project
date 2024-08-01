using Moq;
using WebApplication5.CQRS.Accounts.Command;
using WebApplication5.CQRS.Models;
using WebApplication5.CQRS.Repositories.Accounts;
using WebApplication5.CQRS.Services.AccountServices;
using Xunit;

namespace WebApplication5.Tests.CQRSTest.ServicesTest.AccountServicesTest
{
    public class AccountCreateCommandHandlerTest
    {
        private readonly Mock<IAccounts> mockAccount;
        private readonly Mock<ILogger<AccountCreateCommandHandler>> mockLogger;

        private readonly AccountCreateCommandHandler handler;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="mockAccount"></param>
        /// <param name="mockLogger"></param>
        public AccountCreateCommandHandlerTest()
        {
            mockAccount = new Mock<IAccounts>();
            mockLogger = new Mock<ILogger<AccountCreateCommandHandler>>();
            handler = new AccountCreateCommandHandler(mockAccount.Object,mockLogger.Object);
        }

        [Fact]
        public async Task AccountCreateCommandHandlerSuccess()
        {
            //Arrange configure
            int accountResult = 1;
            mockAccount.Setup(x => x.AccountCreate(It.IsAny<AccountCreateDTO>())).ReturnsAsync(1);

            //Act hành động
            var command = new AccountCreateCommand(new AccountCommandDTO
            {
                AccountId=1,
                AccountName = "abc",
                Address = "abc",
                FullName = "abc"
            });
            var result = await handler.Handle(command, It.IsAny<CancellationToken>());
            
            //Assert kiểm tra
            Assert.NotNull(result);
            Assert.Equal(result.CountSuccess, accountResult);
        }

        [Theory]
        [InlineData(1)]
        public async Task AccountCreateCommandHandlerSuccessTestTheod(int accountResult)
        {
            //Arrange configure
            mockAccount.Setup(x => x.AccountCreate(It.IsAny<AccountCreateDTO>())).ReturnsAsync(1);

            //Act hành động
            var command = new AccountCreateCommand(new AccountCommandDTO
            {
                AccountId = 1,
                AccountName = "abc",
                Address = "abc",
                FullName = "abc"
            });
            var result = await handler.Handle(command, It.IsAny<CancellationToken>());

            //Assert kiểm tra
            Assert.NotNull(result);
            Assert.Equal(result.CountSuccess, accountResult);
        }

        [Theory, MemberData(nameof(SplitCountData))]
        public async Task AccountCreateCommandHandlerSuccessTestMemberData(TestMemberData2 testMemberData2,TestMemberData1 testMemberData1)
        {
            //Arrange configure
            mockAccount.Setup(x => x.AccountCreate(It.IsAny<AccountCreateDTO>())).ReturnsAsync(1);

            //Act hành động
            int accountResult = 1;
            var command = new AccountCreateCommand(new AccountCommandDTO
            {
                AccountId = 1,
                AccountName = "abc",
                Address = "abc",
                FullName = "abc"
            });
            var result = await handler.Handle(command, It.IsAny<CancellationToken>());

            //Assert kiểm tra
            Assert.NotNull(result);
            Assert.Equal(result.CountSuccess, accountResult);

            // Check object memberdata2 
            Assert.NotNull(testMemberData2);
            Assert.Equal(1,testMemberData2.AccountId);
            Assert.Equal(10,testMemberData2.YearOld);

            // Check object memberdata1
            Assert.NotNull(testMemberData1);
            Assert.Equal("abc", testMemberData1.Name);
            Assert.Equal("bcd", testMemberData1.AccountName);

        }

        public static IEnumerable<object[]> SplitCountData =>
            new List<object[]>
            {
                new object[] 
                { 
                    new TestMemberData2
                    {
                        AccountId = 1,
                        YearOld = 10
                    } ,
                    new TestMemberData1
                    {
                        Name ="abc",
                        AccountName = "bcd"
                    }
                }
            };
    }

    public class TestMemberData1
    {
        public string Name { get; set; }
        public string AccountName { get; set; }
    }

    public class TestMemberData2
    {
        public int AccountId { get; set; }
        public int YearOld { get; set; }
    }
}
