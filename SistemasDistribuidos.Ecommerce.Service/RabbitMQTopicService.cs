using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace SistemasDistribuidos.Ecommerce.Service
{
    public class RabbitMQTopicService
    {
        private readonly IConfiguration _configuration;

        public RabbitMQTopicService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task Publish(string topicNameConfig, string message)
        {
            var topicName = _configuration[topicNameConfig];

            if (string.IsNullOrEmpty(topicName))
                throw new Exception($"{topicNameConfig} not found in appsettings.json");

            var factory = new ConnectionFactory { HostName = "localhost" };

            using var connection = await factory.CreateConnectionAsync();

            using var channel = await connection.CreateChannelAsync();

            await  channel.ExchangeDeclareAsync(exchange: topicName, type: ExchangeType.Topic);

            var body = Encoding.UTF8.GetBytes(message);

            await channel.BasicPublishAsync(exchange: topicName,
                                    routingKey: string.Empty,
                                    body: body);
        }

        public async Task CreateListener(string topicNameConfig, Func<object, BasicDeliverEventArgs,  Task> callback)
        {
            await Task.Run(async () =>
            {
                var topicName = _configuration[topicNameConfig];

                if (string.IsNullOrEmpty(topicName))
                    throw new Exception($"{topicNameConfig} not found in appsettings.json");

                var factory = new ConnectionFactory { HostName = "localhost" };

                using var connection = await factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();

                await channel.ExchangeDeclareAsync(exchange: topicName, type: ExchangeType.Topic);

                QueueDeclareOk queueDeclareResult = await channel.QueueDeclareAsync();

                string queueName = queueDeclareResult.QueueName;

                await channel.QueueBindAsync(queue: queueName, exchange: topicName, routingKey: string.Empty);

                var consumer = new AsyncEventingBasicConsumer(channel);

                consumer.ReceivedAsync += (model, ea) =>
                {
                    return callback(model, ea);
                };

                await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer);
            });
        }
    }
}
