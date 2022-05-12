using ContainerShippingProgram.ContainerEntities;
using ContainerShippingProgram.Events;
using System;
using System.Collections.Generic;

namespace ContainerShippingProgram.Controllers
{
    internal interface IContainerController
    {
        #region Properties
        
        /// <summary>
        /// The saved containers
        /// </summary>
        public IReadOnlyCollection<BaseContainer> Containers { get; }

        #endregion


        #region Events

        /// <summary>
        /// Event for when an unrecognized command is received
        /// </summary>
        public event EventHandler<MessageEventArgs> UnrecognizedCommandReceived;
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

        #endregion


        #region Methods

        #endregion
    }
}