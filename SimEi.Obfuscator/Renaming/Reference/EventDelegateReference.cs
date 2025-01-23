using AsmResolver.DotNet;
using SimEi.Obfuscator.Renaming.Reference.Resolving;

namespace SimEi.Obfuscator.Renaming.Reference
{
    internal class EventDelegateReference : ITrackedReference
    {
        private readonly EventDefinition _event;

        private readonly IResolvedReference<ITypeDefOrRef> _delegateRef;

        public EventDelegateReference(EventDefinition @event)
        {
            _event = @event;

            _delegateRef = ReferenceResolver.Resolve(@event.EventType!);
        }


        public void Fix()
        {
            _event.EventType = _delegateRef.GetResolved();
        }
    }
}
