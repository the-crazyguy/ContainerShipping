using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerShippingProgram.Exceptions
{

    [Serializable]
    public class InvalidContainerBuildingStateException : Exception
    {
        public InvalidContainerBuildingStateException() : this("Invalid container building state") { }
        public InvalidContainerBuildingStateException(string message) : base(message) { }
        public InvalidContainerBuildingStateException(string message, Exception inner) : base(message, inner) { }
        protected InvalidContainerBuildingStateException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
