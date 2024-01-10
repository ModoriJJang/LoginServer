using Microsoft.AspNetCore.Mvc;

namespace TH_LoginServer.Controllers
{

    [ApiController]
    public class LoginController : ControllerBase
    {
        [Route( "api/[controller]/SignIn" )]
        [HttpPost]
        public IActionResult SignIn( [FromBody] SignInData data, RedisDB redisDB, IHttpContextAccessor httpContextAccessor )
        {
            string ipAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            try
            {
                string SignInJson = redisDB.SignIn( data.ID, data.PW, ipAddress );

                if( string.IsNullOrEmpty( SignInJson ) == true )
                {
                    return BadRequest( "SignIn Error" );
                }

                return Ok( SignInJson );
            }
            catch( Exception ex )
            {
                return BadRequest( ex.Message );
            }
        }

        [Route( "api/[controller]/SignUp" )]
        [HttpPost]
        public IActionResult SignUp( [FromBody] SignUpData data, RedisDB redisDB )
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
            public string PW { get; set; } = string.Empty;
        }

        public class SignUpData
        {
            public string ID { get; set; } = string.Empty;
            public string PW { get; set; } = string.Empty;
        }
    }
}
