﻿using TechTalk.SpecFlow;
using Xunit;

namespace katarabbitmq.bdd.tests.Steps
{
    [Binding]
    public class DockerShutdownStepDefinitions
    {
        [When("a TERM signal is sent to both server and client")]
        public static void WhenATermSignalIsSent()
        {
            Processes.Robot.SendTermSignal();
            Processes.Client.SendTermSignal();
        }

        [Then("both applications shut down.")]
        public static void TheApplicationShutsDown()
        {
            Assert.True(Processes.Robot.HasExited);
            Assert.True(Processes.Client.HasExited);
        }
    }
}
