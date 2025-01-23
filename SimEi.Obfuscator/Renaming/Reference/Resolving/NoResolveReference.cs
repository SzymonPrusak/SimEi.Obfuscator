
namespace SimEi.Obfuscator.Renaming.Reference.Resolving
{
    internal class NoResolveReference<T> : IResolvedReference<T>
    {
        private readonly T _original;

        public NoResolveReference(T original)
        {
            _original = original;
        }

        public T GetResolved() => _original;
    }
}
