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
        public string RefreshToken;
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
        _db.StringSet( ID, jsonInfo );

        userInfo.PW = string.Empty;

        return JsonConvert.SerializeObject( userInfo );
    }

    public void SignUp(string ID, string PW)
    {
        USERINFO info = new USERINFO();
        info.ID = ID;
        info.PW = SecurityUtils.GenerateHashPassword( PW );
        info.AccessToken = string.Empty;
        info.RefreshToken = null;
        info.ConnectedIP = null;

        string jsonInfo = JsonConvert.SerializeObject( info );

        _db.StringSet( ID, jsonInfo );
    }

    public string ReissueAccessToken(string userID, string ipAddress)
    {
        RedisValue userData = _db.StringGet( userID );
        if( userData.IsNullOrEmpty == true )
        {
            throw new Exception( "Not Member!" );
        }

        USERINFO userInfo = JsonConvert.DeserializeObject<USERINFO>( userData );
        userInfo.AccessToken = SecurityUtils.GenerateAccessToken( userID );
        userInfo.ConnectedIP = ipAddress;

        string jsonInfo = JsonConvert.SerializeObject( userInfo );

        _db.StringSet( "testID", jsonInfo );
        userInfo.PW = string.Empty;

        return JsonConvert.SerializeObject( userInfo );
    }
    public string ReissueRefreshToken(string userID, string ipAddress)
    {
        RedisValue userData = _db.StringGet( userID );
        if( userData.IsNullOrEmpty == true )
        {
            throw new Exception( "Not Member!" );
        }

        USERINFO userInfo = JsonConvert.DeserializeObject<USERINFO>( userData );
        userInfo.RefreshToken = SecurityUtils.GenerateRefreshToken( ipAddress );
        userInfo.ConnectedIP = ipAddress;

        string jsonInfo = JsonConvert.SerializeObject( userInfo );

        _db.StringSet( "testID", jsonInfo );
        userInfo.PW = string.Empty;

        return JsonConvert.SerializeObject( userInfo );
    }

}
