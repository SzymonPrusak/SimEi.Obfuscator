using System.Xml.Serialization;

namespace SimEi.Obfuscator.Config
{
    [XmlType("Module")]
    public class Module
    {
        [XmlAttribute]
        public string Name { get; set; } = string.Empty;

        [XmlElement(nameof(Rule))]
        public List<Rule> Rules { get; set; } = new List<Rule>();
    }
}
