using ContainerShippingProgram.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerShippingProgram.ContainerEntities
{
    /// <summary>
    /// A class representing the full container type
    /// </summary>
    public class FullContainer : BaseContainer
    {
        //TODO: Do you need public sets for values or should everything be set upon creation?
        private decimal weight;
        /// <summary>
        /// The weight of the container's contents
        /// </summary>
        /// <exception cref="InvalidContainerWeightException">Thrown when an invalid weight is provided</exception>
        public decimal Weight
        {
            get { return weight; }
            set 
            {
                if (value < ProgramConstants.MinContainerContentsWeight)
                {
                    throw new InvalidContainerWeightException("Weight cannot be negative");
                }
                if (value > ProgramConstants.MaxContainerContentsWeight)
                {
                    throw new InvalidContainerWeightException($"Weight cannot be over {ProgramConstants.MaxContainerContentsWeight}kg");
                }
                weight = value;
            }
        }
        /// <summary>
        /// Determines whether the container is refridgerated or not
        /// </summary>
        public bool IsRefridgerated { get; set; }

        /// <summary>
        /// Basic constructor for full containers
        /// </summary>
        /// <param name="id">The unique id of the container</param>
        public FullContainer(int id) : this(id, string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Detailed constructor for a full container
        /// </summary>
        /// <param name="id">The unique id of the container</param>
        /// <param name="description">The description of the container</param>
        /// <param name="originCountry">The country of origin of the container</param>
        public FullContainer(int id, string description, string originCountry) : this (id, description, originCountry, 0m, false)
        {
        }

        /// <summary>
        /// Full parameter constructor for full containers
        /// </summary>
        /// <param name="id">The unique id of the container</param>
        /// <param name="description">The description of the container</param>
        /// <param name="originCountry">The country of origin of the container</param>
        /// <param name="weight">The weight of the container</param>
        /// <param name="isRefridgerated">Determines whether the container is refridgerated or not</param>
        public FullContainer(int id, string description, string originCountry, decimal weight, bool isRefridgerated) : base(id, description, originCountry)
        {
            Weight = weight;
            IsRefridgerated = isRefridgerated;
        }

        /// <summary>
        /// Calculates the fees for the container
        /// </summary>
        /// <returns>The total feels</returns>
        public override decimal CalculateFees()
        {
            decimal result = Weight * ProgramConstants.KgFee;

            if (IsRefridgerated)
            {
                result += result * ProgramConstants.ExpediencyFeePercent;
            }

            return result;
        }
    }
}
