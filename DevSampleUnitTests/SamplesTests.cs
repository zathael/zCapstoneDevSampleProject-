using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DevSample;
using System.Linq;

namespace SamplesTests
{
    [TestClass]
    public class SampleGeneratorTests
    {
        /// <summary>
        /// LoadSamplesTest
        /// Unit Test to ensure samples can be loaded with the appropriate count.
        /// </summary>
        [TestMethod]
        public void LoadSamplesTest()
        {
            // Arrange
            int samplesToGenerate = 10;
            var _sampleStartDate = new DateTime(1990, 1, 1, 1, 1, 1, 1);
            var _sampleIncrement = new TimeSpan(0, 5, 0);
            SampleGenerator sampleGenerator = new SampleGenerator(_sampleStartDate, _sampleIncrement);


            // Act
            sampleGenerator.LoadSamples(samplesToGenerate);

            // Assert
            Assert.AreEqual(samplesToGenerate, sampleGenerator.Samples.Count());
        }

        /// <summary>
        /// ValidateSamplesTest
        /// Integration Test to ensure samples can be validated after being loaded. 
        /// This can be reduced to a unit test if we use a faking library to simulate results or by manually generating samples here.
        /// </summary>
        [TestMethod]
        public void ValidateSamplesTest()
        {
            // Arrange
            int samplesToGenerate = 10;
            var _sampleStartDate = new DateTime(1990, 1, 1, 1, 1, 1, 1);
            var _sampleIncrement = new TimeSpan(0, 5, 0);
            SampleGenerator sampleGenerator = new SampleGenerator(_sampleStartDate, _sampleIncrement);
            sampleGenerator.LoadSamples(samplesToGenerate);

            // Act
            sampleGenerator.ValidateSamples();

            // Assert
            // Assuming ValidateSample() always returns true in this example
            Assert.AreEqual(samplesToGenerate, sampleGenerator.SamplesValidated);
        }
    }
}

