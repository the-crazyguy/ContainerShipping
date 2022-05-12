using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerShippingProgram.Exceptions
{

    [Serializable]
    public class InvalidContainerTypeException : ContainerException
    {
        public InvalidContainerTypeException() : this("Invalid container type") { }
        public InvalidContainerTypeException(string message) : base(message) { }
        public InvalidContainerTypeException(string message, Exception inner) : base(message, inner) { }
        protected InvalidContainerTypeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
