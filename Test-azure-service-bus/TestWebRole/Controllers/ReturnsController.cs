﻿using System;
using System.Web.Mvc;
using Microsoft.ServiceBus.Messaging;
using TestWebRole.Connector;
using TestWebRole.Models;
using Newtonsoft.Json;

namespace TestWebRole.Controllers
{
    public class ReturnsController : Controller
    {
        private readonly CustomQueueConnector _connector;
        public ReturnsController()
        {
            _connector = new CustomQueueConnector("ReturnsQueue", "ReturnCreatedTopic","ReturnCreatedSub");
            _connector.Initialize();
        }

        public ActionResult Send()
        {
            try
            {
                var namespaceManager = _connector.CreateNamespaceManager();

                var queue = namespaceManager.GetQueue(_connector.QueueName);
                var sub = namespaceManager.GetSubscription(_connector.TopicName, _connector.SubName);
                ViewBag.MessageCount = queue.MessageCount + "... And Topic(ReturnCreatedTopic) -> Sub (ReturnCreatedSub) Count: " + sub.MessageCountDetails.ActiveMessageCount;
                ViewBag.QueueName = _connector.QueueName;
            }
            catch (Exception e)
            {
                ViewBag.Error = "Error connecting to the Service Bus. Error : " + e.Message;
            }
            return View();
        }

        [HttpPost]
        public ActionResult Send(ReturnCreated msg, bool isTopic)
        {
            if (!ModelState.IsValid)
                return View(msg);

            msg.Timestamp = DateTime.Now;

            if (isTopic)
            {
                try
                {
                    var client = _connector.TopicClient;
                    var message = new BrokeredMessage(msg);
                    message.Properties["msgType"] = message.GetType().AssemblyQualifiedName;
                    client.Send(message);
                    TempData["Result"] = "Message sent successfully. (" + message.MessageId + ")";
                }
                catch (Exception e)
                {
                    TempData["Result"] = "Error: " + e.Message;
                }
            }
            else
            {
                var jsonMsg = JsonConvert.SerializeObject(msg, Formatting.Indented);
                var message = new BrokeredMessage(jsonMsg);

                try
                {
                    // Submit the custMessage.
                    _connector.MessagesQueueClient.Send(message);
                    TempData["Result"] = "Message sent successfully. (" + message.MessageId + ")";
                    TempData["Out"] = jsonMsg;
                }
                catch (Exception e)
                {
                    TempData["Result"] = "Error: " + e.Message;
                }
            }
            return RedirectToAction("Send");
        }
    }
}