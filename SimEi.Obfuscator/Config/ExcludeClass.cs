using System.Xml.Serialization;

namespace SimEi.Obfuscator.Config
{
    [XmlType]
    public class ExcludeClass
    {
        [XmlAttribute]
        public string? Name { get; set; }


        [XmlIgnore]
        public bool? Public { get; set; }
        [XmlAttribute(nameof(Public))]
        public string? PublicStr
        {
            get => Public?.ToString();
            set { Public = value != null ? bool.Parse(value) : null; }
        }


        [XmlAttribute]
        public string? SubtypeOf { get; set; }


        [XmlElement("ExcludeMember")]
        public List<MemberInfo>? Members { get; set; }
    }
}
