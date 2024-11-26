using Avans.StatisticalRobot;

class Drive
{
    static public void start()
    {
        Button button = new Button(6);
        int driveState = 0;
        bool lastButtonState = false;

        while (true)
        {
            bool currentButtonState = button.GetState() == "Pressed";

            // Checken of de knop nu word ingedrukt (en de vorige keer niet), zodat er geen dubbelen triggers zijn
            if (currentButtonState && !lastButtonState)
            {
                if (driveState < 3)
                {
                    driveState++;
                }
                else
                {
                    driveState = 0;
                }
                switch (driveState)
                {
                    // case 0:
                    //     Robot.Motors(0, 0);
                    //     break;
                    case 1:
                        Robot.Motors(30, 30);
                        break;
                    case 2:
                        Robot.Motors(60, 60);
                        break;
                    case 3:
                        Robot.Motors(120, 120);
                        break;
                    default:
                        Robot.Motors(0, 0);
                        break;
                }
            }
            Robot.Wait(1);
            lastButtonState = currentButtonState;  // Update de button state
            Robot.Wait(1);
        }
    }
}