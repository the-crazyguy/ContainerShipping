using ContainerShippingProgram.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerShippingProgram.ContainerEntities
{
    /// <summary>
    /// A base class for all containers
    /// </summary>
    public class BaseContainer
    {
        private int id;
        /// <summary>
        /// Id of the container
        /// </summary>
        /// <exception cref="InvalidIdException">Thrown when an invalid Id is provided</exception>
        public int Id
        {
            get { return id; }
            private set
            {
                if (value < ProgramConstants.MinIdValue)
                {
                    throw new InvalidIdException($"Invalid ID: {value}");
                }
                id = value;
            }
        }
        /// <summary>
        /// The description of the container
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The country of origin of the container
        /// </summary>
        public string OriginCountry { get; set; }

        #region Public Constructors

        /// <summary>
        /// Basic constructor for containers
        /// </summary>
        /// <param name="id">The unique id of the container</param>
        public BaseContainer(int id) : this(id, string.Empty, string.Empty)
        {
        }
        
        /// <summary>
        /// Detailed constructor for containers
        /// </summary>
        /// <param name="id">The unique id of the container</param>
        /// <param name="description">The description of the container</param>
        /// <param name="originCountry">The country of origin of the container</param>
        public BaseContainer(int id, string description, string originCountry)
        {
            // Data validity is checked in the setters
            Id = id;
            Description = description;
            OriginCountry = originCountry;
        }
        #endregion

        /// <summary>
        /// Method to calculate the container's fees. Each sub-container type implements its own fee calculation logic
        /// </summary>
        /// <returns>The total fees for the container</returns>
        public virtual decimal CalculateFees()
        {
            return ProgramConstants.FixedFee;
        }
    }
}
