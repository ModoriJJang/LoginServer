using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

public class SecurityUtils
{
    private static readonly string _secretKey = "b1144f0489e7c5679d98d9054aec8155b03c7e3c4f8614f653cb11d28bbe27f99dc7e2ace231cc7e66e491fd95b7a2eb13d0660b10a358cbf3d5bf2c939715cc";

    private static readonly string HASHKEY = "TH_LOGINSERVER";

    public class RefreshToken
    {
        public RefreshToken( string connectIP )
        {
            Token = GenerateToken();
            Expires = DateTime.UtcNow.AddDays( 7 );
            Created = DateTime.UtcNow;
            ConnectedIP = connectIP;
        }

        public string Token;
        public DateTime Expires;
        public DateTime Created;
        public string ConnectedIP;

        public string GenerateToken()
        {
            return Convert.ToBase64String( RandomNumberGenerator.GetBytes( 64 ) );
        }
    }

    public static string GenerateAccessToken( string userID )
    {
        string accessSecretKey = _secretKey;
        SymmetricSecurityKey securityKey = new SymmetricSecurityKey( System.Text.Encoding.UTF8.GetBytes( accessSecretKey ) );
        SigningCredentials signingCredentials = new SigningCredentials( securityKey, SecurityAlgorithms.HmacSha256Signature );

        SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor();
        descriptor.Subject = new ClaimsIdentity( new[] { new Claim( ClaimTypes.Name, userID ) } );
        descriptor.Expires = DateTime.UtcNow.AddMinutes( 30 );
        descriptor.SigningCredentials = signingCredentials;

        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        JwtSecurityToken token = handler.CreateJwtSecurityToken( descriptor );

        string accessToken = handler.WriteToken( token );

        return accessToken;
    }

    public static bool ValidateAccessToken( string token )
    {
        string accessSecretKey = _secretKey;
        SymmetricSecurityKey securityKey = new SymmetricSecurityKey( System.Text.Encoding.UTF8.GetBytes( accessSecretKey ) );

        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

        TokenValidationParameters validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = securityKey,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero,
        };

        handler.ValidateToken( token, validationParameters, out SecurityToken validatedToken );
        if( validatedToken == null )
        {
            throw new Exception( "FAIL_NOT_EQUAL_TOKENINFO" );
        }

        JwtSecurityToken jwtToken = (JwtSecurityToken)validatedToken;

        bool LifeTimeCheck = jwtToken.ValidTo > DateTime.UtcNow;

        return LifeTimeCheck;
    }

    public static string GenerateRefreshToken( string ipAddress )
    {
        RefreshToken refreshToken = new RefreshToken( ipAddress );
        string token = JsonConvert.SerializeObject( refreshToken );
        return token;
    }

    public static bool ValidateRefreshToken( string token )
    {
        RefreshToken refreshToken = JsonConvert.DeserializeObject<RefreshToken>( token );

        bool LifeTimeCheck = refreshToken.Expires > DateTime.UtcNow;
        return LifeTimeCheck;
    }

    public static string GenerateHashPassword( string data )
    {
        //Salting & Key Stretching
        //특정 문자열을 추가해 원본값을 유추하지 못하도록 처리
        string hashString = HASHKEY + data;
        return BCrypt.Net.BCrypt.HashPassword( hashString );
    }

    public static bool VerifyHashPassword( string data, string hashData )
    {
        string hashString = HASHKEY + data;
        return BCrypt.Net.BCrypt.Verify( hashString, hashData );
    }


}
