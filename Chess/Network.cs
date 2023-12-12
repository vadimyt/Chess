using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Windows.Documents;
using System;
using System.Windows;

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
            try
            {
                client.Connect(adress, port);
                // получаем данные
                byte[] responseBytes = new byte[BUFFLEN];
                int bytes = client.Receive(responseBytes);
                string response = Encoding.UTF8.GetString(responseBytes, 0, bytes);
                return response;
            }
            catch (Exception)
            {
                MessageBox.Show("Не удается подключиться к серверу", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
                Application.Current.Shutdown();
                return "error";
            }
            
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
