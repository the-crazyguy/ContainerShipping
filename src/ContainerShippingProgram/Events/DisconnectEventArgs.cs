using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerShippingProgram.Events
{
    internal class DisconnectEventArgs : EventArgs
    {
        /// <summary>
        /// Determines whether the user requested the disconnection
        /// </summary>
        public bool IsRequestedByUser { get; set; }

        /// <summary>
        /// Default constructor, implies the user requested the disconnection
        /// </summary>
        public DisconnectEventArgs() : this(isRequestedByUser: true)
        {
        }

        /// <summary>
        /// Constructor determining whether the disconnection is requested by the user
        /// </summary>
        /// <param name="isRequestedByUser">Determining whether the disconnection is requested by the user</param>
        public DisconnectEventArgs(bool isRequestedByUser)
        {
            IsRequestedByUser = isRequestedByUser;
        }
    }
}
