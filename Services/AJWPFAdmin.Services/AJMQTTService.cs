using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Services
{
    public class AJMQTTService
    {
        public static readonly string CARPASSEDTOPIC = "/carpassed";

        public class AJMQTTSvcStatusChangedEvent : PubSubEvent<bool>
        {

        }

        private static readonly MqttFactory _factory = new MqttFactory();
        private static readonly IMqttClient _client = _factory.CreateMqttClient();

        private AJMQTTSvcStatusChangedEvent _svcChangedEvent;

        private readonly IEventAggregator _eventAggregator;

        public AJMQTTService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _svcChangedEvent = eventAggregator.GetEvent<AJMQTTSvcStatusChangedEvent>();
        }

        public Task<MqttClientPublishResult> PublishAsync(string topic, string payload, bool retain = false)
        {
            var msg = new MqttApplicationMessageBuilder()
                .WithTopic(topic).WithPayload(payload).WithRetainFlag(retain).Build();
            return _client.PublishAsync(msg);
        }

        public Task<MqttClientSubscribeResult> SubscribeAsync(string topic, Func<MqttApplicationMessageReceivedEventArgs, Task> handle)
        {
            var options = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(topic).Build();
            _client.ApplicationMessageReceivedAsync += handle;
            return _client.SubscribeAsync(options);
        }


        public async Task ConnectAsync(string server, int? port)
        {
            _client.ConnectedAsync += OnClientConnectedAsync;
            _client.DisconnectedAsync += OnClientDisconnectedAsync;

            await _client.ConnectAsync(new MqttClientOptionsBuilder()
                .WithTcpServer(server, port).Build());
        }

        private async Task OnClientDisconnectedAsync(MqttClientDisconnectedEventArgs e)
        {
            if (e.ClientWasConnected)
            {
                // Use the current options as the new options.
                await _client.ConnectAsync(_client.Options);
            }
            _svcChangedEvent.Publish(false);
        }

        private Task OnClientConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            _svcChangedEvent.Publish(true);
            return Task.CompletedTask;
        }

        public Task<bool> CloseAsync()
        {
            return _client.TryDisconnectAsync();
        }

    }
}
