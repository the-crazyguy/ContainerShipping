using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerShippingProgram.Exceptions
{
    /// <summary>
    /// Represents errors relating to the container shipping logic
    /// </summary>
    [Serializable]
    public class ContainerException : Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ContainerException() : base("Invalid container data") { }
        /// <summary>
        /// Constructor with an error message provided
        /// </summary>
        /// <param name="message">The custom error message</param>
        public ContainerException(string message) : base(message) { }
        /// <summary>
        /// Constructor with an error message and an inner exception
        /// </summary>
        /// <param name="message">The custom error message</param>
        /// <param name="inner">The inner exception, which caused the throwing on this one</param>
        public ContainerException(string message, Exception inner) : base(message, inner) { }
        protected ContainerException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
