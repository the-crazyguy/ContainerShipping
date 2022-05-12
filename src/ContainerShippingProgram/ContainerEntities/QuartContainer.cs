using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerShippingProgram.ContainerEntities
{
    public class QuartContainer : BaseContainer
    {
        /// <summary>
        /// Default constructor for quart containers
        /// </summary>
        /// <param name="id">The unique id of the container</param>
        public QuartContainer(int id) : this(id, string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Constructor to create a quart container from a provided base container
        /// </summary>
        /// <param name="source">The base container from which to create a quart container</param>
        public QuartContainer(BaseContainer source) : this(source.Id, source.Description, source.OriginCountry)
        {
        }

        /// <summary>
        /// Detailed constructor for quart containers
        /// </summary>
        /// <param name="id">The unique id of the container</param>
        /// <param name="description">The description of the container</param>
        /// <param name="originCountry">The country of origin of the container</param>
        public QuartContainer(int id, string description, string originCountry) : base(id, description, originCountry)
        {
        }

        /// <summary>
        /// Calculates the fees for the container
        /// </summary>
        /// <returns>The total feels</returns>
        public override decimal CalculateFees()
        {
            return ProgramConstants.FixedFee;
        }
    }
}
