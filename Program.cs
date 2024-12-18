using System.Device.Gpio;
using Avans.StatisticalRobot;

Console.WriteLine("Connected!");

MQTTHandler.Instance.ReadMessage();
await Drive.StartAsync();