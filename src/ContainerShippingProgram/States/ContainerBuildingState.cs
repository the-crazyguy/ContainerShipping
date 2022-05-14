﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerShippingProgram.States
{
    enum ContainerBuildingState
    {
        WaitingForStart,
        DetermineType,
        DetermineRefridgeration,
        DetermineWeight,
        DetermineVolume,
        SaveContainer
    }
}
