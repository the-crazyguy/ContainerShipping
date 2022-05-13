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
            //TODO: Print/generate report


            if (e.IsRequestedByUser)
            {
                //Inform for successful disconnection
            }
            else
            {
                //Inform for unsuccessful disconnection
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
                    server.Dispose();
                    return;
                }

                // Accept client
                // TODO: Create a sub-thread per client
                server.AcceptClient();

                //Send welcome message
                if (server.IsClientConnected)
                {
                    server.WriteLine(ProtocolMessages.Welcome);
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
                    // Saving is handled outside the switch-case but is still a valid state - kept here in case of a future fall-through
                    break;
                
                default:
                    throw new InvalidContainerBuildingStateException();
            }

            // Note: If statement outside the switch case to avoid using a goto statement for saving
            // Otherwise, the user will be prompted to enter a string to get back into the state machine to execute the saving
            if (containerBuildingState == ContainerBuildingState.SaveContainer)
            {
                SaveCurrentContainer();
                server.WriteLine(ProtocolMessages.Acknowledge);
                //TODO: Determine whether to end the connection or keep reading via the view - let the user set up the server
                //containerBuildingState = ContainerBuildingState.WaitingForStart;
                DisconnectionRequested?.Invoke(this, new DisconnectEventArgs(isRequestedByUser: true));
            }
            #endregion
        }

        private void SaveCurrentContainer()
        {
            if (currentContainer != null)
            {
                containers.Add(currentContainer);
            }
        }

        /// <summary>
        /// Generates a full report for the containers asynchronously
        /// </summary>
        /// <returns>A string containing the rerport</returns>
        public async Task<string> GetFullReportAsync()
        {
            #region Initialization
            int columnsCount = 5;
            const int columnWidth = 15;

            string fullLineSeparator = new string('_', columnsCount * columnWidth + 2);
            string tableTitleLine = string.Format($"{"Id",-columnWidth}{ "Weight",-columnWidth }{"Volume",-columnWidth}{"Refrid",-columnWidth} {"Fee",-columnWidth}");
            string currentLine;
            
            List<string> output = new List<string>();
            output.Add(fullLineSeparator);
            output.Add(tableTitleLine);
            output.Add(fullLineSeparator);

            List<FullContainer> fullContainers = new List<FullContainer>();
            List<HalfContainer> halfContainers = new List<HalfContainer>();
            List<QuartContainer> quartContainers = new List<QuartContainer>();
            #endregion

            #region Separate containers by type
            foreach (BaseContainer container in containers)
            {
                if (container is FullContainer)
                {
                    fullContainers.Add(container as FullContainer);
                }
                else if (container is HalfContainer)
                {
                    halfContainers.Add(container as HalfContainer);
                }
                else if (container is QuartContainer)
                {
                    quartContainers.Add(container as QuartContainer);
                }
                else
                {
                    // Do nothing
                }
            }
            #endregion

            #region Full Size
            output.Add("Full size");
            decimal fullContainersTotalFees = 0m;
            foreach (FullContainer container in fullContainers)
            {
                decimal currentContainerFees = container.CalculateFees();
                fullContainersTotalFees += currentContainerFees;

                currentLine = string.Format($"{container.Id,-columnWidth}{container.Weight.ToString() + "kg",-columnWidth }{string.Empty,-columnWidth}{(container.IsRefridgerated ? "Y" : "N"),-columnWidth}{currentContainerFees,-columnWidth:C2}");
                output.Add(currentLine);
            }

            //Add total:
            currentLine = string.Format($"{string.Empty,-(columnWidth * 3)}{"Total:",-columnWidth}{fullContainersTotalFees,-columnWidth:C2}");
            output.Add(currentLine);
            #endregion

            #region Half Size
            output.Add("Half Size");
            decimal halfContainersTotalFees = 0m;
            foreach (HalfContainer container in halfContainers)
            {
                decimal currentContainerFees = container.CalculateFees();
                halfContainersTotalFees += currentContainerFees;
                
                currentLine = string.Format($"{container.Id,-columnWidth}{string.Empty,-columnWidth}{container.Volume.ToString() + "m3",-columnWidth}{"N/A",-columnWidth}{currentContainerFees,-columnWidth:C2}");
                output.Add(currentLine);
            }

            //Add total:
            currentLine = string.Format($"{string.Empty,-(columnWidth * 3)}{"Total:",-columnWidth}{halfContainersTotalFees,-columnWidth:C2}");
            output.Add(currentLine);
            #endregion

            #region Quart Size
            output.Add("Quart Size");
            decimal quartContainersTotalFees = 0m;
            foreach (QuartContainer container in quartContainers)
            {
                decimal currentContainerFees = container.CalculateFees();
                quartContainersTotalFees += currentContainerFees;

                currentLine = string.Format($"{container.Id,-columnWidth}{string.Empty,-columnWidth }{string.Empty,-columnWidth}{"N/A",-columnWidth}{currentContainerFees,-columnWidth:C2}");
                output.Add(currentLine);
            }

            //Add total:
            currentLine = string.Format($"{string.Empty,-(columnWidth * 3)}{"Total:",-columnWidth}{quartContainersTotalFees,-columnWidth:C2}");
            output.Add(currentLine);
            #endregion

            #region Grand total
            decimal totalContainerFees = fullContainersTotalFees + halfContainersTotalFees + quartContainersTotalFees;
            currentLine = string.Format($"{string.Empty,-(columnWidth * 3)}{"Grand Total:",-columnWidth}{totalContainerFees,-columnWidth:C2}");
            output.Add(currentLine);
            #endregion

            //Convert the result to a single string
            string result = string.Join('\n', output);
            return result;
        }

    }
}
