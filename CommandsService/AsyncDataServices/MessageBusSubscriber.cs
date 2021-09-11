using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommandsService.EventProcessing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandsService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration configuration;
        private readonly IEventProcessing eventProcessing;
        private IConnection connection;
        private IModel channel;
        private string queueName;

        public MessageBusSubscriber(IConfiguration configuration, IEventProcessing eventProcessing)
        {
            this.configuration = configuration;
            this.eventProcessing = eventProcessing;
            this.InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory()
            {
                HostName = this.configuration["RabbitMQHost"],
                Port = int.Parse(this.configuration["RabbitMQPort"])
            };

            this.connection = factory.CreateConnection();
            this.channel = this.connection.CreateModel();
            this.channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            this.queueName = this.channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName,
                exchange: "trigger",
                routingKey: "");

            Console.WriteLine("--> Listenting on the Message Bus...");

            this.connection.ConnectionShutdown += RabbitMQ_ConnectionShitdown;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(this.channel);

            consumer.Received += (ModuleHandle, ea) =>
            {
                Console.WriteLine("--> Event Received");
                var body = ea.Body;
                var notifiedMessage = Encoding.UTF8.GetString(body.ToArray());

                this.eventProcessing.ProcessEvent(notifiedMessage);
            };

            this.channel.BasicConsume(queue: this.queueName, autoAck: true, consumer: consumer);
            Console.WriteLine("--> consumer closed");
            return Task.CompletedTask;
        }

        private void RabbitMQ_ConnectionShitdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> Connection Shutdown in subscriber");
        }

        public override void Dispose()
        {
            if (this.channel.IsOpen)
            {
                this.channel.Close();
                this.connection.Close();
            }

            base.Dispose();
        }
    }
}