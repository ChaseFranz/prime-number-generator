using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputePrimeNumbers
{
    public class PrimeNumberGenerator
    {
        private ConcurrentBag<int> PrimeNumbers { get; set; }

        public List<int> GetOrderedPrimeNumbers() => PrimeNumbers.OrderBy(p => p).ToList();

        /// <summary>
        /// Generates all prime numbers up to N.
        /// </summary>
        /// <returns></returns>
        public void CalculatePrimes(int n)
        {
            PrimeNumbers = new ConcurrentBag<int>();
            int minPrimeCandidate = 2;

            Parallel.For(minPrimeCandidate, n + 1, (int primeCandidate) =>
            {
                bool isPrime = true;
                double primeCandidateRoot = Math.Sqrt(primeCandidate);

                // check current primeCandidate for prime
                for (int divisor = 2; divisor <= primeCandidateRoot; divisor++)
                {
                    if (primeCandidate % divisor == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }
                if (isPrime)
                {
                    PrimeNumbers.Add(primeCandidate);
                }
            });
        }

        /// <summary>
        /// Logs all computed prime numbers to console
        /// </summary>
        public void LogPrimeNumbers()
        {
            if (PrimeNumbers.IsEmpty)
            {
                Console.WriteLine($"No prime numbers");
            }
            else
            {
                Console.WriteLine("Prime Numbers:");
                foreach (int prime in GetOrderedPrimeNumbers())
                {
                    Console.Write($"{prime} ");
                }
                Console.Write("\n");
            }
        }
    }
}
