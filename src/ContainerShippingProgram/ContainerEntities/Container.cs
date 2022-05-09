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
    public abstract class Container
    {
        //TODO: Id auto-increments and has to be unique - check in controller
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
        public string Description { get; }
        /// <summary>
        /// The country of origin of the container
        /// </summary>
        public string OriginCountry { get; }

        #region Public Constructors
        /// <summary>
        /// Default constructor for containers
        /// </summary>
        /// <param name="id">The unique id of the container</param>
        /// <param name="description">The description of the container</param>
        /// <param name="originCountry">The country of origin of the container</param>
        public Container(int id, string description, string originCountry)
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
        public abstract decimal CalculateFees();
    }
}
