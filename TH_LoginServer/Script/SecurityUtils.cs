using Microsoft.IdentityModel.Tokens;
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

        string refreshToken = handler.WriteToken( token );

        return refreshToken;
    }

    public static RefreshToken GenerateRefreshToken( string ipAddress )
    {
        RefreshToken refreshToken = new RefreshToken( ipAddress );
        return refreshToken;
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
