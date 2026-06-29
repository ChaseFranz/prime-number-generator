using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComputePrimeNumbers
{
    public class PrimeNumberGenerator
    {
        /// <summary>
        /// Computes all prime numbers from 2 up to and including <paramref name="n"/>
        /// using a segmented Sieve of Eratosthenes. The base primes up to sqrt(n) are
        /// found with a plain sequential sieve, then the remainder of the range is
        /// sieved in parallel across disjoint segments. Because each segment owns a
        /// distinct slice of the backing array, threads never write the same slot and
        /// no locking is required.
        /// </summary>
        /// <param name="n">The inclusive upper bound to search for primes.</param>
        /// <returns>
        /// The primes in ascending order, or an empty list when <paramref name="n"/> &lt; 2.
        /// </returns>
        public IReadOnlyList<int> CalculatePrimes(int n)
        {
            if (n < 2)
            {
                return Array.Empty<int>();
            }

            int sqrt = (int)Math.Sqrt(n);

            // composite[i] becomes true once i is known to be non-prime.
            bool[] composite = new bool[n + 1];

            // 1. Find the base primes in [2, sqrt(n)] with a sequential sieve.
            List<int> basePrimes = new List<int>();
            for (int candidate = 2; candidate <= sqrt; candidate++)
            {
                if (!composite[candidate])
                {
                    basePrimes.Add(candidate);
                    for (long multiple = (long)candidate * candidate; multiple <= sqrt; multiple += candidate)
                    {
                        composite[multiple] = true;
                    }
                }
            }

            // 2. Sieve (sqrt(n), n] in parallel. Each segment is a disjoint slice of the
            //    array, so segments can be marked concurrently without synchronization.
            int segmentStart = sqrt + 1;
            if (segmentStart <= n)
            {
                int span = n - segmentStart + 1;
                int segmentSize = Math.Max(1, span / (Environment.ProcessorCount * 4) + 1);
                int segmentCount = (span - 1) / segmentSize + 1;

                Parallel.For(0, segmentCount, segmentIndex =>
                {
                    int low = segmentStart + segmentIndex * segmentSize;
                    int high = (int)Math.Min((long)low + segmentSize - 1, n);

                    foreach (int prime in basePrimes)
                    {
                        // Start at the larger of prime*prime (smaller multiples carry a
                        // smaller prime factor already handled) and the first multiple
                        // of prime that falls within this segment.
                        long firstMultiple = Math.Max((long)prime * prime, ((low + prime - 1L) / prime) * prime);
                        for (long multiple = firstMultiple; multiple <= high; multiple += prime)
                        {
                            composite[multiple] = true;
                        }
                    }
                });
            }

            // 3. Collect the survivors in ascending order.
            List<int> primes = new List<int>();
            for (int i = 2; i <= n; i++)
            {
                if (!composite[i])
                {
                    primes.Add(i);
                }
            }

            return primes;
        }
    }
}
