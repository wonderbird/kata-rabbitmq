using System.Text;
using System.Threading;
using System.Threading.Tasks;
using katarabbitmq.infrastructure;
using katarabbitmq.model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace katarabbitmq.robot.app
{
    public class SensorDataSender : RabbitMqConnectedService
    {
        private readonly ILogger<SensorDataSender> _logger;

        public SensorDataSender(IRabbitMqConnection rabbit, ILogger<SensorDataSender> logger)
            : base(rabbit, logger) =>
            _logger = logger;

        protected override async Task ExecuteSensorLoopBody(CancellationToken stoppingToken)
        {
            await base.ExecuteSensorLoopBody(stoppingToken);

            if (!Rabbit.IsConnected)
            {
                return;
            }

            SendMeasurement();
        }

        private void SendMeasurement()
        {
            var measurement = new LightSensorValue { ambient = 7 };
            var message = JsonConvert.SerializeObject(measurement, Formatting.None);
            var body = Encoding.UTF8.GetBytes(message);

            Rabbit.Channel.BasicPublish("", "sensors", null, body);

            _logger.LogInformation($"Sent '{message}'");
        }
    }
}