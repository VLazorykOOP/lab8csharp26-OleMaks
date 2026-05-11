using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace lab8
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            GenerateTestFiles();

            Console.WriteLine("ЗАВДАННЯ 1: Дати та час (рррр.мм.дд:гг:хх)");
            Task1();

            Console.WriteLine("\nЗАВДАННЯ 2: Видалення та заміна префіксів");
            Task2();

            Console.WriteLine("\nЗАВДАННЯ 3: Вилучення слів першого тексту з другого");
            Task3();

            Console.WriteLine("\nЗАВДАННЯ 4: Двійкові файли");
            Task4();

            Console.WriteLine("\nЗАВДАННЯ 5: Робота з файловою системою");
            Task5();
        }

        static void Task1()
        {
            string text = File.ReadAllText("task1.txt");
            Console.WriteLine("Оригінальний текст:\n" + text);

            string pattern = @"\b(19\d\d|20\d\d)\.(0[1-9]|1[0-2])\.(0[1-9]|[12]\d|3[01]):([01]\d|2[0-4]):([0-5]\d|60)\b";
            MatchCollection matches = Regex.Matches(text, pattern);

            Console.WriteLine($"\nЗнайдено дат-часу: {matches.Count}");
            foreach (Match m in matches)
            {
                Console.WriteLine($" - {m.Value}");
            }

            string modifiedText = Regex.Replace(text, pattern, "[ДАТА_ВИЛУЧЕНА]");
            File.WriteAllText("task1_out.txt", modifiedText);
            Console.WriteLine("Модифікований текст збережено у 'task1_out.txt'.");
        }

        static void Task2()
        {
            string text = File.ReadAllText("task2.txt");
            Console.WriteLine("Оригінальний текст: " + text);

            string result = Regex.Replace(text, @"\b\w+\b", match =>
            {
                string word = match.Value;
                

                if (Regex.IsMatch(word, @"^(re|not|be)", RegexOptions.IgnoreCase))
                    return "";

                if (Regex.IsMatch(word, @"^не", RegexOptions.IgnoreCase))
                    return Regex.Replace(word, @"^не", "not", RegexOptions.IgnoreCase);

                return word;
            });


            result = Regex.Replace(result, @"\s+", " ").Trim();

            File.WriteAllText("task2_out.txt", result);
            Console.WriteLine("Результат (task2_out.txt): " + result);
        }

        static void Task3()
        {
            string text1 = File.ReadAllText("task3_1.txt");
            string text2 = File.ReadAllText("task3_2.txt");

            Console.WriteLine("Текст 1: " + text1);
            Console.WriteLine("Текст 2: " + text2);

            var wordsToRemove = Regex.Matches(text1, @"\b\w+\b")
                                     .Cast<Match>()
                                     .Select(m => m.Value.ToLower())
                                     .ToHashSet();
            string resultText = Regex.Replace(text2, @"\b\w+\b", match =>
            {
                if (wordsToRemove.Contains(match.Value.ToLower()))
                    return ""; 
                return match.Value;
            });

            resultText = Regex.Replace(resultText, @"\s+", " ").Trim();
            File.WriteAllText("task3_out.txt", resultText);
            Console.WriteLine("Результат (task3_out.txt): " + resultText);
        }

        static void Task4()
        {
            string binFile = "task4.dat";
            Random rnd = new Random();
            int n = 10; 

            // Запис двійкового файлу
            using (BinaryWriter bw = new BinaryWriter(File.Open(binFile, FileMode.Create)))
            {
                Console.Write("Згенеровані числа: ");
                for (int i = 0; i < n; i++)
                {
                    double val = rnd.NextDouble() * 100;
                    Console.Write($"{val:F2}  ");
                    bw.Write(val);
                }
                Console.WriteLine();
            }

            double sum = 0;
            int count = 0;

            using (BinaryReader br = new BinaryReader(File.Open(binFile, FileMode.Open)))
            {
                long length = br.BaseStream.Length / 8; 
                
                for (long i = 0; i < length; i++)
                {
                    double val = br.ReadDouble();

                    if ((i + 1) % 2 == 0)
                    {
                        sum += val;
                        count++;
                    }
                }
            }

            if (count > 0)
                Console.WriteLine($"Середнє арифметичне елементів на парних позиціях: {sum / count:F2}");
        }

        static void Task5()
        {
            string surname = "Оленич"; 
            string basePath = Path.Combine(Directory.GetCurrentDirectory(), "temp"); 
            string dir1 = Path.Combine(basePath, surname + "1");
            string dir2 = Path.Combine(basePath, surname + "2");
            string allDir = Path.Combine(basePath, "ALL");

            if (Directory.Exists(basePath)) Directory.Delete(basePath, true);

            Directory.CreateDirectory(dir1);
            Directory.CreateDirectory(dir2);


            string f1 = Path.Combine(dir1, "t1.txt");
            string f2 = Path.Combine(dir1, "t2.txt");
            File.WriteAllText(f1, "<Янукович Віктор Іванович, 2001> року народження, місце проживання <м. Суми>");
            File.WriteAllText(f2, "<Комар Сергій Федорович, 2000 > року народження, місце проживання <м. Київ>");

            string f3 = Path.Combine(dir2, "t3.txt");
            string content1 = File.ReadAllText(f1);
            string content2 = File.ReadAllText(f2);
            File.WriteAllText(f3, content1 + Environment.NewLine + content2);


            FileInfo fi3 = new FileInfo(f3);
            Console.WriteLine($"Файл: {fi3.Name}, Розмір: {fi3.Length} байт, Створено: {fi3.CreationTime}");

            File.Move(f2, Path.Combine(dir2, "t2.txt"));
            File.Copy(f1, Path.Combine(dir2, "t1.txt"));

            Directory.Move(dir2, allDir);
            Directory.Delete(dir1, true);

            Console.WriteLine($"\nФайли у папці {allDir}:");
            DirectoryInfo dirAllInfo = new DirectoryInfo(allDir);
            foreach (FileInfo file in dirAllInfo.GetFiles())
            {
                Console.WriteLine($"- {file.Name} ({file.Length} байт)");
            }
        }

        static void GenerateTestFiles()
        {
            File.WriteAllText("task1.txt", "Система була запущена 2026.05.12:14:30. Наступне оновлення планується на 2026.06.01:08:00.");
            File.WriteAllText("task2.txt", "This is a rewrite and a return. Please restart the process.");
            File.WriteAllText("task3_1.txt", "hello world testing csharp");
            File.WriteAllText("task3_2.txt", "hello everyone! we are testing a new csharp program today.");
        }
    }
}