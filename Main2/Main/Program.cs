using System;
using System.IO;

namespace TextTokenizer
{
    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Токенизация текста ===");

            string basePath = Environment.CurrentDirectory;
            string inputPath = Path.Combine(basePath, "input.txt");
            string ruPath = Path.Combine(basePath, "stopwords_ru.txt");
            string enPath = Path.Combine(basePath, "stopwords_en.txt");
            string xmlPath = Path.Combine(basePath, "text.xml");

            string inputText = string.Empty;

            Console.WriteLine("Выберите источник текста:");
            Console.WriteLine("1 — загрузить текст из файла input.txt");
            Console.WriteLine("2 — использовать тестовый встроенный пример");
            Console.Write("Ваш выбор: ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                if (File.Exists(inputPath))
                {
                    inputText = File.ReadAllText(inputPath);
                    Console.WriteLine($"\nТекст загружен из файла: {inputPath}");
                }
                else
                {
                    Console.WriteLine("\nФайл input.txt не найден. Будет использован тестовый пример.\n");
                    inputText = SampleText();
                }
            }
            else
            {
                inputText = SampleText();
                Console.WriteLine("\nИспользуется тестовый текст:\n");
            }

            Console.WriteLine(inputText);

            // === Загрузка стоп-слов ===
            Text.LoadStopWords(ruPath, enPath);

            // === Парсинг и работа с текстом ===
            Text text = Text.Parse(inputText);
            Console.WriteLine("\n== Исходный текст ==");
            Console.WriteLine(text);

            text.RemoveStopWords();
            Console.WriteLine("\n== После удаления стоп-слов ==");
            Console.WriteLine(text);

            Console.WriteLine("\n== Сортировка по количеству слов ==");
            foreach (var s in text.OrderByWordCount())
                Console.WriteLine(s);

            Console.WriteLine("\n== Слова длиной 3 в вопросительных предложениях ==");
            foreach (var w in text.FindWordsInQuestions(3))
                Console.WriteLine(w);

            text.RemoveWordsByLengthAndConsonant(5);
            text.ReplaceWordsInSentence(0, 4, "[заменено]");

            Console.WriteLine("\n== После удаления и замены ==");
            Console.WriteLine(text);

            // === XML ===
            text.SaveAsXml(xmlPath);
            Console.WriteLine($"\nXML сохранён: {xmlPath}");

            // === Проверка десериализации ===
            Text loaded = Text.LoadFromXml(xmlPath);
            Console.WriteLine("\n== Текст, считанный обратно из XML ==");
            Console.WriteLine(loaded);
        }

        private static string SampleText()
        {
            return "I love programming, but I do not like bugs. " +
                   "Я люблю программирование, но не люблю ошибки! " +
                   "Как ты думаешь?";
        }
    }
}
