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
        public UnrecognizedCommandException(string message) : base(message) { }
        public UnrecognizedCommandException(string message, Exception inner) : base(message, inner) { }
        protected UnrecognizedCommandException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
