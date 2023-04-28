using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSample
{
    class Program // set to public so we can access LogMessage in other classes
    {

        static readonly int _cyclesToRun;
        static readonly int _samplesToLoad;
        static readonly DateTime _sampleStartDate;
        static readonly TimeSpan _sampleIncrement;

        static string logFilePath;

        static Program()
        {
            //Note: these settings should not be modified

            _cyclesToRun = Environment.ProcessorCount  > 1 ? Environment.ProcessorCount / 2 : 1; //hopefully we have more than 1 core to work with, run cores/2 cycles with a max of 4 cycles
            _cyclesToRun = _cyclesToRun > 4 ? 4 : _cyclesToRun;
            _samplesToLoad = 222222;
            _sampleStartDate = new DateTime(1990, 1, 1, 1, 1, 1, 1);
            _sampleIncrement = new TimeSpan(0, 5, 0);
        }

        static void Main(string[] args)
        {
            // Sets the unique text file path for the log LogToFile() based on the current time.
            logFilePath = Path.Combine("C:\\Temp", $"Log_{DateTime.Now:yyyyMMdd_HHmmss}.txt");


            Stopwatch totalMonitor = new Stopwatch();
            totalMonitor.Start();

            LogMessage($"Starting Execution on a {Environment.ProcessorCount} core system. A total of {_cyclesToRun} cycles will be run");

            TimeSpan totalElapseTimed = new TimeSpan();

            for (int i = 0; i < _cyclesToRun; i++)
            {
                try
                {
                    TimeSpan cycleElapsedTime = new TimeSpan();

                    Stopwatch cycleTimer = new Stopwatch();

                    SampleGenerator sampleGenerator = new SampleGenerator(_sampleStartDate, _sampleIncrement);

                    LogMessage($"Cycle {i} Started Sample Load.");


                    cycleTimer.Start();

                    sampleGenerator.LoadSamples(_samplesToLoad);

                    cycleTimer.Stop();
                    cycleElapsedTime = cycleTimer.Elapsed;
                    totalElapseTimed += cycleElapsedTime;
                    LogMessage($"Cycle {i} Finished Sample Load. Load Time: {cycleElapsedTime.TotalMilliseconds.ToString("N")} ms.");


                    LogMessage($"Cycle {i} Started Sample Validation.");


                    cycleTimer.Restart();

                    sampleGenerator.ValidateSamples();

                    cycleTimer.Stop();
                    cycleElapsedTime = cycleTimer.Elapsed;
                    totalElapseTimed += cycleElapsedTime;
                    LogMessage($"Cycle {i} Finished Sample Validation. Total Samples Validated: {sampleGenerator.SamplesValidated}. Validation Time: {cycleElapsedTime.TotalMilliseconds.ToString("N")} ms.");


                    decimal valueSum = 0;

                    foreach (Sample s in sampleGenerator.Samples)
                    {
                        valueSum += s.Value;
                    }

                    //TODO: why do we only seem to get 7 digits of precision? The CEO wants to see at least 20!
                    /* TODO RESPONSE: 
                     *      we original set valueSum to a 'float'. 
                     *          - Floats only have 6~9 digits of precision.
                     *          - Doubles only have 15~17 digits of precision
                     *          - Decimals have 28-29 digits of precision.
                     *          
                     *      Using Decimals will take up more space, but we are already adding s.Value together, which comprises of longs.
                     *      longs use 8 bytes as is, so maintaining a 16 byte decimal for their sum isn't a significant space loss
                     *      
                     *      I set it to decimal to give us at least 20 digits of precision, where available in the dataset
                     * 
                     */ 

                    LogMessage($"Cycle {i} Sum of All Samples: {valueSum.ToString("N")}.");

                    LogMessage($"Cycle {i} Finished. Total Cycle Time: {totalElapseTimed.TotalMilliseconds.ToString("N")} ms.");

                }
                catch(Exception ex)
                {
                    LogMessage($"Execution Failed!\n{ex.ToString()}");
                }

            }

            totalMonitor.Stop();

            LogMessage("-----");
            LogMessage($"Execution Finished. Total Elapsed Time: {totalMonitor.Elapsed.TotalMilliseconds.ToString("N")} ms.");


            Console.Read();

        }


        // changed to public so static access is available for exception logging elsewhere, in case we have a non-stopping exception (such as on a parallel thread)
        public static void LogMessage(string message)
        {
            LogToFile(message);
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fffff")} - {message}");
        }

        static void LogToFile(string message)
        {
            //TODO: implement this when someone complains about it not working... 
            /* TODO RESPONSE:
             * 
             *  Set up a StreamWriter to append a log when it is run. We set up a logFilePath in the Main() method so we have a static file we update each time we run this.
             *  Using AppendText lets us update the file as we run.
             * 
             */

            // Ensure the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));

            // Append the message to the log file
            using (StreamWriter writer = File.AppendText(logFilePath))
            {
                writer.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fffff")} - {message}");
            }

        }
    }
}
