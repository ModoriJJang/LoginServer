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
            try
            {
                string ipAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

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
            redisDB.SignUp( data.ID, data.PW );
            return Ok( "" );
        }

        [Route( "api/[controller]/ReSign" )]
        [HttpPost]
        public IActionResult ReSign()
        {
            return Ok( "" );
        }

        [Route( "api/[controller]/ValidataToken" )]
        [HttpPost]
        public IActionResult ValidataToken([FromBody] TokenData data, RedisDB redisDB, IHttpContextAccessor httpContextAccessor )
        {
            try
            {
                string ipAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                string returnJson = string.Empty;

                bool ValidateAccessTokenCheck = SecurityUtils.ValidateAccessToken( data.AccessToken );
                bool ValidateRefreshTokenCheck = SecurityUtils.ValidateRefreshToken( data.RefreshToken );

                if( ValidateAccessTokenCheck == false && ValidateRefreshTokenCheck == true )
                {
                    returnJson = redisDB.ReissueAccessToken( data.ID, ipAddress );
                }
                else if( ValidateAccessTokenCheck == true && ValidateRefreshTokenCheck == false )
                {
                    returnJson = redisDB.ReissueRefreshToken( data.ID, ipAddress );
                }
                else
                {
                    return Ok( "ReEnterSessionPlease" );
                }
                
                return Ok( returnJson );
            }
            catch( Exception ex )
            {
                return BadRequest( ex.Message );
            }
        }

        public class SignInData()
        {
            public string ID { get; set; } = string.Empty;
            public string PW { get; set; } = string.Empty;
            public string Client { get; set; } = string.Empty;
        }

        public class SignUpData
        {
            public string ID { get; set; } = string.Empty;
            public string PW { get; set; } = string.Empty;
            public string Client { get; set; } = string.Empty;
        }

        public class TokenData
        {
            public string ID { get; set; } = string.Empty;
            public string AccessToken { get; set; } = string.Empty;
            public string RefreshToken { get; set; } = string.Empty;
        }
    }
}
