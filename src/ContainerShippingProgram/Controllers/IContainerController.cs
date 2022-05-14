using ContainerShippingProgram.ContainerEntities;
using ContainerShippingProgram.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        /// <summary>
        /// Generates a full report for the containers
        /// </summary>
        /// <returns>A string containing the rerport</returns>
        string GetFullReport();
        /// <summary>
        /// Generates a full report for the containers asynchronously
        /// </summary>
        /// <returns>A string containing the rerport</returns>
        Task<string> GetFullReportAsync();
        #endregion
    }
}