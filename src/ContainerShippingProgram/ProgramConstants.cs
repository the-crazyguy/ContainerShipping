using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerShippingProgram
{
    public static class ProgramConstants
    {
        public const decimal KgFee = 0.91m;
        public const decimal M3Fee = 19.37m;
        public const decimal FixedFee = 1692.72m;
        public const decimal ExpediencyFeePercent = 0.08m;

        public const int MinIdValue = 1000;

        public const decimal MinContainerContentsWeight = 0m;
        public const decimal MaxContainerContentsWeight = 20000m;

        public const decimal MinContainerContentsVolume = 0m;
        public const decimal MaxContainerContentsVolume = 40m;
    }
}
