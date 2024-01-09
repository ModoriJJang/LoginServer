using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices.JavaScript;

namespace TH_LoginServer.Controllers
{

    [ApiController]
    public class LoginController : ControllerBase
    {
        [Route( "api/[controller]/SignIn" )]
        [HttpPost]
        public IActionResult SignIn([FromBody] SignInData data, RedisDB redisDB)
        {
            redisDB.SignIn( data.ID, data.PW );

            if( redisDB.SignIn( data.ID, data.PW ) )
            {
                return Ok( "OK" );
            }

            return BadRequest("FAIL");
        }

        [Route( "api/[controller]/SignUp" )]
        [HttpPost]
        public IActionResult SignUp([FromBody] SignUpData data, RedisDB redisDB)
        {
            redisDB.SignUp();
            return Ok( "" );
        }

        [Route( "api/[controller]/ReSign" )]
        [HttpPost]
        public IActionResult ReSign()
        {
            return Ok( "" );
        }

        public class SignInData()
        {
            public string ID { get; set; } = string.Empty;
            public string PW {get;set;} = string.Empty;
        }

        public class SignUpData
        {
            public string ID {get;set;} = string.Empty;
            public string PW {get;set;} = string.Empty;
        }
    }
}
