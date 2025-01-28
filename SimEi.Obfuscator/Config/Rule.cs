using System.Xml.Serialization;

namespace SimEi.Obfuscator.Config
{
    [XmlType]
    public class Rule
    {
        [XmlAttribute]
        public string? Name { get; set; }


        [XmlAttribute]
        public string? FullName { get; set; }


        [XmlAttribute]
        public string? HasAttribute { get; set; }


        [XmlIgnore]
        public bool? Public { get; set; }
        [XmlAttribute(nameof(Public))]
        public string? PublicStr
        {
            get => Public?.ToString();
            set => Public = value != null ? bool.Parse(value) : null;
        }


        [XmlAttribute]
        public string? SubtypeOf { get; set; }


        [XmlIgnore]
        public ActionType? Action { get; set; }
        [XmlAttribute(nameof(Action))]
        public string? ActonStr
        {
            get => Action?.ToString();
            set => Action = value != null ? Enum.Parse<ActionType>(value) : null;
        }


        [XmlElement(nameof(Rule))]
        public List<Rule>? Rules { get; set; }
    }
}
