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


        [XmlIgnore]
        public bool? SkipMembers { get; set; }
        [XmlAttribute(nameof(SkipMembers))]
        public string? SkipMembersStr
        {
            get => SkipMembers?.ToString();
            set { SkipMembers = value != null ? bool.Parse(value) : null; }
        }


        [XmlElement("ExcludeMember")]
        public List<MemberInfo>? ExplicitMembers { get; set; }
    }
}
