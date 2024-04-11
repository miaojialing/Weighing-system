using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Packets;
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

        private bool _isApplicationExit;

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
            if (_client == null || !_client.IsConnected)
            {
                return Task.FromResult(new MqttClientPublishResult(0, MqttClientPublishReasonCode.PayloadFormatInvalid, "MQTT服务尚未连接", new MqttUserProperty[0]));
            }

            try
            {
                var msg = new MqttApplicationMessageBuilder()
                .WithTopic(topic).WithPayload(payload).WithRetainFlag(retain).Build();
                return _client.PublishAsync(msg);
            }
            catch (Exception e)
            {
                return Task.FromResult(new MqttClientPublishResult(0, MqttClientPublishReasonCode.PayloadFormatInvalid, $"MQTT服务异常:{e.Message}", new MqttUserProperty[0]));
            }
        }

        public Task<MqttClientSubscribeResult> SubscribeAsync(string topic, Func<MqttApplicationMessageReceivedEventArgs, Task> handle)
        {
            try
            {
                if (!_client.IsConnected)
                {
                    return Task.FromResult(new MqttClientSubscribeResult(0, new MqttClientSubscribeResultItem[0],
                        $"已断开 topic:{topic}", new MqttUserProperty[0]));
                }
                _client.ApplicationMessageReceivedAsync -= handle;
                
                var options = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(topic).Build();

                _client.ApplicationMessageReceivedAsync += handle;

                return _client.SubscribeAsync(options);
            }
            catch (Exception e)
            {
                return Task.FromResult(new MqttClientSubscribeResult(0, new MqttClientSubscribeResultItem[0],
                    $"MQTT服务异常:{e.Message}", new MqttUserProperty[0]));
            }
            
        }


        public async Task ConnectAsync(string server, int? port)
        {
            try
            {
                _client.ConnectedAsync += OnClientConnectedAsync;
                _client.DisconnectedAsync += OnClientDisconnectedAsync;

                var options = new MqttClientOptionsBuilder().WithTcpServer(server, port).WithTimeout(TimeSpan.FromSeconds(5)).Build();

                await Task.Factory.StartNew(async () =>
                {
                    var errCount = 0;
                    while (!_isApplicationExit)
                    {
                        try
                        {
                            // This code will also do the very first connect! So no call to _ConnectAsync_ is required in the first place.
                            if (!await _client.TryPingAsync())
                            {
                                var ret = await _client.ConnectAsync(options, CancellationToken.None);
                                if (ret.ResultCode == MqttClientConnectResultCode.Success)
                                {
                                    _svcChangedEvent.Publish(true);
                                }
                            }
                        }
                        catch
                        {
                            errCount++;
                            _svcChangedEvent.Publish(false);
                            if (errCount > 3)
                            {
                                break;
                            }
                        }
                        finally
                        {
                            // Check the connection state every 30 seconds and perform a reconnect if required.
                            await Task.Delay(TimeSpan.FromSeconds(10));
                        }
                    }
                }, TaskCreationOptions.LongRunning);

            }
            catch (Exception)
            {
            }

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

        public Task<bool> CloseAsync(bool applicationExit)
        {
            _isApplicationExit = applicationExit;
            return _client.TryDisconnectAsync();
        }

    }
}
