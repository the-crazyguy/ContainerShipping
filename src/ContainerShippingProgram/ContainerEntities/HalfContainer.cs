using ContainerShippingProgram.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerShippingProgram.ContainerEntities
{
    public class HalfContainer : BaseContainer
    {
        //TODO: Do you need public sets for values or should everything be set upon creation?
        private decimal volume;
        /// <summary>
        /// The volume of the container's contents
        /// </summary>
        /// <exception cref="InvalidContainerVolumeException">Thrown when an invalid volume is provided</exception>
        public decimal Volume
        {
            get { return volume; }
            private set 
            {
                if (value < ProgramConstants.MinContainerContentsVolume)
                {
                    throw new InvalidContainerVolumeException("Volume cannot be negative");
                }
                if (value > ProgramConstants.MaxContainerContentsVolume)
                {
                    throw new InvalidContainerVolumeException($"Volume cannot exceed {ProgramConstants.MaxContainerContentsVolume}m3");
                }
                volume = value;
            }
        }

        /// <summary>
        /// Default constructor for half containers
        /// </summary>
        /// <param name="id">The unique id of the container</param>
        /// <param name="description">The description of the container</param>
        /// <param name="originCountry">The country of origin of the container</param>
        /// <param name="volume">The volume of the container's contents</param>
        public HalfContainer(int id, string description, string originCountry, decimal volume) : base(id, description, originCountry)
        {
            Volume = volume;
        }

        /// <summary>
        /// Calculates the fees for the container
        /// </summary>
        /// <returns>The total feels</returns>
        public override decimal CalculateFees()
        {
            return Volume * ProgramConstants.M3Fee;
        }
    }
}
