enum WorkingType
{
    Forward,
    Backward,
    Left,
    Right,
    Stop,
    Automatic,
}

class SharedWorkingType
{
    public static WorkingType workingType { get; set; } = WorkingType.Stop;
}