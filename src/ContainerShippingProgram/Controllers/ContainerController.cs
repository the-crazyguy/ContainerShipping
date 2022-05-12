using ContainerShippingProgram.ContainerEntities;
using ContainerShippingProgram.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ContainerShippingProgram.Servers;
using ContainerShippingProgram.States;
using ContainerShippingProgram.Exceptions;

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

        private ContainerBuildingState containerBuildingState;
        private int currentContainerId;
        private int GetNewContainerId() => currentContainerId++;
        private BaseContainer currentContainer = null;

        private IServer server;

        private Thread serverThread;
        
        private CancellationTokenSource serverCts;
        private CancellationTokenSource commandCts;


        //Events
        /// <summary>
        /// Event for when an unrecognized command is received
        /// </summary>
        public event EventHandler<MessageEventArgs> UnrecognizedCommandReceived;    // TODO: Use another eventArgs for the commands, or remove
        /// <summary>
        /// Event for when a message that has to be displayed by the view is received
        /// </summary>
        public event EventHandler<MessageEventArgs> MessageToPrintReceived;
        /// <summary>
        /// Event for when an empty command has been received
        /// </summary>
        public event EventHandler EmptyCommandReceived;
        /// <summary>
        /// Event for when the stop command has been received
        /// </summary>
        public event EventHandler<DisconnectEventArgs> DisconnectionRequested;

        /// <summary>
        /// Default constructor for <see cref="ContainerController"/>
        /// </summary>
        public ContainerController()
        {
            currentContainerId = ProgramConstants.MinIdValue;
            containerBuildingState = ContainerBuildingState.WaitingForStart;

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
            DisconnectionRequested += DisconnectionRequestedEventHandler;
        }

        /// <summary>
        /// Event handler for when a stop command is received
        /// </summary>
        private void DisconnectionRequestedEventHandler(object sender, DisconnectEventArgs e)
        {
            currentContainer = null;
            commandCts.Cancel();

            if (e.IsRequestedByUser)
            {
                //TODO: Print/generate report
            }
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
            // TODO: Implement IDisposable?
            server.Dispose();
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

            MessageEventArgs currentMessageEventArgs = new MessageEventArgs();


            //Infinite loop - keep server alive for the duration of the program
            while (true)
            {
                if (serverToken.IsCancellationRequested)
                {
                    //Stop the server
                    //TODO: Clean-up, if any
                    return;
                }

                //TODO: Remove - not in accordance to the protocol, here for testing purposes
                currentMessageEventArgs.Message = "Server ready";
                MessageToPrintReceived?.Invoke(this, currentMessageEventArgs);

                // Accept client
                // TODO: Create a sub-thread per client
                server.AcceptClient();

                //Send welcome message
                if (server.IsClientConnected)
                {
                    server.WriteLine(ProtocolMessages.Welcome);
                    currentMessageEventArgs.Message = ProtocolMessages.Welcome;
                    MessageToPrintReceived?.Invoke(this, currentMessageEventArgs);

                    containerBuildingState = ContainerBuildingState.WaitingForStart;
                }


                // Create a new cancellation token source and dispose of the old one (if an old one exists)
                commandCts?.Dispose();
                commandCts = new CancellationTokenSource();
                CancellationToken commandCencellationToken = commandCts.Token;

                // Read data
                HandleCommunication(commandCencellationToken);

                // Client wants to disconnect
                server.DisconnectClient();
            }
        }


        /// <summary>
        /// Handles the server-client communication
        /// </summary>
        /// <param name="token">The cancellation token with which to stop reading new data</param>
        private void HandleCommunication(CancellationToken token)
        {
            do
            {
                // NOTE: ReadLine() is blocking
                string command = server.ReadLine()?.Trim();

                try
                {
                    HandleCommand(command);
                }
                catch (ContainerProtocolException ex)
                {
                    server.WriteLine($"{ProtocolMessages.ErrorPrefix}{ex.Message}");
                }
                catch (ContainerException ex)
                {
                    server.WriteLine($"{ProtocolMessages.ErrorPrefix}{ex.Message}");
                }
                catch (InvalidContainerBuildingStateException ex)
                {
                    MessageToPrintReceived?.Invoke(this, new MessageEventArgs(ex.Message));
                    DisconnectionRequested?.Invoke(this, new DisconnectEventArgs(isRequestedByUser: false));
                }
                catch (Exception)
                {
                    //TODO: Handle all exception types
                }
            }
            while (!token.IsCancellationRequested);
        }


        /// <summary>
        /// Handles the provided command
        /// </summary>
        /// <param name="command">The command to handle</param>
        /// <exception cref="UnrecognizedCommandException">Thrown when an unrecognized command is received</exception>
        /// <exception cref="InvalidContainerTypeException">Thrown when an invalid container type is encountered</exception>
        /// <exception cref="InvalidContainerWeightException">Thrown when an invalid weight is encountered</exception>
        /// <exception cref="InvalidContainerVolumeException">Thrown when an invalid volume is encountered</exception>
        /// <exception cref="InvalidContainerBuildingStateException">Thrown when an invalid container building state is reached</exception>
        private void HandleCommand(string command)
        {
            #region Guard statements
            if (command == null)
            {
                //null means the server could not read anything, therefore the connection was (forced) broken
                DisconnectionRequested?.Invoke(this, new DisconnectEventArgs(isRequestedByUser: false));
                return;
            }

            // TODO: Remove if unnecessary
            if (string.IsNullOrWhiteSpace(command))
            {
                EmptyCommandReceived?.Invoke(this, EventArgs.Empty);
                return;
            }

            if (command == ProtocolMessages.Stop)
            {
                server.WriteLine(ProtocolMessages.Acknowledge);
                DisconnectionRequested?.Invoke(this, new DisconnectEventArgs(isRequestedByUser: true)); //Event is raised in case the view needs to react
                return;
            }
            #endregion

            #region Container building state machine
            switch (containerBuildingState)
            {
                case ContainerBuildingState.WaitingForStart:
                    if (command == ProtocolMessages.Start)
                    {
                        currentContainer = new BaseContainer(GetNewContainerId());
                        server.WriteLine(ProtocolMessages.Type);
                        //TODO: Description and origin country
                        // Go to the next state
                        containerBuildingState = ContainerBuildingState.DetermineType;
                    }
                    break;

                case ContainerBuildingState.DetermineType:
                    switch (command)
                    {
                        case ProtocolMessages.FullType:
                            currentContainer = new FullContainer(currentContainer);
                            server.WriteLine(ProtocolMessages.Refridgerated);
                            // Go to the next state
                            containerBuildingState = ContainerBuildingState.DetermineRefridgeration;
                            break;

                        case ProtocolMessages.HalfType:
                            currentContainer = new HalfContainer(currentContainer);
                            server.WriteLine(ProtocolMessages.Volume);
                            // Go to the next state
                            containerBuildingState = ContainerBuildingState.DetermineVolume;
                            break;

                        case ProtocolMessages.QuartType:
                            currentContainer = new QuartContainer(currentContainer);
                            // Go to the next state
                            containerBuildingState = ContainerBuildingState.SaveContainer;
                            break;

                        default:
                            throw new InvalidContainerTypeException();
                    }
                    break;

                case ContainerBuildingState.DetermineRefridgeration:
                    if (currentContainer is not FullContainer)
                    {
                        throw new InvalidContainerTypeException();
                    }

                    switch (command)
                    {
                        case ProtocolMessages.Yes:
                            (currentContainer as FullContainer).IsRefridgerated = true;
                            break;

                        case ProtocolMessages.No:
                            (currentContainer as FullContainer).IsRefridgerated = false;
                            break;

                        default:
                            //UnrecognizedCommandReceived?.Invoke(this, new MessageEventArgs(command));
                            throw new UnrecognizedCommandException(command);
                    }

                    server.WriteLine(ProtocolMessages.Weight);
                    // Go to the next state
                    containerBuildingState = ContainerBuildingState.DetermineWeight;
                    break;

                case ContainerBuildingState.DetermineWeight:
                    {
                        if (currentContainer is not FullContainer)
                        {
                            throw new InvalidContainerTypeException();
                        }

                        if (!decimal.TryParse(command, out decimal containerWeight))
                        {
                            throw new InvalidContainerWeightException();
                        }

                        (currentContainer as FullContainer).Weight = containerWeight;
                    }
                    // Go to the next state
                    containerBuildingState = ContainerBuildingState.SaveContainer;
                    break;
                
                case ContainerBuildingState.DetermineVolume:
                    {
                        if (currentContainer is not HalfContainer)
                        {
                            throw new InvalidContainerTypeException();
                        }

                        if (!decimal.TryParse(command, out decimal containerVolume))
                        {
                            throw new InvalidContainerVolumeException();
                        }

                        (currentContainer as HalfContainer).Volume = containerVolume;
                    }
                    // Go to the next state
                    containerBuildingState = ContainerBuildingState.SaveContainer;
                    break;
                
                case ContainerBuildingState.SaveContainer:
                    containers.Add(currentContainer);
                    currentContainer = null;
                    //TODO: Determine whether to end the connection or keep reading via the view - let the user set up the server
                    containerBuildingState = ContainerBuildingState.WaitingForStart;
                    break;
                
                default:
                    throw new InvalidContainerBuildingStateException();
            }
            #endregion

            // TODO: Unrecognized command handling
            //Unrecognized command, raise an event
            //Question/TODO: Should empty commands be treated as unrecognized commands to eliminate the need for the extra event?
            //UnrecognizedCommandReceived?.Invoke(this, new CommandEventArgs(command));
        }
    }
}
