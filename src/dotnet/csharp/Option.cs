namespace Sample;

public union Option<T>(T Some, None None)
{
    public void Report()
    {
        Console.WriteLine(Value.ToString());
    }
}

public enum None
{
    None
}

public static class OptionSample
{
    public static void Run()
    {
        var option1 = PossibleCat("meow");
        var option2 = PossibleCat("rawr");

        option1.Report();
        option2.Report();
    }

    private static Option<Cat> PossibleCat(string meow)
    {
        return meow == "meow" ? new Cat(meow) : None.None;
    }
}
