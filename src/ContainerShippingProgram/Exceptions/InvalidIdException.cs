using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerShippingProgram.Exceptions
{
    /// <summary>
    /// Represents errors relating to the Id of the containers
    /// </summary>
    [Serializable]
    public class InvalidIdException : ContainerException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public InvalidIdException() : base("Invalid ID") { }
        /// <summary>
        /// Constructor with an error message provided
        /// </summary>
        /// <param name="message">The custom error message</param>
        public InvalidIdException(string message) : base(message) { }
        /// <summary>
        /// Constructor with an error message and an inner exception
        /// </summary>
        /// <param name="message">The custom error message</param>
        /// <param name="inner">The inner exception, which caused the throwing on this one</param>
        public InvalidIdException(string message, Exception inner) : base(message, inner) { }
        protected InvalidIdException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
