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

        [XmlElement("Assembly")]
        public List<AssemblyXml> Assemblies { get; set; } = new List<AssemblyXml>();

        [XmlArray]
        public List<ExcludeClass> GlobalExcludes { get; set; } = new List<ExcludeClass>();
    }
}
