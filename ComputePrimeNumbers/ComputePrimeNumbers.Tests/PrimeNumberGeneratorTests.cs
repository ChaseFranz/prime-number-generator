using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ComputePrimeNumbers.Tests
{
    public class PrimeNumberGeneratorTests
    {
        private readonly PrimeNumberGenerator _generator = new PrimeNumberGenerator();

        [Theory]
        [InlineData(-5)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void CalculatePrimes_BelowTwo_ReturnsEmpty(int n)
        {
            Assert.Empty(_generator.CalculatePrimes(n));
        }

        [Fact]
        public void CalculatePrimes_Two_ReturnsSinglePrime()
        {
            Assert.Equal(new[] { 2 }, _generator.CalculatePrimes(2));
        }

        [Fact]
        public void CalculatePrimes_Ten_ReturnsKnownPrimes()
        {
            Assert.Equal(new[] { 2, 3, 5, 7 }, _generator.CalculatePrimes(10));
        }

        [Fact]
        public void CalculatePrimes_Thirty_ReturnsKnownPrimes()
        {
            Assert.Equal(
                new[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29 },
                _generator.CalculatePrimes(30));
        }

        [Fact]
        public void CalculatePrimes_ResultIsAscending()
        {
            IReadOnlyList<int> primes = _generator.CalculatePrimes(1000);

            Assert.Equal(primes.OrderBy(p => p), primes);
        }

        [Fact]
        public void CalculatePrimes_IncludesUpperBoundWhenPrime()
        {
            IReadOnlyList<int> primes = _generator.CalculatePrimes(13);

            Assert.Contains(13, primes);
            Assert.Equal(13, primes[primes.Count - 1]);
        }

        [Fact]
        public void CalculatePrimes_ExcludesUpperBoundWhenComposite()
        {
            IReadOnlyList<int> primes = _generator.CalculatePrimes(100);

            Assert.DoesNotContain(100, primes);
            Assert.Contains(97, primes); // largest prime <= 100
        }

        // Prime-counting function values: pi(100)=25, pi(1000)=168, pi(10000)=1229.
        [Theory]
        [InlineData(100, 25)]
        [InlineData(1000, 168)]
        [InlineData(10000, 1229)]
        public void CalculatePrimes_ReturnsExpectedCount(int n, int expectedCount)
        {
            Assert.Equal(expectedCount, _generator.CalculatePrimes(n).Count);
        }

        [Fact]
        public void CalculatePrimes_AllResultsArePrime()
        {
            IReadOnlyList<int> primes = _generator.CalculatePrimes(2000);

            Assert.All(primes, p => Assert.True(IsPrime(p), $"{p} should be prime"));
        }

        [Fact]
        public void CalculatePrimes_MatchesReferenceImplementation()
        {
            // Exercises a range large enough to span the sqrt boundary and many
            // parallel segments, cross-checked against an independent trial-division.
            const int n = 5000;

            Assert.Equal(ReferencePrimes(n), _generator.CalculatePrimes(n));
        }

        private static List<int> ReferencePrimes(int n)
        {
            List<int> primes = new List<int>();
            for (int candidate = 2; candidate <= n; candidate++)
            {
                if (IsPrime(candidate))
                {
                    primes.Add(candidate);
                }
            }
            return primes;
        }

        private static bool IsPrime(int value)
        {
            if (value < 2)
            {
                return false;
            }
            for (int divisor = 2; (long)divisor * divisor <= value; divisor++)
            {
                if (value % divisor == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
