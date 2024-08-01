using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using WebApplication5.Attributes;
using WebApplication5.CQRS.Accounts.Command;
using WebApplication5.CQRS.Integration.Accounts.Query;
using WebApplication5.DependencyInjection;

namespace WebApplication5.Controllers.v2
{

    public class TestController : BaseController
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private readonly IClassA _classA;
        private readonly IClassB _classB;

        private readonly ILogger<TestController> _logger;
        private readonly IMediator mediator;
        public TestController(ILogger<TestController> logger, IClassA classA, IClassB classB, IMediator mediator)
        {
            _logger = logger;
            _classA = classA;
            _classB = classB;
            this.mediator = mediator;
            Console.WriteLine("TestYield(array)" + JsonConvert.SerializeObject(TestYield(array)));
        }
        [HttpGet("Test")]
        [ExcusionAttribute("Test1")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost("account-create")]
        [AllowAnonymous]
        public async Task<IActionResult> AccountCreate()
        {
            object accountDTO = new
            {
                AccountId = 1,
                AccountName = "anhnt490",
                FullName = "Ngô Thế Anh",
                Address = "Test test test"
            };
            var command = new AccountCreateCommand(accountDTO.Adapt<AccountCommandDTO>());
            var result = await mediator.Send(command);
            return Ok(result.CountSuccess);
        }

        [HttpGet("account-get-list")]
        [AllowAnonymous]
        public async Task<IActionResult> AccountGetList([FromHeader][Required] long companyId)
        {
            var query = new AccountGetListQuery(companyId);
            var result = await mediator.Send(query);
            return Ok(result.accountInfo);
        }
        private IList<int> array = new List<int>() { 1,2, 2, 3, 4, 5,4,4,2,3,2,3 };
        private IEnumerable<int> TestYield(IList<int> array, int value = 2)
        {
            for(int i = 0; i < array.Count; i++)
            {
                if(array[i] == value)
                {
                    yield return i;
                }
            }
        }
    }
}