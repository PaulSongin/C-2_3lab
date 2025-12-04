using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

struct GeneticData
{
    public string Protein;
    public string Organism;
    public string AminoAcids;
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("=== Генетический поиск ===");
        Console.Write("Введите путь к файлу sequences.txt: ");
        string seqPath = Console.ReadLine();
        Console.Write("Введите путь к файлу commands.txt: ");
        string cmdPath = Console.ReadLine();
        Console.Write("Введите имя для записи в genedata.txt: ");
        string userName = Console.ReadLine();

        List<GeneticData> data = ReadSequences(seqPath);
        string outFile = "genedata.txt";

        using (StreamWriter writer = new StreamWriter(outFile))
        {
            writer.WriteLine(userName);
            writer.WriteLine("Генетический поиск");

            int opIndex = 1;
            foreach (string line in File.ReadLines(cmdPath))
            {
                string[] parts = line.Split('\t');
                string command = parts[0].Trim();

                Console.WriteLine($"Выполняю команду #{opIndex}: {line}");
                writer.WriteLine($"{opIndex:000} {command}");

                if (command == "search" && parts.Length >= 2)
                {
                    string seq = RLDecoding(parts[1].Trim());
                    Search(seq, data, writer);
                }
                else if (command == "diff" && parts.Length >= 3)
                {
                    Diff(parts[1].Trim(), parts[2].Trim(), data, writer);
                }
                else if (command == "mode" && parts.Length >= 2)
                {
                    Mode(parts[1].Trim(), data, writer);
                }
                else
                {
                    writer.WriteLine("INVALID COMMAND");
                }

                writer.WriteLine(new string('-', 40));
                opIndex++;
            }
        }

        Console.WriteLine($"Результат записан в {outFile}");
    }

    static List<GeneticData> ReadSequences(string path)
    {
        List<GeneticData> result = new List<GeneticData>();
        foreach (string line in File.ReadLines(path))
        {
            string[] parts = line.Split('\t');
            if (parts.Length < 3) continue;

            GeneticData gd = new GeneticData();
            gd.Protein = parts[0].Trim();
            gd.Organism = parts[1].Trim();
            gd.AminoAcids = RLDecoding(parts[2].Trim());
            result.Add(gd);
        }
        return result;
    }

    static void Search(string sequence, List<GeneticData> data, StreamWriter writer)
    {
        bool found = false;
        foreach (var g in data)
        {
            if (g.AminoAcids.Contains(sequence))
            {
                writer.WriteLine($"{g.Organism}\t{g.Protein}");
                found = true;
            }
        }
        if (!found) writer.WriteLine("NOT FOUND");
    }

    static void Diff(string protein1, string protein2, List<GeneticData> data, StreamWriter writer)
    {
        GeneticData? g1 = null, g2 = null;
        foreach (var g in data)
        {
            if (g.Protein == protein1) g1 = g;
            if (g.Protein == protein2) g2 = g;
        }
        writer.Write("amino-acids difference: ");
        if (g1 == null || g2 == null)
        {
            writer.Write("MISSING:");
            if (g1 == null) writer.Write(" " + protein1);
            if (g2 == null) writer.Write(" " + protein2);
            writer.WriteLine();
            return;
        }

        string s1 = g1.Value.AminoAcids, s2 = g2.Value.AminoAcids;
        int diff = Math.Abs(s1.Length - s2.Length);
        for (int i = 0; i < Math.Min(s1.Length, s2.Length); i++)
            if (s1[i] != s2[i]) diff++;
        writer.WriteLine(diff);
    }

    static void Mode(string protein, List<GeneticData> data, StreamWriter writer)
    {
        GeneticData? g = null;
        foreach (var x in data)
            if (x.Protein == protein) { g = x; break; }

        writer.Write("amino-acid occurs: ");
        if (g == null)
        {
            writer.WriteLine("MISSING: " + protein);
            return;
        }

        Dictionary<char, int> freq = new Dictionary<char, int>();
        foreach (char c in g.Value.AminoAcids)
        {
            if (!freq.ContainsKey(c)) freq[c] = 0;
            freq[c]++;
        }

        int maxCount = -1; char best = '\0';
        foreach (var kv in freq)
        {
            if (kv.Value > maxCount || (kv.Value == maxCount && kv.Key < best))
            {
                best = kv.Key; maxCount = kv.Value;
            }
        }
        writer.WriteLine($"{best} {maxCount}");
    }

    static string RLDecoding(string amino_acids)
    {
        if (string.IsNullOrEmpty(amino_acids)) return "";
        StringBuilder result = new StringBuilder();
        int i = 0;
        while (i < amino_acids.Length)
        {
            if (char.IsDigit(amino_acids[i]))
            {
                int count = amino_acids[i] - '0';
                i++;
                if (i < amino_acids.Length)
                    result.Append(new string(amino_acids[i], count));
            }
            else
            {
                result.Append(amino_acids[i]);
            }
            i++;
        }
        return result.ToString();
    }
}
