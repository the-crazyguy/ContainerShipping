using ContainerShippingProgram;
using ContainerShippingProgram.ContainerEntities;
using NUnit.Framework;
using ContainerShippingProgram.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json.Linq;
using ContainerShippingTestProject.TestClasses;

namespace ContainerShippingTestProject
{
    public class ContainerEntitiesTests
    {
        [SetUp]
        public void Setup()
        {
        }

        #region Container Creation
        [Test]
        public void CreateContainer_ProvideInvalidId_ThrowsInvalidIdException()
        {
            // Arrange
            int id = ProgramConstants.MinIdValue - 1;
            string description = "Some description";
            string originCountry = "Country of origin";
            string expectedExceptionMessage = $"Invalid ID: {id}";

            // Act

            // Assert
            Assert.Throws(Is.TypeOf<InvalidIdException>()
                         .And.Message.EqualTo(expectedExceptionMessage),
                         () => { _ = new ContainerTestWrapper(id, description, originCountry); });
        }

        [Test]
        public void CreateHalfContainer_ProvideTooLargeVolume_ThrowsInvalidVolumeException()
        {
            // Arrange
            int id = 1001;
            string description = "Some description";
            string originCountry = "Country of origin";
            decimal volume = ProgramConstants.MaxContainerContentsVolume + 1;
            string expectedExceptionMessage = $"Volume cannot exceed {ProgramConstants.MaxContainerContentsVolume}m3";

            // Act

            // Assert
            Assert.Throws(Is.TypeOf<InvalidContainerVolumeException>()
                         .And.Message.EqualTo(expectedExceptionMessage),
                         () => { _ = new HalfContainer(id, description, originCountry, volume); });
        }

        [Test]
        public void CreateHalfContainer_ProvideTooSmallVolume_ThrowsInvalidVolumeException()
        {
            // Arrange
            int id = 1001;
            string description = "Some description";
            string originCountry = "Country of origin";
            decimal volume = ProgramConstants.MinContainerContentsVolume - 1;
            string expectedExceptionMessage = "Volume cannot be negative";

            // Act

            // Assert
            Assert.Throws(Is.TypeOf<InvalidContainerVolumeException>()
                         .And.Message.EqualTo(expectedExceptionMessage),
                         () => { _ = new HalfContainer(id, description, originCountry, volume); });
        }


        [Test]
        public void CreateFullContainer_ProvideTooSmallWeight_ThrowsInvalidWeightException()
        {
            // Arrange
            int id = 1001;
            string description = "Some description";
            string originCountry = "Country of origin";
            decimal weight = ProgramConstants.MinContainerContentsWeight - 1;
            bool isRefridgerated = false;
            string expectectedExceptionMessage = "Weight cannot be negative";
            // Act

            // Assert
            Assert.Throws(Is.TypeOf<InvalidContainerWeightException>()
                         .And.Message.EqualTo(expectectedExceptionMessage),
                         () => { _ = new FullContainer(id, description, originCountry, weight, isRefridgerated); });
        }

        [Test]
        public void CreateFullContainer_ProvideTooLargeWeight_ThrowsInvalidWeightException()
        {
            // Arrange
            int id = 1001;
            string description = "Some description";
            string originCountry = "Country of origin";
            decimal weight = ProgramConstants.MaxContainerContentsWeight + 1;
            bool isRefridgerated = false;
            string expectectedExceptionMessage = $"Weight cannot be over {ProgramConstants.MaxContainerContentsWeight}kg";
            // Act

            // Assert
            Assert.Throws(Is.TypeOf<InvalidContainerWeightException>()
                         .And.Message.EqualTo(expectectedExceptionMessage),
                         () => { _ = new FullContainer(id, description, originCountry, weight, isRefridgerated); });
        }

        #endregion

        #region Calculate Fees
        [Test]
        public void CalculateFees_CreateQuartContainer_ReturnsFixedFee()
        {
            // Arrange
            int id = 1001;
            string description = "Some description";
            string originCountry = "Country of origin";
            decimal expectedFees = ProgramConstants.FixedFee;

            // Act
            QuartContainer container = new QuartContainer(id, description, originCountry);
            decimal actualFees = container.CalculateFees();

            // Assert
            Assert.AreEqual(expectedFees, actualFees);
        }

        [Test]
        public void CalculateFees_CreateHalfContainer_ReturnsCorrectFee()
        {
            // Arrange
            int id = 1001;
            string description = "Some description";
            string originCountry = "Country of origin";
            decimal volume = 10;
            decimal expectedFees = ProgramConstants.M3Fee * volume;

            // Act
            HalfContainer container = new HalfContainer(id, description, originCountry, volume);
            decimal actualFees = container.CalculateFees();

            // Assert
            Assert.AreEqual(expectedFees, actualFees);
        }

        [Test]
        public void CalculateFees_CreateFullContainerNotRefriderated_ReturnsCorrectFee()
        {
            // Arrange
            int id = 1001;
            string description = "Some description";
            string originCountry = "Country of origin";
            decimal weight = 1000;
            bool isRefridgerated = false;

            FullContainer container = new FullContainer(id, description, originCountry, weight, isRefridgerated);

            decimal expectedFees = ProgramConstants.KgFee * weight;

            // Act
            decimal actualFees = container.CalculateFees();

            // Assert
            Assert.AreEqual(expectedFees, actualFees);
        }

        [Test]
        public void CalculateFees_CreateFullContainerRefriderated_ReturnsCorrectFee()
        {
            // Arrange
            int id = 1001;
            string description = "Some description";
            string originCountry = "Country of origin";
            decimal weight = 1000;
            bool isRefridgerated = true;

            FullContainer container = new FullContainer(id, description, originCountry, weight, isRefridgerated);

            decimal expectedFees = ProgramConstants.KgFee * weight;
            expectedFees += expectedFees * ProgramConstants.ExpediencyFeePercent;

            // Act
            decimal actualFees = container.CalculateFees();

            // Assert
            Assert.AreEqual(expectedFees, actualFees);
        }

        #endregion

    }
}
