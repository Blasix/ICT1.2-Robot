using SimpleMqtt;

class MQTTHandler
{
    private readonly string name = "Robot";
    private readonly string topic = "commands";
    private readonly SimpleMqttClient mqttClient;

    private static MQTTHandler? instance;

    private static readonly object lockObject = new object();

    private MQTTHandler()
    {
        mqttClient = SimpleMqttClient.CreateSimpleMqttClientForHiveMQ(name);
        mqttClient.SubscribeToTopic(topic);
    }

    public static MQTTHandler Instance
    {
        get
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    instance ??= new MQTTHandler();
                }
            }
            return instance;
        }
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


    public void PublishStatus()
    {
        string message = SharedWorkingType.workingType switch
        {
            WorkingType.Stop => "Stopped|" + DateTime.Now,
            WorkingType.Automatic => "Automatic|" + DateTime.Now,
            _ => "Manual|" + DateTime.Now
        };
        mqttClient.PublishMessage(message, "status");
    }
}