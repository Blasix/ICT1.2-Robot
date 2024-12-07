using Avans.StatisticalRobot;

class Drive
{
    // Add this class to hold shared state
    public class SharedState
    {
        public int DriveState { get; set; } = 0;
    }

    static public async Task StartAsync()
    {
        // Ultrasonic ultrasonic = new Ultrasonic(16);
        var state = new SharedState();

        using var cts = new CancellationTokenSource();

        Console.CancelKeyPress += (s, e) =>
        {
            cts.Cancel();
            e.Cancel = true;
        };

        try
        {
            var buttonTask = RunButtonMonitor(state, cts.Token);
            // var distanceTask = RunDistanceMonitor(ultrasonic, state, cts.Token);
            var driveTask = RunDriveState(state, cts.Token);

            await Task.WhenAll(buttonTask,
            // distanceTask, 
            driveTask);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Drive cancelled");
        }
        finally
        {
            Robot.Motors(0, 0);
        }
    }

    private static async Task RunButtonMonitor(SharedState state, CancellationToken ct)
    {
        Button button = new Button(6);
        bool LastButtonState = false;
        while (!ct.IsCancellationRequested)
        {
            bool currentButtonState = button.GetState() == "Pressed";
            if (currentButtonState && LastButtonState)
            {
                if (state.DriveState < 3)
                    state.DriveState++;
                else
                    state.DriveState = 0;
            }
            LastButtonState = currentButtonState;
            await Task.Delay(2);
        }
    }

    private static async Task RunDriveState(SharedState state, CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {

            switch (state.DriveState)
            {
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
            await Task.Delay(100);
        }
    }
}