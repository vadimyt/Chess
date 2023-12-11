using System.Net.Sockets;
using System.Net;
using System.Text;
namespace Chess
{
    internal class Network
    {
        Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        int port = 45245;
        IPAddress adress = IPAddress.Parse("82.179.140.18");
        int BUFFLEN = 512;        
        public string con()
        {
            client.Connect(adress, port);
            // получаем данные
            byte[] responseBytes = new byte[BUFFLEN];
            int bytes = client.Receive(responseBytes);
            string response = Encoding.UTF8.GetString(responseBytes, 0, bytes);
            return response;
        }
        public void snd(string str)
        {
            var sendBytes=Encoding.UTF8.GetBytes(str);
            client.Send(sendBytes);
        }
        public string rcv()
        {
            byte[] responseBytes = new byte[BUFFLEN];
            int bytes = client.Receive(responseBytes);
            string response = Encoding.UTF8.GetString(responseBytes, 0, bytes);
            return response;
        }
    }
}
