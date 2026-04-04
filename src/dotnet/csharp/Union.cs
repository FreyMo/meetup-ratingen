namespace Sample;

public union Pet(Cat, Dog);

public record class Cat(string Name);
public record class Dog(string Name);

public static class UnionSample
{
    public static void Run()
    {
        Pet pet = new Cat("Whiskers");

        Console.WriteLine(pet switch
        {
            Cat c => $"Cat: {c.Name}",
            Dog d => $"Dog: {d.Name}",
        });
    }
}

