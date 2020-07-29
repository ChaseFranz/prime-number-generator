using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputePrimeNumbers
{
    public class PrimeNumberGenerator
    {
        public ConcurrentBag<int> PrimeNumbers { get; private set; }

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

        public void LogPrimeNumbers()
        {
            if (PrimeNumbers.IsEmpty)
            {
                Console.WriteLine($"No prime numbers");
            }
            else
            {
                Console.WriteLine("Prime Numbers:");
                foreach (int prime in PrimeNumbers.OrderBy(p => p).ToList())
                {
                    Console.Write($"{prime} ");
                }
                Console.Write("\n");
            }
        }
    }
}
