using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerShippingProgram.Exceptions
{
    /// <summary>
    /// Represents errors relating to the weight of the containers
    /// </summary>
    [Serializable]
    public class InvalidContainerWeightException : ContainerException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public InvalidContainerWeightException() : base("Invalid weight of container contents") { }
        /// <summary>
        /// Constructor with an error message provided
        /// </summary>
        /// <param name="message">The custom error message</param>
        public InvalidContainerWeightException(string message) : base(message) { }
        /// <summary>
        /// Constructor with an error message and an inner exception
        /// </summary>
        /// <param name="message">The custom error message</param>
        /// <param name="inner">The inner exception, which caused the throwing on this one</param>
        public InvalidContainerWeightException(string message, Exception inner) : base(message, inner) { }
        protected InvalidContainerWeightException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
