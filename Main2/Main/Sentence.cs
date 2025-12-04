using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TextTokenizer
{
    public enum SentenceType
    {
        Declarative,  // .
        Question,     // ?
        Exclamation,  // !
        Unknown
    }

    public class Sentence
    {
        public List<Word> Words { get; set; } = new();
        public SentenceType Type { get; set; } = SentenceType.Unknown;

        public Sentence() { }

        public static Sentence Parse(string raw)
        {
            Sentence s = new();
            var matches = Regex.Matches(raw, @"\w+|[.!?]");
            foreach (Match m in matches)
            {
                if (Regex.IsMatch(m.Value, @"\w+"))
                    s.Words.Add(new Word(m.Value));
                else
                {
                    s.Type = m.Value switch
                    {
                        "." => SentenceType.Declarative,
                        "?" => SentenceType.Question,
                        "!" => SentenceType.Exclamation,
                        _ => SentenceType.Unknown
                    };
                }
            }
            return s;
        }

        public bool IsQuestion => Type == SentenceType.Question;
        public int WordCount => Words.Count;

        public override string ToString()
        {
            string ending = Type switch
            {
                SentenceType.Question => "?",
                SentenceType.Exclamation => "!",
                SentenceType.Declarative => ".",
                _ => ""
            };
            return string.Join(" ", Words.Select(w => w.Value)) + ending;
        }
    }
}
