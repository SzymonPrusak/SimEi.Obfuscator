using System.Diagnostics.CodeAnalysis;
using AsmResolver.DotNet.Signatures;

namespace SimEi.Obfuscator
{
    internal struct MethodIdentifier : IEquatable<MethodIdentifier>
    {
        public MethodSignature Signature;
        public string Name;

        public MethodIdentifier(MethodSignature signature, string name)
        {
            Signature = signature;
            Name = name;
        }


        public bool Equals(MethodIdentifier other) => SignatureComparer.Default.Equals(Signature, other.Signature) && Name.Equals(other.Name);

        public override bool Equals([NotNullWhen(true)] object? obj) => obj is MethodIdentifier i && Equals(i);

        public override int GetHashCode() => (SignatureComparer.Default.GetHashCode(Signature), Name).GetHashCode();
    }
}
