using System.Xml.Serialization;

namespace SimEi.Obfuscator.Config
{
    [XmlType]
    public class ProbePath
    {
        [XmlAttribute]
        public string Value { get; set; } = string.Empty;
    }
}
