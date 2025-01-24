using AsmResolver.DotNet;

namespace SimEi.Obfuscator.Renaming
{
    internal class NamingContext : INamingContext
    {
        private readonly string _prefix;
        private readonly NamingLevel _moduleNaming;
        private readonly Dictionary<TypeDefinition, NamingLevel> _subTypeNamings;

        public NamingContext(string prefix = "_")
        {
            _prefix = prefix;

            _moduleNaming = new NamingLevel();
            _subTypeNamings = new Dictionary<TypeDefinition, NamingLevel>();
        }


        public string GetNextName(TypeDefinition? type, RenamedElementType elType)
        {
            string name;
            if (type == null)
                name = _moduleNaming.GetNextName(elType);
            else
            {
                if (!_subTypeNamings.TryGetValue(type, out var level))
                {
                    level = new NamingLevel();
                    _subTypeNamings[type] = level;
                }
                name = level.GetNextName(elType);
            }
            return _prefix + name;
        }


        private class NamingLevel
        {
            private const string AllChars = "ㄱㄲㄳㄴㄵㄶㄷㄸㄹㄺㄻㄼㄽㄾㄿㅀㅁㅂㅃㅄㅅㅆㅇㅈㅉㅊㅋㅌㅍㅎㅏㅐㅑㅒㅓㅔㅕㅖㅗㅘㅙㅚㅛㅜㅝㅞㅟㅠㅡㅢㅥㅦㅧㅨㅩㅪㅫㅬㅭㅮㅯㅰㅱㅲㅳㅴㅵㅶㅷㅸㅹㅺㅻㅼㅽㅾㅿㆀㆁㆂㆃㆄㆅㆆㆇㆈㆉㆊ㊐㊑㊒㊓㊔㊕㊊㊋㊌㊍㊎㊏㊖㊗㊞㊟㊠㊡㊢㊣㊤㊥㊦㊘㊙㊚㊛㊜㊝㊧㊨㊩㊪㊫㊬㊭㊰㊮㊯ッツヅミテデトドナぁあぃいぅうぇえぉおかがきぎけげこごさざしじすずせぜそぞただちぢっつづてでとどなにぬねのはばぱひびぴふぶぷへべぺほぼぽまみむめもゃやゅゆょよらりるれろゎわゐゑをんゔゕゖ゛゜ゝゞゟ゠ァアィイカガキギクグケゲコゴサザシジスズセゼソゾタダチヂニヌネノハバパヒビピフブプヘベペホボポマムメモャヤュユョヨラリルレロヮワヰヱヲンヴヵヶヷヸヹヺヾくぐゥウェエォオ";

            private readonly Dictionary<RenamedElementType, (char[], int)> _elNamings;

            public NamingLevel()
            {
                _elNamings = new Dictionary<RenamedElementType, (char[], int)>();
                foreach (RenamedElementType type in Enum.GetValues(typeof(RenamedElementType)))
                    _elNamings[type] = (GetRandomNamingChars(), 0);
            }


            public string GetNextName(RenamedElementType type)
            {
                (var chars, int index) = _elNamings[type];

                string n = string.Empty;
                int left = index;
                do
                {
                    n += chars[left % chars.Length];
                    left = left / chars.Length;
                }
                while (left > 0);

                _elNamings[type] = (chars, index + 1);
                return n;
            }


            private char[] GetRandomNamingChars()
            {
                var random = new Random();
                var chars = AllChars.ToCharArray();
                random.Shuffle(chars);
                chars = chars
                    .Take(random.Next(chars.Length / 2, chars.Length))
                    .ToArray();
                return chars;
            }
        }
    }
}
