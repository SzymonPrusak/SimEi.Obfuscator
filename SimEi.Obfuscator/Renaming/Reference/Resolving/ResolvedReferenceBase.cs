
namespace SimEi.Obfuscator.Renaming.Reference.Resolving
{
    internal abstract class ResolvedReferenceBase<T> : IResolvedReference<T>
    {
        private T? _cache;


        public T GetResolved()
        {
            return _cache ??= Resolve();
        }

        protected abstract T Resolve();
    }
}
