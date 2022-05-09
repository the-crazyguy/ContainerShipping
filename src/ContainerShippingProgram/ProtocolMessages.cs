using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerShippingProgram
{
    public static class ProtocolMessages
    {
        public const string Welcome = "WELCOME";
        public const string Start = "START";
        public const string Stop = "STOP";

        public const string FullType = "FULL";
        public const string HalfType = "HALF";
        public const string QuartType = "QUART";
        
        public const string Yes = "YES";
        public const string No = "NO";

        public const string Acknowledge = "ACK";

        public const string Error = "ERR";
        
    }
}
