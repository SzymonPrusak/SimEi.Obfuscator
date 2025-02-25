﻿using AsmResolver.DotNet.Signatures;
using SimEi.Obfuscator.Renaming.Reference.Resolving;

namespace SimEi.Obfuscator.Renaming.Reference
{
    internal class FieldSignatureReference : ITrackedReference
    {
        private readonly FieldSignature _signature;

        private readonly IResolvedReference<TypeSignature> _fieldTypeRef;

        public FieldSignatureReference(FieldSignature signature, ReferenceResolver resolver)
        {
            _signature = signature;

            _fieldTypeRef = resolver.ResolveSig(_signature.FieldType);
        }


        public void Fix()
        {
            _signature.FieldType = _fieldTypeRef.GetResolved();
        }
    }
}
