using Newtonsoft.Json;
using signup_sheet_server.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace signup_sheet_server.DataExchange
{
    class Communication
    {
        private TcpListener listener;
        private Control parent;

        public void StartServer(Control parent, int port)
        {
            // Save the parent for future callback usage.
            this.parent = parent;

            // Assign listeneing target.
            IPEndPoint localIp = new IPEndPoint(IPAddress.Any, port);
            this.listener = new TcpListener(localIp);

            this.listener.Start();

            WaitForClientConnect();
        }
        public void StopServer()
        {
            // Raise the flag to stop the async callback function.
            this.listener.Stop();
        }

        private void WaitForClientConnect()
        {
            object obj = new object();
            this.listener.BeginAcceptTcpClient(new AsyncCallback(OnClientConnect), obj);
        }

        private void OnClientConnect(IAsyncResult async)
        {
            try
            {
                TcpClient clientSocket = this.listener.EndAcceptTcpClient(async);
                ClientHandler clientRequest = new ClientHandler(this.parent, clientSocket);
                clientRequest.StartClient();
            }
            catch(ObjectDisposedException)
            {
                return;
            }
            catch
            {
                throw;
            }

            WaitForClientConnect();
        }

        #region Accessories.

        public string GetIp()
        {
            IPHostEntry host;
            string localIP = "Unknown.";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach(IPAddress ip in host.AddressList)
            {
                if(ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }

        #endregion
    }
    
    class ClientHandler
    {
        private TcpClient clientSocket;
        private NetworkStream networkStream = null;

        private Control parent;

        public ClientHandler(Control parent, TcpClient connectedClient)
        {
            this.parent = parent;
            this.clientSocket = connectedClient;
        }

        public void StartClient()
        {
            this.networkStream = this.clientSocket.GetStream();
            WaitForRequest();
        }
        
        public void WaitForRequest()
        {
            byte[] buffer = new byte[this.clientSocket.ReceiveBufferSize];
            this.networkStream.BeginRead(buffer, 0, buffer.Length, ReadCallback, buffer);
        }

        private void ReadCallback(IAsyncResult result)
        {
            NetworkStream networkStream = this.clientSocket.GetStream();

            int read = networkStream.EndRead(result);
            // End of the transmission, close everything.
            if(read == 0)
            {
                this.networkStream.Close();
                this.clientSocket.Close();
                return;
            }

            // Decode the content based on default encoding.
            byte[] buffer = result.AsyncState as byte[];
            string data = Encoding.Default.GetString(buffer, 0, read);

            // Remove unwanted newline characters.
            data = System.Text.RegularExpressions.Regex.Replace(data, @"\t|\n|\r", "");
            Console.WriteLine("Receive card ID: " + data);

            // Acquire the user info.
            UserInfo user = (parent as MainForm).GetUserInfo(data);
            if(user != null)
            {
                string name = user.FirstName + ' ' + user.LastName;

                // Perform the signup (cross thread invoke).
                // Since this is a cross thread call, need to invoke the function through MethodInvoker.
                //(this.parent as MainForm).Signup(name);
                (this.parent as MainForm).Invoke((MethodInvoker)(() => (this.parent as MainForm).Signup(name)));

                // Pack the information in JSON format.
                data = JsonConvert.SerializeObject(user);
            }
            else
            {
                data = string.Empty;
            }

            // Send the data back to the client.
            byte[] sendBytes = Encoding.ASCII.GetBytes(data);
            networkStream.Write(sendBytes, 0, sendBytes.Length);
            networkStream.Flush();

            // Continue pulling the data from the stream.
            WaitForRequest();
        }
    }
}
