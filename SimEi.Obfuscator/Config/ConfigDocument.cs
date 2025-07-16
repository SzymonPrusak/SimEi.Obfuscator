using System.Xml.Serialization;

namespace SimEi.Obfuscator.Config
{
    [XmlType]
    public class ConfigDocument
    {
        [XmlElement]
        public BaseDir? BaseDir { get; set; }

        [XmlElement("ProbePath")]
        public List<ProbePath> ProbePaths { get; set; } = new List<ProbePath>();

        [XmlElement("Module")]
        public List<Module> Modules { get; set; } = new List<Module>();

        [XmlArray]
        public List<Rule> GlobalRules { get; set; } = new List<Rule>();

        [XmlElement("StripAttribute")]
        public List<string> StrippedAttributes = new List<string>();
    }
}
