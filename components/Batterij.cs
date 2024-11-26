using Avans.StatisticalRobot;

class Batterij
{
    public static void Spanning()
    {
        while (true)
        {
            int mv = Robot.ReadBatteryMillivolts();
            Console.WriteLine(mv);
            if (mv > 3000)
            {
                Robot.LEDs(0, 0, 255);
            }
            else
            {
                Robot.LEDs(255, 0, 0);
            }
        }
    }
}