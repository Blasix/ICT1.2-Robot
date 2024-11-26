using Avans.StatisticalRobot;

class ButtonLight
{
    static public void start()
    {
        Led led = new Led(5);
        Button button = new Button(6);
        bool lightState = false;
        bool lastButtonState = false;

        led.SetOff();

        while (true)
        {
            bool currentButtonState = button.GetState() == "Pressed";

            // Checken of de knop nu word ingedrukt (en de vorige keer niet), zodat er geen dubbelen triggers zijn
            if (currentButtonState && !lastButtonState)
            {
                Console.WriteLine("Button pressed!");
                // Verander de status van het lampje
                lightState = !lightState;
                if (lightState)
                    led.SetOn();
                else
                    led.SetOff();
            }

            lastButtonState = currentButtonState;  // Update de button state
        }
    }
}