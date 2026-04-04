namespace Sample;

public union Result<T, E>(T Ok, E Error)
{
    public T Or(T other)
    {
        return this switch {
            T ok => ok,
            _ => other
        };
    }
}

public static class ResultSample
{
    public static void Run()
    {
        var result1 = FailableCat("meow");
        Result<Cat, CatError> result2 = FailableCat("rawr");
        Cat result3 = FailableCat("rawr").Or(new Cat("qtπ"));

        Console.WriteLine(result2.Value.ToString());
        Console.WriteLine(result3.Name);
    }

    private static Result<Cat, CatError> FailableCat(string meow)
    {
        return meow == "meow" ? new Cat(meow) : CatError.TooCute;
    }
}

public enum CatError
{
    TooCute,
    Lazy
}
