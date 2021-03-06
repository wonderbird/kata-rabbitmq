using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using katarabbitmq.bdd.tests.Helpers;
using TechTalk.SpecFlow;
using Xunit;
using Xunit.Abstractions;

namespace katarabbitmq.bdd.tests.Steps
{
    [Binding]
    public class LightSensorReadingsStepDefinitions
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private int _countReceivedSensorReadings;

        public LightSensorReadingsStepDefinitions(ITestOutputHelper testOutputHelper) =>
            _testOutputHelper = testOutputHelper;

        [When("the robot and client app have been connected for (.*) seconds")]
        public async Task WhenTheRobotAndClientAppHasBeenConnectedForSeconds(double seconds)
        {
            await WaitUntilProcessesConnectedToRabbitMq(Processes.Robot, Processes.Client);

            await WaitForSeconds(seconds);

            ParseSensorDataFromClientProcess();
        }

        private static async Task WaitUntilProcessesConnectedToRabbitMq(params RemoteControlledProcess[] processes)
        {
            bool IsConnectionEstablished()
            {
                return processes.ToList().All(p => p.IsConnectionEstablished);
            }

            while (!IsConnectionEstablished())
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1.0));
            }
        }

        private async Task WaitForSeconds(double seconds)
        {
            var stopwatch = Stopwatch.StartNew();
            await Task.Delay(TimeSpan.FromSeconds(seconds));
            stopwatch.Stop();

            _testOutputHelper?.WriteLine($"Waited for {stopwatch.ElapsedMilliseconds / 1000.0} seconds");
        }

        private void ParseSensorDataFromClientProcess()
        {
            var output = Processes.Client.ReadOutput();
            var lines = output.Split('\n').ToList();
            _countReceivedSensorReadings = lines.Count(l => l.Contains("Sensor data"));
        }

        [Then("the client app received at least (.*) sensor values")]
        public void ThenTheClientAppReceivedAtLeastSensorValues(int expectedSensorValuesCount)
        {
            _testOutputHelper.WriteLine($"Received {_countReceivedSensorReadings} values");
            Assert.True(_countReceivedSensorReadings >= expectedSensorValuesCount,
                $"Client app must receive at least {expectedSensorValuesCount} sensor value(s). It actually received {_countReceivedSensorReadings} values");
        }
    }
}