namespace Api;

public class TestRunWorkersOptions
{
    public static string Name = nameof(TestRunWorkersOptions);

    public int TestCapacity { get; set; }
    public TimeSpan Delay { get; set; }
}