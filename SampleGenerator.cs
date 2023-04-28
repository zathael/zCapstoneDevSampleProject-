using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DevSample
{
    public class SampleGenerator
    {

        private readonly DateTime _sampleStartDate;
        private readonly TimeSpan _sampleIncrement;

        private readonly List<Sample> _sampleList;


        public SampleGenerator(DateTime sampleStartDate, TimeSpan sampleIncrement)
        {
            _sampleList = new List<Sample>();
            _sampleStartDate = sampleStartDate;
            _sampleIncrement = sampleIncrement;
        }


        /// <summary>
        /// Samples should be a time-descending ordered list
        /// </summary>
        public List<Sample> Samples { get { return _sampleList; } }


        public int SamplesValidated { get; private set; }


        public void LoadSamples(int samplesToGenerate)
        {

            //TODO: can we load samples faster?
            //TODO RESPONSE: 
            /* Yes, through Parallelism! This will split this out among threads via Parallel.For and lets us grab it through a Concurrent(collection).
             *
             *      We have to be careful to not just take the date based off of only the _sampleIncrement so that items are loaded at the Actual Time they're loaded
             * 
             */

            _sampleList.Clear();

            DateTime date = _sampleStartDate;

            // Use a ConcurrentBag to store unordered list of samples
            ConcurrentBag<Sample> samples = new ConcurrentBag<Sample>();

            Parallel.For(0, samplesToGenerate, i =>
            {
                DateTime currentDate = date + TimeSpan.FromTicks(_sampleIncrement.Ticks * i); // update time. Add the tick offset indepedently of the other samples using sampleIncrement.
                Sample s = new Sample(i == 0);
                s.LoadSampleAtTime(currentDate); // we are assuming this will always be thread safe. its current implementation does not access shared resources.
                samples.Add(s);
            });

            // Add the samples from ConcurrentBag to the _sampleList in the correct order
            _sampleList.AddRange(samples.OrderByDescending(s => s.Timestamp));

        }


        public void ValidateSamples()
        {

            //TODO: can we validate samples faster?
            //TODO RESPONSE: 
            /* Yes, through Parallelism! This will split this out among threads via Parallel.For, and then sum the valid returns.
             * This can be micro-optimized further by cache-ing lookup values, but you have to test these. 
             *  I tried to cache _sampleList.Count but that increased the validation time on my machine.
             */


            int samplesValidated = 0;
            object lockObject = new object(); // A lock to ensure thread-safe increments later

            Parallel.For(0, _sampleList.Count, i =>
            {
                try // ValidateSample only returns True or throws an exception, so we cover the exception this way.
                {
                    bool isValid = _sampleList[i].ValidateSample(
                        i < _sampleList.Count - 1 ? _sampleList[i + 1] : null,
                        _sampleIncrement);

                    if (isValid)
                    {
                        // Use a lock to ensure thread-safe increments of the samplesValidated variable
                        lock (lockObject)
                        {
                            samplesValidated++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // We would probably want to log this exception.
                    Console.WriteLine($"Error validating sample {i}: {ex.Message}");
                    Program.LogMessage(ex.Message); // using updated logger
                    // no 'continue' needed. Parallel.For continues even when an exception occurs.
                }

            });

            SamplesValidated = samplesValidated;

        }
    }
}
