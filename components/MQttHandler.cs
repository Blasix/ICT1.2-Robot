using SimpleMqtt;

class MQTTHandler
{
    private readonly string name = "Robot";
    private readonly string topic = "commands";
    private readonly SimpleMqttClient mqttClient;

    public MQTTHandler()
    {
        mqttClient = SimpleMqttClient.CreateSimpleMqttClientForHiveMQ(name);
        mqttClient.SubscribeToTopic(topic);
    }

    public void ReadMessage()
    {
        mqttClient.OnMessageReceived += (sender, e) =>
        {

            Console.WriteLine($"Received: {e.Message} on {e.Topic}");

            SharedWorkingType.workingType = e.Message.ToLower() switch
            {
                "forward" => WorkingType.Forward,
                "backward" => WorkingType.Backward,
                "left" => WorkingType.Left,
                "right" => WorkingType.Right,
                "automatic" => WorkingType.Automatic,
                "stop" => WorkingType.Stop,
                _ => WorkingType.Stop
            };
        };
    }
}