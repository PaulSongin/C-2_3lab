namespace TextTokenizer
{
    public class Word
    {
        public string Value { get; set; }

        public Word() { }
        public Word(string value) => Value = value;

        public int Length => Value?.Length ?? 0;

        public bool StartsWithConsonant()
        {
            if (string.IsNullOrEmpty(Value)) return false;
            char first = char.ToLower(Value[0]);
            return "бвгджзйклмнпрстфхцчшщbcdfghjklmnpqrstvwxyz".Contains(first);
        }

        public override string ToString() => Value;
    }
}
