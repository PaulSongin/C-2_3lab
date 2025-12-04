using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace TextTokenizer
{
    [XmlRoot("Text")]
    public class Text
    {
        public List<Sentence> Sentences { get; set; } = new();

        public Text() { }

        public static Text Parse(string input)
        {
            Text t = new();
            string[] parts = Regex.Split(input, @"(?<=[.!?])\s+");
            foreach (var p in parts)
                if (!string.IsNullOrWhiteSpace(p))
                    t.Sentences.Add(Sentence.Parse(p));
            return t;
        }

        public IEnumerable<Sentence> OrderByWordCount() => Sentences.OrderBy(s => s.WordCount);
        public IEnumerable<Sentence> OrderByLength() => Sentences.OrderBy(s => s.ToString().Length);

        public IEnumerable<string> FindWordsInQuestions(int length)
        {
            HashSet<string> found = new();
            foreach (var s in Sentences.Where(s => s.Type == SentenceType.Question))
                foreach (var w in s.Words.Where(w => w.Length == length))
                    found.Add(w.Value);
            return found;
        }

        public void RemoveWordsByLengthAndConsonant(int length)
        {
            foreach (var s in Sentences)
                s.Words.RemoveAll(w => w.Length == length && w.StartsWithConsonant());
        }

        public void ReplaceWordsInSentence(int sentenceIndex, int length, string replacement)
        {
            if (sentenceIndex < 0 || sentenceIndex >= Sentences.Count) return;
            var s = Sentences[sentenceIndex];
            for (int i = 0; i < s.Words.Count; i++)
                if (s.Words[i].Length == length)
                    s.Words[i] = new Word(replacement);
        }

        // === Стоп-слова ===
        private static readonly HashSet<string> ruStop = new(StringComparer.OrdinalIgnoreCase);
        private static readonly HashSet<string> enStop = new(StringComparer.OrdinalIgnoreCase);

        public static void LoadStopWords(string ruPath, string enPath)
        {
            ruStop.Clear();
            enStop.Clear();

            if (File.Exists(ruPath))
                foreach (var w in File.ReadAllLines(ruPath))
                    if (!string.IsNullOrWhiteSpace(w)) ruStop.Add(w.Trim());

            if (File.Exists(enPath))
                foreach (var w in File.ReadAllLines(enPath))
                    if (!string.IsNullOrWhiteSpace(w)) enStop.Add(w.Trim());
        }

        public void RemoveStopWords()
        {
            foreach (var s in Sentences)
                s.Words.RemoveAll(w => IsStopWord(w.Value));
        }

        private static bool IsStopWord(string w)
        {
            if (string.IsNullOrWhiteSpace(w)) return false;
            w = w.ToLower();
            if (Regex.IsMatch(w, @"[а-яё]")) return ruStop.Contains(w);
            if (Regex.IsMatch(w, @"[a-z]")) return enStop.Contains(w);
            return false;
        }

        // === XML ===
        public void SaveAsXml(string path)
        {
            var ser = new XmlSerializer(typeof(Text));
            using var fs = new FileStream(path, FileMode.Create);
            ser.Serialize(fs, this);
        }

        public static Text LoadFromXml(string path)
        {
            var ser = new XmlSerializer(typeof(Text));
            using var fs = new FileStream(path, FileMode.Open);
            return (Text)ser.Deserialize(fs);
        }

        public override string ToString() => string.Join(" ", Sentences.Select(s => s.ToString()));
    }
}
