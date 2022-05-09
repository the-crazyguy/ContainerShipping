using ContainerShippingProgram.ContainerEntities;
using ContainerShippingProgram.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ContainerShippingProgram.Servers;

namespace ContainerShippingProgram.Controllers
{
    /// <summary>
    /// A class responsible for handling container information
    /// </summary>
    internal class ContainerController : IContainerController
    {
        private List<BaseContainer> containers;
        /// <summary>
        /// The saved containers
        /// </summary>
        public IReadOnlyCollection<BaseContainer> Containers => containers.AsReadOnly();

        private IServer server;

        private Thread serverThread;
        
        private CancellationTokenSource serverCts;
        private CancellationTokenSource commandCts;


        //Events
        /// <summary>
        /// Event for when an unrecognized command is received
        /// </summary>
        public event EventHandler<CommandEventArgs> UnrecognizedCommandReceived;
        /// <summary>
        /// Event for when a message that has to be displayed by the view is received
        /// </summary>
        public event EventHandler<CommandEventArgs> MessageToPrintReceived; //TODO: Rename, use another eventArgs?
        /// <summary>
        /// Event for when an empty command has been received
        /// </summary>
        public event EventHandler EmptyCommandReceived;
        /// <summary>
        /// Event for when the stop command has been received
        /// </summary>
        public event EventHandler StopCommandReceived;

        /// <summary>
        /// Default constructor for <see cref="ContainerController"/>
        /// </summary>
        public ContainerController()
        {
            containers = new List<BaseContainer>();
            server = new Server("0.0.0.0", 1234);
            
            serverCts = new CancellationTokenSource();

            //Initialize a separate thread to run the server on
            CreateServerThread();
            StartServerThread(serverThread);

            SubscribeToEvents();
        }

        //Note/Question: Is it necessary for private methods have documentation?
        /// <summary>
        /// Subscribe to all events that are required for the class to function properly
        /// </summary>
        private void SubscribeToEvents()
        {
            StopCommandReceived += StopCommandReceivedEventHandler;
        }

        /// <summary>
        /// Event handler for when a stop command is received
        /// </summary>
        private void StopCommandReceivedEventHandler(object sender, EventArgs e)
        {
            commandCts.Cancel();
        }

        /// <summary>
        /// Destructor to free up resources
        /// </summary>
        ~ContainerController()
        {
            //Destructor
            //TODO/Note: no need to close opened threads as they are background threads and close when the main thread closes
            //Stop running the server after the controller is disposed of
            StopServerThread(serverCts);
        }

        /// <summary>
        /// Creates a thread for the server to run on
        /// </summary>
        private void CreateServerThread()
        {
            serverThread = new Thread(() => RunServer(serverCts.Token));
            serverThread.IsBackground = true;
            serverThread.Name = "Server";
        }

        /// <summary>
        /// Starts the <see cref="Thread"/> provided
        /// </summary>
        /// <param name="threadToStart">The thread to start</param>
        private void StartServerThread(Thread threadToStart)
        {
            threadToStart.Start();
        }

        /// <summary>
        /// Reuest cancellation of the threads connected to the provided <see cref="CancellationTokenSource"/>
        /// </summary>
        /// <param name="serverCts">The cancellation token source whose tokens to cancel</param>
        private void StopServerThread(CancellationTokenSource serverCts)
        {
            serverCts.Cancel();
        }

        /// <summary>
        /// Server logic, ran on a separate thread from the main one for improved responsiveness
        /// </summary>
        /// <param name="serverToken">The cancellation token with which to stop the server</param>
        private void RunServer(CancellationToken serverToken)
        {
            //Make sure the server is started
            if (!server.IsRunning)
            {
                server.Start();
            }

            //TODO: Rename
            CommandEventArgs currentCommandEventArgs = new CommandEventArgs();


            //Infinite loop - keep server alive for the duration of the program
            while (true)
            {
                if (serverToken.IsCancellationRequested)
                {
                    //Stop the server
                    //TODO: Clean-up, if any
                    return;
                }

                //TODO: Take in commands
                //TODO: Remove - not in accordance to the protocol, here for testing purposes
                currentCommandEventArgs.Command = "Server ready";
                MessageToPrintReceived?.Invoke(this, currentCommandEventArgs);

                // Accept client
                server.AcceptClient();

                //TODO: If the client is active, send welcome message
                if (server.IsClientConnected)
                {
                    server.WriteLine(ProtocolMessages.Welcome);
                    currentCommandEventArgs.Command = ProtocolMessages.Welcome;
                    MessageToPrintReceived?.Invoke(this, currentCommandEventArgs);
                }


                // Create a new cancellation token source and dispose of the old one (if an old one exists)
                commandCts?.Dispose();
                commandCts = new CancellationTokenSource();
                CancellationToken commandCencellationToken = commandCts.Token;

                // Read data
                HandleReadingData(commandCencellationToken);

                // Client wants to disconnect
                server.DisconnectClient();
            }
        }

        /// <summary>
        /// Handles the reading of incoming data
        /// </summary>
        /// <param name="token">The cancellation token that with which to stop reading data</param>
        private void HandleReadingData(CancellationToken token)
        {
            do
            {
                string command = server.ReadLine().Trim();
                HandleCommand(command);
            } 
            while (!token.IsCancellationRequested);
        }

        /// <summary>
        /// Handles the provided command
        /// </summary>
        /// <param name="command">The command to handle</param>
        private void HandleCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                EmptyCommandReceived?.Invoke(this, EventArgs.Empty);
            }

            switch (command)
            {
                case ProtocolMessages.Stop:
                    server.WriteLine(ProtocolMessages.Acknowledge);
                    StopCommandReceived?.Invoke(this, EventArgs.Empty); //Event is raised in case the view needs to inform the user
                    break;
                
                default:
                    //Unrecognized command, raise an event
                    //Question/TODO: Should empty commands be treated as unrecognized commands to eliminate the need for the extra event?
                    UnrecognizedCommandReceived?.Invoke(this, new CommandEventArgs(command));
                    break;
            }
        }
    }
}
