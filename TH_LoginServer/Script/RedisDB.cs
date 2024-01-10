using Newtonsoft.Json;
using StackExchange.Redis;

public class RedisDB
{
    private readonly IDatabase _db;
    private readonly IConnectionMultiplexer _redis;

    struct USERINFO
    {
        public string ID;
        public string PW;
        public string AccessToken;
        public SecurityUtils.RefreshToken RefreshToken;
        public string ConnectedIP;
    }

    public RedisDB( IConnectionMultiplexer redis )
    {
        _redis = redis;
        _db = redis.GetDatabase();
    }
    public string SignIn( string ID, string PW, string ipAddress )
    {
        RedisValue userData = _db.StringGet( ID );

        if( userData.IsNullOrEmpty == true )
        {
            throw new Exception( "Not Member!" );
        }

        USERINFO userInfo = JsonConvert.DeserializeObject<USERINFO>( userData );

        if( SecurityUtils.VerifyHashPassword( PW, userInfo.PW ) == false )
        {
            return "FAIL_PASSWORD_NOT_EQAUL";
        }

        userInfo.AccessToken = SecurityUtils.GenerateAccessToken( ID );
        userInfo.RefreshToken = SecurityUtils.GenerateRefreshToken( ipAddress );
        userInfo.ConnectedIP = ipAddress;
        string jsonInfo = JsonConvert.SerializeObject( userInfo );
        _db.StringSet( "testID", jsonInfo );

        userInfo.PW = string.Empty;

        return JsonConvert.SerializeObject( userInfo );
    }

    public void SignUp()
    {
        USERINFO info = new USERINFO();
        info.ID = "testID";
        info.PW = SecurityUtils.GenerateHashPassword( "testPW" );
        info.AccessToken = string.Empty;
        info.RefreshToken = null;
        info.ConnectedIP = null;

        string jsonInfo = JsonConvert.SerializeObject( info );

        _db.StringSet( "testID", jsonInfo );
    }


}
