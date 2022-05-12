using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerShippingProgram.Exceptions
{
    [Serializable]
    public class ContainerProtocolException : Exception
    {
        public ContainerProtocolException() { }
        public ContainerProtocolException(string message) : base(message) { }
        public ContainerProtocolException(string message, Exception inner) : base(message, inner) { }
        protected ContainerProtocolException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
