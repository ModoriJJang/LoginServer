using StackExchange.Redis;
using Newtonsoft.Json;

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
		public string IdToken;
	}

	public RedisDB( IConnectionMultiplexer redis )
	{
		_redis = redis;
		_db = redis.GetDatabase();
	}
	public bool SignIn(string ID, string PW)
	{
		RedisValue userData = _db.StringGet( ID );
		if( userData.IsNullOrEmpty == true  )
		{
			return false;
		}

		USERINFO userInfo = JsonConvert.DeserializeObject<USERINFO>( userData );

		if( userInfo.PW == PW )
		{
			userInfo.AccessToken = "2";
			userInfo.RefreshToken = "2";
			userInfo.IdToken = "2";
			string jsonInfo = JsonConvert.SerializeObject( userInfo );
			_db.StringSet( "testID", jsonInfo );
			return true;
		}

		return false;
	}

	public void SignUp()
	{
		USERINFO info = new USERINFO();
		info.ID = "testID";
		info.PW = "testPW";
		info.AccessToken = "1";
		info.RefreshToken = "1";
		info.IdToken = "1";

		string jsonInfo = JsonConvert.SerializeObject( info );

		_db.StringSet( "testID", jsonInfo );
	}

	
}
