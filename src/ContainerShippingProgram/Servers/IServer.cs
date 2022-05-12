using System;
using System.Net.Sockets;

namespace ContainerShippingProgram.Servers
{
    public interface IServer : IDisposable
    {
        #region Properties
        /// <summary>
        /// Determines whether the server is running or not
        /// </summary>
        public bool IsRunning { get; }
        /// <summary>
        /// Determines whether the client is connected or not
        /// </summary>
        public bool IsClientConnected { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Start the server
        /// </summary>
        void Start();
        /// <summary>
        /// Stop the server
        /// </summary>
        void Stop();
        /// <summary>
        /// Accept the next client waiting to connect
        /// </summary>
        void AcceptClient();
        /// <summary>
        /// Disconnects the current client
        /// </summary>
        void DisconnectClient();
        /// <summary>
        /// Reads a line from the server
        /// </summary>
        /// <returns>The line read</returns>
        string ReadLine();
        /// <summary>
        /// Writes a line to the server
        /// </summary>
        /// <param name="message">The message to write to the client</param>
        void WriteLine(string message);

        #endregion


    }
}