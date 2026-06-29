using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ComputePrimeNumbers
{
    class Program
    {
        private const int EXIT_CODE = -1;
        private static readonly PrimeNumberGenerator primeNumberGenerator = new PrimeNumberGenerator();

        public static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Calculate Primes from 2 to N.");
                Console.WriteLine("Please enter a Natural Number or -1 to exit.");

                string line = Console.ReadLine();
                if (line == null)
                {
                    // End of input (EOF or exhausted piped input) — exit cleanly.
                    break;
                }

                if (!int.TryParse(line, out int n) || (n < 2 && n != EXIT_CODE))
                {
                    Console.WriteLine("Please enter a whole number >= 2, or -1 to exit.\n");
                    continue;
                }

                if (n == EXIT_CODE)
                {
                    break;
                }

                Stopwatch stopWatch = Stopwatch.StartNew();
                IReadOnlyList<int> primes = primeNumberGenerator.CalculatePrimes(n);
                stopWatch.Stop();
                Console.WriteLine($"Execution Time: {stopWatch.Elapsed.TotalSeconds} seconds");

                AskUserToPrintPrimes(primes);
            }
        }

        private static void AskUserToPrintPrimes(IReadOnlyList<int> primes)
        {
            Console.WriteLine("Print Prime Numbers to Console? (y/n)");
            string userInput = Console.ReadLine();

            if (string.Equals(userInput, "y", StringComparison.OrdinalIgnoreCase))
            {
                PrintPrimes(primes);
            }
            Console.Write("\n\n");
        }

        private static void PrintPrimes(IReadOnlyList<int> primes)
        {
            if (primes.Count == 0)
            {
                Console.WriteLine("No prime numbers");
            }
            else
            {
                Console.WriteLine("Prime Numbers:");
                foreach (int prime in primes)
                {
                    Console.Write($"{prime} ");
                }
                Console.Write("\n");
            }
        }
    }
}
