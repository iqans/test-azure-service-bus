﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace ConsoleWorkerRole
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Endpoint=sb://iqanstest1.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=kACru3WDbVWYmD5GEFF81sLIsgi+eyR9fjeO6+NWYpY=";
            var queueName = "MessagesQueue";

            try
            {
                var client = QueueClient.CreateFromConnectionString(connectionString, queueName);

                client.OnMessage(message =>
                {
                    Console.WriteLine("Processing message:");
                    Console.WriteLine(string.Format("Message id: {0}", message.MessageId));
                    var msg = message.GetBody<string>();
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("Message Body: " + msg);
                    Console.ResetColor();
                    Console.WriteLine("Processed at " + DateTime.Now);
                    Console.WriteLine("-------------------------------------------");
                    message.Complete();
                });
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: "+ e.Message);
                Console.ResetColor();
            }
            Console.ReadLine();
        }
    }
}
