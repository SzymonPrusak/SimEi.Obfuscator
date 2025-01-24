using System.Xml.Serialization;

namespace SimEi.Obfuscator.Config
{
    [XmlType("Assembly")]
    public class AssemblyXml
    {
        [XmlAttribute]
        public string Path { get; set; } = string.Empty;

        [XmlElement(nameof(ExcludeClass))]
        public List<ExcludeClass> Excludes { get; set; } = new List<ExcludeClass>();
    }
}
