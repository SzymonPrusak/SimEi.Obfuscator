using System.Xml.Serialization;
using SimEi.Obfuscator.Renaming;

namespace SimEi.Obfuscator.Config
{
    [XmlType("Member")]
    public class MemberInfo
    {
        [XmlAttribute]
        public string? Name { get; set; }


        [XmlIgnore]
        public bool? Private { get; set; }
        [XmlAttribute(nameof(Private))]
        public string? PrivateStr
        {
            get => Private?.ToString();
            set { Private = value != null ? bool.Parse(value) : null; }
        }


        [XmlIgnore]
        public RenamedElementType? ElementType { get; set; }
        [XmlAttribute(nameof(ElementType))]
        public string? TypeStr
        {
            get => ElementType?.ToString();
            set { ElementType = value != null ? Enum.Parse<RenamedElementType>(value) : null; }
        }
    }
}
