using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using PlatformService.DTOs;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration config;
        private readonly IConnection connection;

        public IModel channel { get; }

        public MessageBusClient(IConfiguration config)
        {
            this.config = config;
            var factory = new ConnectionFactory() { HostName = this.config["RabbitMQHost"], Port = int.Parse(this.config["RabbitMQPort"]) };

            try
            {
                connection = factory.CreateConnection();
                channel = connection.CreateModel();
                channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
                connection.ConnectionShutdown += RabbitMQConnectionShutdown;
                Console.WriteLine($"Rabbit MQ Connected to Message Bus ");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Message client bus implementation - {ex.Message}");
            }
        }

        public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
            var message = JsonSerializer.Serialize(platformPublishedDto);
            if (connection.IsOpen)
            {
                Console.WriteLine($"Rabbit MQ Connection is OPEN -----> Sending Message");
                this.SendMessage(message);
            }
            else
            {
                Console.WriteLine($"Rabbit MQ Connection is Closed -----> NOT Sending Message");
            }
        }

        private void SendMessage(string message)
        {
            var messageBody = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "trigger", routingKey: "", basicProperties: null, body: messageBody);
            Console.WriteLine($"We have sent the message to bus {message}");
        }

        public void Dispose()
        {
            Console.WriteLine($"Message bus DISPOSED");
            if (channel.IsOpen)
            {
                channel.Close();
                connection.Close();
            }
        }
        private void RabbitMQConnectionShutdown(object sender, ShutdownEventArgs args)
        {
            Console.WriteLine($"Rabbit MQ Connection Closed {sender.ToString()}");
        }
    }
}