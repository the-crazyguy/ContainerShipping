using ContainerShippingProgram.ContainerEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerShippingTestProject.TestClasses
{
    class ContainerTestWrapper : BaseContainer
    {
        public ContainerTestWrapper(int id, string description, string originCountry) : base(id, description, originCountry)
        {

        }

        public override decimal CalculateFees()
        {
            throw new NotImplementedException();
        }
    }
}
