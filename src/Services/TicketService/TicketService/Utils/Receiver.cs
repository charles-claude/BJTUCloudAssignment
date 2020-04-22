using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace TicketService.Utils
{
    public class Receiver
    {
        public static string Receive(string queue)
    {
        using (IConnection connection = new ConnectionFactory().CreateConnection())
        {
            using (IModel channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue, false, false, false, null);
                var consumer = new EventingBasicConsumer(channel);
                BasicGetResult result = channel.BasicGet(queue, true);
                if (result != null)
                {
                    string data = Encoding.UTF8.GetString(result.Body.ToArray());
                        return (data);
                }
            }
        }
            return (null);
    }
    }
}
