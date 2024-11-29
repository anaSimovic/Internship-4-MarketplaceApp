using System;

namespace Marketplace.Utilities
{
    public static class InputValidator
    {
        public static string GetNonEmptyString(string prompt)
        {
            string input;
            do
            {
                Console.WriteLine(prompt);
                input = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Input cannot be empty. Please try again.");
                }
            } while (string.IsNullOrWhiteSpace(input));

            return input;
        }

        public static decimal GetPositiveDecimal(string prompt)
        {
            decimal value;
            do
            {
                Console.WriteLine(prompt);
                string input = Console.ReadLine();

                if (decimal.TryParse(input, out value) && value > 0)
                {
                    return value;
                }
                Console.WriteLine("Please enter a valid positive number.");
            } while (true);
        }

        public static int GetPositiveInt(string prompt)
        {
            int value;
            do
            {
                Console.WriteLine(prompt);
                string input = Console.ReadLine();

                if (int.TryParse(input, out value) && value > 0)
                {
                    return value;
                }
                Console.WriteLine("Please enter a valid positive integer.");
            } while (true);
        }

        public static DateTime GetValidDate(string prompt)
        {
            DateTime date;
            do
            {
                Console.WriteLine(prompt);
                string input = Console.ReadLine();

                if (DateTime.TryParse(input, out date) && date.Year > 1900 && date <= DateTime.Now)
                {
                    return date;
                }
                Console.WriteLine("Please enter a realistic date (after 1900 and not in the future).");
            } while (true);
        }
    }
}
