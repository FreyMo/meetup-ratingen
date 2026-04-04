// This is only required because the IDE support did not make it to the preview 3.
// However, the compiler of preview 3 already supports union types
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct,
        AllowMultiple = false)]
    public sealed class UnionAttribute : Attribute;

    public interface IUnion
    {
        object? Value { get; }
    }
}
