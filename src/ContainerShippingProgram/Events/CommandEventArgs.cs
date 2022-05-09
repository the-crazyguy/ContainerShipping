using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerShippingProgram.Events
{
    /// <summary>
    /// EventArgs for command events to provide the appropriate command
    /// </summary>
    internal class CommandEventArgs : EventArgs
    {
        /// <summary>
        /// The command for the event
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public CommandEventArgs() : this(string.Empty)
        {
        }

        /// <summary>
        /// Constructor which directly provides a command to send
        /// </summary>
        /// <param name="command">The command to send</param>
        public CommandEventArgs(string command)
        {
            Command = command;
        }
    }
}
