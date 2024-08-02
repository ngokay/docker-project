using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using WebApplication5.Authentication;
using WebApplication5.RabbitMQ;
using WebApplication5.Redis;

namespace WebApplication5.Controllers
{
    public class LoginController : BaseController
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment environment;
        private readonly IRedisCacheService redisCacheService;
        private readonly ILogger<LoginController> logger;
        private readonly ISendMessage sendMessage;

        public static Dictionary<string, IList<dynamic>> msg = new Dictionary<string, IList<dynamic>>()
        {

            ["Direct"] = new List<dynamic>() {
                new {
                    RoutingKey = "Direct-RoutingKey-1",
                    ExchangeName = "Direct-Exchange",
                    Msg = new
                        {
                            name = "Msg-Direct-QueueName-1",
                            msg = "Msg-Direct-QueueName-1"
                        } 
                },
                new {
                    RoutingKey = "Direct-RoutingKey-2",
                    ExchangeName = "Direct-Exchange",
                    Msg = new
                        {
                            name = "Msg-Direct-QueueName-2",
                            msg = "Msg-Direct-QueueName-2"
                        }
                }
            },
            ["Fanout"] = new List<dynamic>() {
                new {
                    RoutingKey = String.Empty,
                    ExchangeName = "Fanout-Exchange",
                    Msg = new
                        {
                            name = "Msg-Fanout-QueueName-1",
                            msg = "Msg-Fanout-QueueName-1"
                        }
                },
                new {
                    RoutingKey = String.Empty,
                    ExchangeName = "Fanout-Exchange",
                    Msg = new
                        {
                            name = "Msg-Fanout-QueueName-2",
                            msg = "Msg-Fanout-QueueName-2"
                        }
                },
            },
            ["Topic"] = new List<dynamic>() {
                new {
                    RoutingKey = "Topic.abc.Exchange-1",
                    ExchangeName = "Topic-Exchange-1",
                    Msg = new
                        {
                            name = "Msg-Topic-QueueName-1 Topic.abc.Exchange-1",
                            msg = "Msg-Topic-QueueName-1 Topic.abc.Exchange-1"
                        } 
                },
                new {
                    RoutingKey = "ggg.Exchange-1",
                    ExchangeName = "Topic-Exchange-1",
                    Msg = new
                        {
                            name = "Msg-Topic-QueueName-1-all ggg.Exchange-1",
                            msg = "Msg-Topic-QueueName-1-all ggg.Exchange-1"
                        }  
                },
                new {
                    RoutingKey = "Topic.tttt.Exchange-2",
                    ExchangeName = "Topic-Exchange-2",
                    Msg = new
                        {
                            name = "Msg-Topic-QueueName-2 Topic.tttt.Exchange-2",
                            msg = "Msg-Topic-QueueName-2 Topic.tttt.Exchange-2"
                        }   
                },
                new {
                    RoutingKey = "uuuuu.Exchange-2",
                    ExchangeName = "Topic-Exchange-2",
                    Msg = new
                        {
                            name = "Msg-Topic-QueueName-2 uuuuu.Exchange-2",
                            msg = "Msg-Topic-QueueName-2 uuuuu.Exchange-2"
                        }    
                }
            },
            ["Headers"] = new List<dynamic>() {
                new {
                    RoutingKey = "Headers-Exchange-1",
                    ExchangeName = "Headers-Exchange",
                    Msg = new
                        {
                            name = "Msg-Headers-QueueName-2 uuuuu.Exchange-1",
                            msg = "Msg-Headers-QueueName-2 uuuuu.Exchange-1"
                        }     
                },
                new {
                    RoutingKey = "Headers-Exchange-2",
                    ExchangeName = "Headers-Exchange",
                    Msg = new
                        {
                            name = "Msg-Headers-QueueName-2 uuuuu.Exchange-2",
                            msg = "Msg-Headers-QueueName-2 uuuuu.Exchange-2"
                        }
                }
            }
        };
        public LoginController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IRedisCacheService redisCacheService, 
            ILogger<LoginController> logger, ISendMessage sendMessage)
        {
            this.configuration = configuration;
            this.environment = webHostEnvironment;
            this.redisCacheService = redisCacheService;
            this.logger = logger;
            this.sendMessage = sendMessage;
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login(LoginDTO loginDTO)
        {
            logger.LogInformation("Action login : da vao login");
            logger.LogError("Action Error : Error");
            logger.LogWarning("Action Warning : Warning");
            Console.WriteLine("Action console : WriteLine");
            Console.Write("Action console Write : Write");
            
            int countLoop = 10000;
            IList<dynamic> lstMsg = new List<dynamic>();
            for (int i = 0; i < countLoop; i++)
            {
                dynamic msg = new
                {
                    name = "abc " + i,
                    msg = "Test send message to RabbitMQ " + i
                };
                lstMsg.Add(msg);
                
            }
            var watch = Stopwatch.StartNew();
            foreach (var item in lstMsg)
            {
                sendMessage.Send(item);
            }

            watch.Stop();

            Console.WriteLine($"Loop count foreach | Total time : {countLoop} | Time Taken : {watch.ElapsedMilliseconds} ms.");
            var watchParallel = Stopwatch.StartNew();
            Parallel.ForEach(lstMsg, msg => {
                sendMessage.Send(msg);
            });

            watchParallel.Stop();
            Console.WriteLine($"Loop count Parallel | Total time : {countLoop} | Time Taken : {watchParallel.ElapsedMilliseconds} ms.");

            sendMessage.CreateQueue(msg);
            try
            {
                if (string.IsNullOrEmpty(loginDTO.Username) ||
                string.IsNullOrEmpty(loginDTO.Password))
                    return BadRequest("Username and/or Password not specified");
                if (loginDTO.Username.Equals("anhnt490") &&
                loginDTO.Password.Equals("anhnt490"))
                {
                    // set token value in db redis
                    object token = GenToken();
                    string keyRedis = string.Format("{0}.{1}", "anhnt490","accessToken");
                    // set redis key value
                    redisCacheService.SetData(keyRedis,token,DateTime.Now.AddDays(2));

                    logger.LogInformation("Action login : xu ly xong login");

                    return Ok(token);
                }
            }
            catch(Exception ex)
            {
                logger.LogInformation("Error gentoken" +ex.Message);
                return BadRequest
                ("An error occurred in generating the token :"+ ex.Message);
            }
            return Unauthorized();
        }
        [AllowAnonymous]
        [HttpPost("refresh-token")]

        public IActionResult RefreshToken(string refreshToken)
        {
            // test single threading
            Console.WriteLine("Start single thread");
            Thread singleThreading = new Thread(new ThreadStart(FunctionSingleThreading));
            singleThreading.Start();
            Console.WriteLine("End single thread");
            

            Console.WriteLine("Start multiple thread");
            MyAsyncFunction();
            Console.WriteLine("End multiple thread");

            string refreshTokenKey = configuration["jwt:refreshToken"];
            if (refreshToken.Equals(refreshTokenKey))
            {
                // update info refresh token
                AddOrUpdateAppSetting("jwt:refreshToken", Guid.NewGuid().ToString());
                //configuration["jwt:refreshToken"] = Guid.NewGuid().ToString();

                int a = 1;
                SumToTal(ref a);
                Console.WriteLine("Test ref: "+ a);

                int b;
                CheckOut(out b);
                Console.WriteLine("Test out:" + b);

                return Ok(GenToken());

            }
            return BadRequest();

            async Task MyAsyncFunction()
            {
                await Task.Run(() => FunctionSingleThreading());
                await Task.Run(() => FunctionMultiThreading());
            }
        }

        [HttpPost("revock-token")]
        [AllowAnonymous]
        public IActionResult RevockToken(string token)
        {
            string keyRedis = string.Format("{0}.{1}", "anhnt490", "revockToken");

            bool statusRevock = redisCacheService.SetData(keyRedis, token, DateTime.Now.AddDays(2));
            return Ok(new { code = HttpStatusCode.OK,status = statusRevock});
        }

        private object GenToken()
        {
            string secretKeyConfigure = configuration["jwt:secretKey"];
            var secretKey = new SymmetricSecurityKey
            (Encoding.UTF8.GetBytes(secretKeyConfigure));
            var signinCredentials = new SigningCredentials
            (secretKey, SecurityAlgorithms.HmacSha256);

            object authorized = new
            {
                action = "WeatherForecast"
            };

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: configuration["jwt:issuer"],
                audience: "http://localhost:51398",
                claims: new List<Claim>()
                {
                            new Claim("UserName","anhnt490"),
                            new Claim("DateOfBirth","2024-01-01"),
                            new Claim("Sex","Nam"),
                            new Claim("FullName","Ngo The Anh"),
                            new Claim("authorized",JsonConvert.SerializeObject(authorized))
                },
                expires: DateTime.Now.AddMinutes(1),
                signingCredentials: signinCredentials
            );
            return new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                RefreshToken = configuration["jwt:refreshToken"]
            };
        }
        private static void AddOrUpdateAppSetting<T>(string sectionPathKey, T value)
        {
            try
            {
                string fileJson = string.Format("appsettings.{0}.json", Microsoft.AspNetCore.Hosting.EnvironmentName.Development);
                var filePath = Path.Combine(AppContext.BaseDirectory, fileJson);
                string json = System.IO.File.ReadAllText(filePath);
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                SetValueRecursively(sectionPathKey, jsonObj, value);

                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                System.IO.File.WriteAllText(filePath, output);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing app settings | {0}", ex.Message);
            }
        }
        private static void SetValueRecursively<T>(string sectionPathKey, dynamic jsonObj, T value)
        {
            // split the string at the first ':' character
            var remainingSections = sectionPathKey.Split(":", 2);

            var currentSection = remainingSections[0];
            if (remainingSections.Length > 1)
            {
                // continue with the procress, moving down the tree
                var nextSection = remainingSections[1];
                SetValueRecursively(nextSection, jsonObj[currentSection], value);
                count(1,2);
                Console.WriteLine("TEST"+ count(1, 2));
            }
            else
            {
                // we've got to the end of the tree, set the value
                jsonObj[currentSection] = value;
            }

            static int count(int a,int b)
            {
                return a + b;
            }
        }

        /// <summary>
        /// Test tham chiếu, ref and out
        /// </summary>
        /// <param name="a"></param>
        private void SumToTal(ref int a)
        {
            a++;
        }
        /// <summary>
        /// Test tham chiếu, ref and out
        /// </summary>
        /// <param name="b"></param>
        private void CheckOut(out int b)
        {
            b = 1;
            b++;
        }

        private void FunctionSingleThreading()
        {
            Thread.Sleep(3 * 60*1000);
            Console.WriteLine($"Test single threading");
        }

        private void FunctionMultiThreading()
        {
            Thread.Sleep(6 * 60 * 1000);
            Console.WriteLine($"Test multiple threading");
        }


    }
}
