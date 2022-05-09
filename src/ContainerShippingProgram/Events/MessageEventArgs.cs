using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerShippingProgram.Events
{
    /// <summary>
    /// EventArgs for command events to provide the appropriate message
    /// </summary>
    internal class MessageEventArgs : EventArgs
    {
        /// <summary>
        /// The command for the event
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public MessageEventArgs() : this(string.Empty)
        {
        }

        /// <summary>
        /// Constructor which directly provides a command to send
        /// </summary>
        /// <param name="message">The command to send</param>
        public MessageEventArgs(string message)
        {
            Message = message;
        }
    }
}
