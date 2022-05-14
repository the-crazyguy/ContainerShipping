using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerShippingProgram.Exceptions
{

    [Serializable]
    public class UnrecognizedCommandException : ContainerProtocolException
    {
        public UnrecognizedCommandException() : this("Unrecognized command received") { }
        public UnrecognizedCommandException(string command) : base($"Unrecognized command received: {command}") { }
        public UnrecognizedCommandException(string command, Exception inner) : base($"Unrecognized command received: {command}", inner) { }
        protected UnrecognizedCommandException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
