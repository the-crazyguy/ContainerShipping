using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerShippingProgram.States
{
    enum ContainerBuildingState
    {
        //TODO: is there a better way to handle...?
        WaitingForStart,
        DetermineType,
        DetermineRefridgeration,
        DetermineWeight,
        DetermineVolume,
        SaveContainer
    }
}
