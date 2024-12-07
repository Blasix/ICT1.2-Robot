using Avans.StatisticalRobot;

class Drive
{
    // public class SharedState
    // {
    //     public int DriveState { get; set; } = 0;
    // }

    static public async Task StartAsync()
    {
        // Ultrasonic ultrasonic = new Ultrasonic(16);
        // var state = new SharedState();

        using var cts = new CancellationTokenSource();

        Console.CancelKeyPress += (s, e) =>
        {
            cts.Cancel();
            e.Cancel = true;
        };

        try
        {
            // var buttonTask = RunButtonMonitor(state, cts.Token);
            // var distanceTask = RunDistanceMonitor(ultrasonic, state, cts.Token);
            var driveTask = RunDriveState(cts.Token);

            await Task.WhenAll(
            // buttonTask,
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

    // private static async Task RunButtonMonitor(SharedState state, CancellationToken ct)
    // {
    //     Button button = new Button(6);
    //     bool LastButtonState = false;
    //     while (!ct.IsCancellationRequested)
    //     {
    //         bool currentButtonState = button.GetState() == "Pressed";
    //         if (currentButtonState && !LastButtonState)
    //         {
    //             if (state.DriveState < 3)
    //                 state.DriveState++;
    //             else
    //                 state.DriveState = 0;
    //         }
    //         LastButtonState = currentButtonState;
    //         await Task.Delay(20);
    //     }
    // }

    private static async Task RunDriveState(CancellationToken ct)
    {
        WorkingType lastWorkingType = WorkingType.Stop;
        while (!ct.IsCancellationRequested)
        {
            if (lastWorkingType != SharedWorkingType.workingType)
            {
                switch (SharedWorkingType.workingType)
                {
                    case WorkingType.Forward:
                        Robot.Motors(120, 120);
                        break;
                    case WorkingType.Backward:
                        Robot.Motors(-120, -120);
                        break;
                    case WorkingType.Left:
                        Robot.Motors(-60, 60);
                        break;
                    case WorkingType.Right:
                        Robot.Motors(60, -60);
                        break;
                    case WorkingType.Stop:
                        Robot.Motors(0, 0);
                        break;
                    case WorkingType.Automatic:
                        // Robot.Motors(1, 1);
                        break;
                }
            }
            lastWorkingType = SharedWorkingType.workingType;
            await Task.Delay(100);
        }
    }
}