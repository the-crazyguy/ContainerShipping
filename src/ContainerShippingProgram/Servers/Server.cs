using ContainerShippingProgram.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace ContainerShippingProgram.Servers
{
    /// <summary>
    /// A server class that represents the server with which to communicate
    /// </summary>
    internal class Server : IServer, IDisposable
    {
        /// <summary>
        /// Determines whether the instance has been disposed to prevent redundant calls
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// Determines whether the server is running or not
        /// </summary>
        public bool IsRunning { get; private set; }
        /// <summary>
        /// Determines whether the client is connected or not
        /// </summary>
        public bool IsClientConnected => clientSocket.Connected;

        //TODO: Set an actual ip
        private IPAddress serverIpAddress;
        private int port;
        private IPEndPoint localEndPoint;
        
        //Sockets
        private Socket listener;
        private Socket clientSocket;

        //Streams
        private NetworkStream networkStream;
        private StreamReader streamReader;
        private StreamWriter streamWriter;


        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="ip">The ip of the server</param>
        /// <param name="port">The port of the server</param>
        public Server(string ip, int port) : this(ip, port, 10) { }
        
        /// <summary>
        /// Default constructor with a custom value for max clients that can be queued up
        /// </summary>
        /// <param name="ip">The ip of the server</param>
        /// <param name="port">The port of the server</param>
        /// <param name="maxClientsQueue">The maximum amount of clients that the server can hold on queue</param>
        public Server(string ip, int port, int maxClientsQueue)
        {
            isDisposed = false;
            
            //TODO: Try-catch block?
            serverIpAddress = IPAddress.Parse(ip);
            this.port = port;
            localEndPoint = new IPEndPoint(serverIpAddress, this.port);
            listener = new Socket(serverIpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(maxClientsQueue);


            Start();
        }

        /// <summary>
        /// Accept the next client waiting to connect
        /// </summary>
        public void AcceptClient()
        {
            clientSocket = listener.Accept();
            InitializeStreams(clientSocket);
        }

        /// <summary>
        /// Initializes all required streams
        /// </summary>
        /// <param name="clientSocket"></param>
        private void InitializeStreams(Socket clientSocket)
        {
            networkStream = new NetworkStream(clientSocket);
            streamReader = new StreamReader(networkStream);
            streamWriter = new StreamWriter(networkStream);
        }

        /// <summary>
        /// Disconnects the current client
        /// </summary>
        public void DisconnectClient()
        {
            clientSocket.Disconnect(true);   
            clientSocket = null;
            networkStream = null;
            streamReader = null;
            streamWriter = null;
        }

        /// <summary>
        /// Reads a line from the server
        /// </summary>
        /// <returns>The line read</returns>
        public string ReadLine()
        {
            if (!IsClientConnected)
            {
                //TODO: throw an exception?
                return string.Empty;
            }

            return streamReader.ReadLine();
        }

        /// <summary>
        /// Writes a line to the server
        /// </summary>
        /// <param name="message">The message to write to the client</param>
        public void WriteLine(string message)
        {
            if (!IsClientConnected)
            {
                //TODO: throw an exception?
                return;
            }

            streamWriter.WriteLine(message);
            streamWriter.Flush();
        }

        /// <summary>
        /// Stop the server
        /// </summary>
        public void Stop()
        {
            DisconnectClient();
            IsRunning = false;
        }

        /// <summary>
        /// Start the server
        /// </summary>
        public void Start()
        {
            IsRunning = true;
        }
        #region Dispose pattern implementation
        protected virtual void Dispose(bool manualDisposing)
        {
            if (!isDisposed)
            {
                if (manualDisposing)
                {
                    // Dispose managed state (managed objects)
                    listener.Disconnect(true);
                    listener.Dispose();

                    clientSocket.Disconnect(true);
                    clientSocket.Dispose();

                    streamReader.Dispose();
                    streamWriter.Dispose();
                    networkStream.Dispose();
                }

                // Free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                isDisposed = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Server()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(manualDisposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
