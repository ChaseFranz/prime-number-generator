using System;
using System.Diagnostics;

namespace ComputePrimeNumbers
{
    class Program
    {
        private static PrimeNumberGenerator primeNumberGenerator;
        private static readonly int EXIT_CODE = -1;

        public static void Main(string[] args)
        {
            primeNumberGenerator = new PrimeNumberGenerator();

            bool continueLoop = true;
            do
            {
                Console.WriteLine("Calculate Primes from 2 to N.");
                Console.WriteLine("Please enter a Natural Number or -1 to exit.");

                bool goodInput = int.TryParse(Console.ReadLine(), out int n);

                if (goodInput && n >= 2)
                {
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    primeNumberGenerator.CalculatePrimes(n);
                    stopWatch.Stop();
                    Console.WriteLine($"Execution Time: {stopWatch.Elapsed.TotalSeconds} seconds");

                    AskUserToPrintPrimes();
                }
                else if (n == EXIT_CODE)
                {
                    continueLoop = false;
                }
            } while (continueLoop);
        }

        private static void AskUserToPrintPrimes()
        {
            Console.WriteLine("Print Prime Numbers to Console? (y/n)");
            string userInput = Console.ReadLine();

            if (userInput.ToLower() == "y")
            {
                primeNumberGenerator?.LogPrimeNumbers();
            }
            Console.Write("\n\n");
        }
    }
}
