
using Grpc.Core;
using Grpc.Net.Client;
using System.Net.NetworkInformation;

public class gRPC_System
{

    public static async void Login_Server(string clientid, string token)
    {
        using var channel = GrpcChannel.ForAddress( "http://host.docker.internal:9876" );

        //Channel channel = new Channel( "localhost:5004", ChannelCredentials.Insecure );
        var client = new gRPC_Client.RPC_System.RPC_SystemClient( channel );

       
        gRPC_Client.Login_Send sendData = new gRPC_Client.Login_Send();
        sendData.Token = token;
        sendData.Clientid = clientid;

        //var temp = client.RPC_Login( sendData );


        var async = await client.RPC_LoginAsync( sendData );

        int b = 0;
    }
}

