using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace RegularExpression
{
    /// <summary>
    /// Програма в одному файлі: клас PhoneNumberFinder інкапсулює Regex,
    /// а Program.Main демонструє використання.
    /// Шукає номери у форматі +3(000)-000-0000.
    /// </summary>
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // Отримати текст для аналізу:
            // Якщо передано шлях до файлу як аргумент командного рядка, читаємо з файлу.
            // Інакше просимо користувача ввести текст у консоль (закінчити введення порожнім рядком).
            string inputText;
            if (args.Length >= 1 && File.Exists(args[0]))
            {
                inputText = File.ReadAllText(args[0]);
            }
            else
            {
                Console.WriteLine("Введіть текст для пошуку (щоб завершити — введіть порожній рядок):");
                var lines = new List<string>();
                while (true)
                {
                    var line = Console.ReadLine();
                    if (line == null)
                    {
                        // Кінець потоку вводу (наприклад, перенаправлено файл)
                        break;
                    }

                    if (line.Length == 0)
                    {
                        // Порожній рядок завершує введення
                        break;
                    }

                    lines.Add(line);
                }

                // Якщо користувач нічого не ввів — використовуємо демонстраційний приклад
                if (lines.Count == 0)
                {
                    inputText = "Контакти: +3(012)-345-6789, текст, +3(000)-000-0000; помилковий +3(12)-123-1234; ще один +3(123)-456-7890.";
                    Console.WriteLine();
                    Console.WriteLine("Не введено текст. Використовується приклад:");
                    Console.WriteLine(inputText);
                    Console.WriteLine();
                }
                else
                {
                    inputText = string.Join(Environment.NewLine, lines);
                }
            }

            // Створюємо пошуковий об'єкт і знаходимо номери
            var finder = new PhoneNumberFinder();
            var matches = finder.FindPhoneNumbers(inputText);

            Console.WriteLine();
            Console.WriteLine("Знайдені номери у форматі +3(000)-000-0000:");
            var foundAny = false;
            foreach (var m in matches)
            {
                Console.WriteLine(m);
                foundAny = true;
            }

            if (!foundAny)
            {
                Console.WriteLine("Нічого не знайдено.");
            }

            // Затримка для зручності в Visual Studio
            Console.WriteLine();
            Console.WriteLine("Натисніть будь-яку клавішу для виходу...");
            Console.ReadKey(intercept: true);
        }
    }

    /// <summary>
    /// Інкапсульований клас для пошуку телефонних номерів за регулярним виразом.
    /// За замовчуванням шукає формат: +3(000)-000-0000.
    /// </summary>
    public sealed class PhoneNumberFinder
    {
        // Приватне readonly поле для Regex (інкапсуляція, дотримання конвенцій)
        private readonly Regex _phoneRegex;

        /// <summary>
        /// Ініціалізує екземпляр з дефолтним патерном для формату +3(000)-000-0000.
        /// </summary>
        public PhoneNumberFinder()
            : this(@"\+3\(\d{3}\)-\d{3}-\d{4}")
        {
        }

        /// <summary>
        /// Ініціалізує екземпляр з конкретним патерном регулярного виразу.
        /// </summary>
        /// <param name="pattern">Регулярний вираз (не null/порожній).</param>
        public PhoneNumberFinder(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                throw new ArgumentException("Pattern cannot be null or whitespace.", nameof(pattern));
            }

            // Компільований для кращої продуктивності та без врахування локалі
            _phoneRegex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant);
        }

        /// <summary>
        /// Повертає всі знайдені телефонні номери у вхідному тексті.
        /// </summary>
        /// <param name="text">Вхідний текст (не null).</param>
        /// <returns>IEnumerable рядків — знайдені номери.</returns>
        public IEnumerable<string> FindPhoneNumbers(string text)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            var matches = _phoneRegex.Matches(text);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    yield return match.Value;
                }
            }
        }
    }
}