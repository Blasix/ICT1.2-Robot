using System.Device.Gpio;
using Avans.StatisticalRobot;

Console.WriteLine("Connected!");

MQTTHandler mqttHandler = new MQTTHandler();
mqttHandler.ReadMessage();
await Drive.StartAsync();