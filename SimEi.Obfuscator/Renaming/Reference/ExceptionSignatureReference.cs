using AsmResolver.DotNet;
using AsmResolver.DotNet.Code.Cil;
using SimEi.Obfuscator.Renaming.Reference.Resolving;

namespace SimEi.Obfuscator.Renaming.Reference
{
    internal class ExceptionSignatureReference : ITrackedReference
    {
        private readonly CilExceptionHandler _handler;

        private readonly IResolvedReference<ITypeDefOrRef>? _resolved;

        public ExceptionSignatureReference(CilExceptionHandler handler, ReferenceResolver resolver)
        {
            _handler = handler;

            if (handler.ExceptionType != null)
                _resolved = resolver.Resolve(handler.ExceptionType!);
        }


        public void Fix()
        {
            _handler.ExceptionType = _resolved?.GetResolved();
        }
    }
}
