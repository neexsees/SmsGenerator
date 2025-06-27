using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmsGeneratorApp;

namespace TestProject1
{
    [TestClass]
    public sealed class CodeGeneratorTests
    {
        private const int TestIterations = 100;
        private static readonly Random TestRandom = new Random();

        #region Helper Methods
        private static bool IsPrime(int number)
        {
            if (number <= 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            var boundary = (int)Math.Floor(Math.Sqrt(number));

            for (int i = 3; i <= boundary; i += 2)
                if (number % i == 0)
                    return false;

            return true;
        }
        #endregion

        #region GenerateFirst2NPrimes Tests
        [TestMethod]
        public void GenerateFirst2NPrimes_ZeroOrNegativeInput_ReturnsEmptyArray()
        {
            // Arrange & Act
            var zeroResult = CodeGenerator.GenerateFirst2NPrimes(0);
            var negativeResult = CodeGenerator.GenerateFirst2NPrimes(-5);

            // Assert
            Assert.IsNotNull(zeroResult);
            Assert.AreEqual(0, zeroResult.Length);
            Assert.IsNotNull(negativeResult);
            Assert.AreEqual(0, negativeResult.Length);
        }
        [TestMethod]
        public void GenerateFirst2NPrimes_SmallInputs_ReturnsCorrectPrimes()
        {
            // Arrange
            var testCases = new Dictionary<int, int[]>
            {
                [1] = new[] { 2, 3 },
                [2] = new[] { 2, 3, 5, 7 },
                [3] = new[] { 2, 3, 5, 7, 11, 13 },
                [4] = new[] { 2, 3, 5, 7, 11, 13, 17, 19 }
            };

            foreach (var testCase in testCases)
            {
                int input = testCase.Key;
                int[] expectedPrimes = testCase.Value;

                // Act
                int[] result = CodeGenerator.GenerateFirst2NPrimes(input);

                // Assert - проверяем основные свойства
                Assert.AreEqual(2 * input, result.Length,
                    $"For input {input}, expected {2 * input} primes, got {result.Length}");

                // Проверяем, что все числа простые
                foreach (int prime in result)
                {
                    Assert.IsTrue(IsPrime(prime),
                        $"Number {prime} in result is not prime for input {input}");
                }

                // Проверяем, что результат содержит ожидаемые простые числа
                // (но не обязательно в том же порядке)
                foreach (int expectedPrime in expectedPrimes)
                {
                    Assert.IsTrue(result.Contains(expectedPrime),
                        $"Expected prime {expectedPrime} not found in result for input {input}");
                }

                // Проверяем, что числа уникальны (без дубликатов)
                Assert.AreEqual(result.Length, result.Distinct().Count(),
                    $"Result contains duplicate primes for input {input}");
            }
        }
        [TestClass]
        public class FindValidModulusTests
        {
            private const int TestIterations = 50;

            #region Basic Validation Tests
            [TestMethod]
            [ExpectedException(typeof(ArgumentException))]
            public void FindValidModulus_InvalidLengthCode_ThrowsException()
            {
                // Arrange
                var invalidLengths = new[] { -1, 0, 5, 10, 15 };

                foreach (var length in invalidLengths)
                {
                    // Act
                    CodeGenerator.FindValidModulus(length, 10, out _, out _, out _, out _);
                }
            }

            [TestMethod]
            public void FindValidModulus_ValidLengthCodes_DoesNotThrow()
            {
                // Arrange
                var validLengths = new[] { 6, 7, 8, 9 };

                foreach (var length in validLengths)
                {
                    // Act
                    var m = CodeGenerator.FindValidModulus(length, 10, out _, out _, out _, out _);

                    // Assert
                    Assert.IsTrue(m >= Math.Pow(10, length - 1));
                    Assert.IsTrue(m <= Math.Pow(10, length) - 1);
                }
            }
            #endregion

            #region Core Functionality Tests
            [TestMethod]
            public void FindValidModulus_ReturnsValidModulusForDifferentLengths()
            {
                // Arrange
                var testCases = new[]
                {
            new { Length = 6, Min = 100_000L, Max = 999_999L },
            new { Length = 7, Min = 1_000_000L, Max = 9_999_999L },
            new { Length = 8, Min = 10_000_000L, Max = 99_999_999L },
            new { Length = 9, Min = 100_000_000L, Max = 999_999_999L }
        };

                foreach (var testCase in testCases)
                {
                    // Act
                    var m = CodeGenerator.FindValidModulus(testCase.Length, 10,
                        out long phi, out var divisors, out long p, out long q);

                    // Assert
                    Assert.IsTrue(m >= testCase.Min && m <= testCase.Max,
                        $"For length {testCase.Length}, m ({m}) should be in range");
                    Assert.AreEqual(m, p * q, "m should equal p * q");
                    Assert.IsTrue(phi > 20, $"phi ({phi}) should be greater than 20");
                    Assert.IsTrue(divisors.Count > 0, "Should have divisors");
                    Assert.IsTrue(CodeGenerator.MutualSimplicity((int)p, (int)q),
                        "p and q should be coprime");
                }
            }

            [TestMethod]
            public void FindValidModulus_ReturnsDifferentValuesOnMultipleCalls()
            {
                // Arrange
                const int lengthCode = 6;
                const int numberOfCodes = 10;
                var results = new HashSet<long>();

                // Act
                for (int i = 0; i < TestIterations; i++)
                {
                    var m = CodeGenerator.FindValidModulus(lengthCode, numberOfCodes,
                        out _, out _, out _, out _);
                    results.Add(m);
                }

                // Assert
                Assert.IsTrue(results.Count > 1, "Should return different moduli");
            }
            #endregion

            #region Edge Case Tests
            [TestMethod]
            public void FindValidModulus_HandlesExtremeNumberOfCodes()
            {
                // Arrange
                var testCases = new[]
                {
            new { NumberOfCodes = 1, MinPhi = 2L },
            new { NumberOfCodes = 1000, MinPhi = 2000L },
            new { NumberOfCodes = 5000, MinPhi = 10000L }
        };

                foreach (var testCase in testCases)
                {
                    // Act
                    var m = CodeGenerator.FindValidModulus(6, testCase.NumberOfCodes,
                        out long phi, out _, out _, out _);

                    // Assert
                    Assert.IsTrue(phi > testCase.MinPhi,
                        $"For {testCase.NumberOfCodes} codes, phi should be > {testCase.MinPhi}");
                }
            }

            [TestMethod]
            public void FindValidModulus_HandlesEdgeCase_WhenMinQEqualsMaxQ()
            {
                // Arrange
                const int lengthCode = 6;
                const int numberOfCodes = 10;

                // Принудительно создаем edge-case
                long p = 999983; // Большое простое число, близкое к 10^6
                long mMin = (long)Math.Pow(10, lengthCode - 1);
                long mMax = (long)Math.Pow(10, lengthCode) - 1;
                long minQ = mMin / p + 1;
                long maxQ = mMax / p;

                Assert.IsTrue(minQ >= maxQ, "Тестовые данные не создают edge-case");

                // Act
                long m = CodeGenerator.FindValidModulus(lengthCode, numberOfCodes,
                    out _, out _, out long foundP, out long foundQ);

                // Assert
                Assert.IsTrue(m >= mMin && m <= mMax);
                Assert.IsTrue(IsPrime((int)foundP));
                Assert.IsTrue(IsPrime((int)foundQ));
            }
            #endregion

            #region Stress Tests
            [TestMethod]
            public void FindValidModulus_CompletesInReasonableTime()
            {
                // Arrange
                const int lengthCode = 6;
                const int numberOfCodes = 10;
                var stopwatch = new Stopwatch();

                // Act
                stopwatch.Start();
                for (int i = 0; i < 100; i++)
                {
                    CodeGenerator.FindValidModulus(lengthCode, numberOfCodes,
                        out _, out _, out _, out _);
                }
                stopwatch.Stop();

                // Assert
                Assert.IsTrue(stopwatch.ElapsedMilliseconds < 5000,
                    "Should complete 100 iterations in under 5 seconds");
            }

            [TestMethod]
            public void FindValidModulus_WithHighNumberOfCodes_StillFindsValidModulus()
            {
                // Arrange
                const int lengthCode = 6;
                const int numberOfCodes = 10_000;

                // Act
                var m = CodeGenerator.FindValidModulus(lengthCode, numberOfCodes,
                    out long phi, out _, out _, out _);

                // Assert
                Assert.IsTrue(phi > 2 * numberOfCodes,
                    $"phi ({phi}) should be greater than {2 * numberOfCodes}");
            }
            #endregion

            #region Internal Logic Tests
            [TestMethod]
            public void FindValidModulus_GeneratedPrimes_AreValidAndCoprime()
            {
                // Arrange
                const int lengthCode = 6;
                const int numberOfCodes = 10;
                var primePairs = new List<(long p, long q)>();

                // Act
                for (int i = 0; i < TestIterations; i++)
                {
                    CodeGenerator.FindValidModulus(lengthCode, numberOfCodes,
                        out _, out _, out long p, out long q);
                    primePairs.Add((p, q));
                }

                // Assert
                foreach (var (p, q) in primePairs)
                {
                    Assert.IsTrue(IsPrime((int)p), $"{p} should be prime");
                    Assert.IsTrue(IsPrime((int)q), $"{q} should be prime");
                    Assert.IsTrue(CodeGenerator.MutualSimplicity((int)p, (int)q),
                        $"{p} and {q} should be coprime");
                }
            }

            [TestMethod]
            public void FindValidModulus_PhiCalculation_IsCorrect()
            {
                // Arrange
                const int lengthCode = 6;
                const int numberOfCodes = 10;

                // Act
                CodeGenerator.FindValidModulus(lengthCode, numberOfCodes,
                    out long phi, out _, out long p, out long q);

                // Assert
                long expectedPhi = (p - 1) * (q - 1);
                Assert.AreEqual(expectedPhi, phi,
                    $"phi should equal ({p}-1)*({q}-1) = {expectedPhi}");
            }

            [TestMethod]
            public void FindValidModulus_PhiDivisors_AreValid()
            {
                // Arrange
                const int lengthCode = 6;
                const int numberOfCodes = 10;

                // Act
                CodeGenerator.FindValidModulus(lengthCode, numberOfCodes,
                    out long phi, out var divisors, out _, out _);

                // Assert
                foreach (var divisor in divisors)
                {
                    Assert.AreEqual(0, phi % divisor,
                        $"{divisor} should divide {phi} without remainder");
                }
            }
            #endregion

            #region Helper Methods
            private static bool IsPrime(int number)
            {
                if (number <= 1) return false;
                if (number == 2) return true;
                if (number % 2 == 0) return false;

                var boundary = (int)Math.Floor(Math.Sqrt(number));

                for (int i = 3; i <= boundary; i += 2)
                    if (number % i == 0)
                        return false;

                return true;
            }
            #endregion
        }

        [TestMethod]
        public void GenerateFirst2NPrimes_LargeInput_ReturnsCorrectNumberOfPrimes()
        {
            // Arrange
            const int input = 100;

            // Act
            var result = CodeGenerator.GenerateFirst2NPrimes(input);

            // Assert
            Assert.AreEqual(2 * input, result.Length);
            Assert.IsTrue(result.All(IsPrime), "All returned numbers should be prime");
            Assert.IsTrue(result.SequenceEqual(result.OrderBy(x => x)),
                "Primes should be in ascending order");
        }
        #endregion

        #region SelectRandomPrimeP Tests
        [TestMethod]
        public void SelectRandomPrimeP_ValidInput_ReturnsPrimeFromGeneratedSet()
        {
            // Arrange
            const int numberOfCodes = 10;
            var primes = CodeGenerator.GenerateFirst2NPrimes(numberOfCodes);

            // Act & Assert
            for (int i = 0; i < TestIterations; i++)
            {
                int prime = CodeGenerator.SelectRandomPrimeP(numberOfCodes);
                CollectionAssert.Contains(primes, prime);
            }
        }

        [TestMethod]
        public void SelectRandomPrimeP_MultipleCalls_ReturnsDifferentPrimes()
        {
            // Arrange
            const int numberOfCodes = 10;
            var results = new HashSet<int>();

            // Act
            for (int i = 0; i < TestIterations; i++)
            {
                results.Add(CodeGenerator.SelectRandomPrimeP(numberOfCodes));
            }

            // Assert
            Assert.IsTrue(results.Count > 1, "Should return different primes on multiple calls");
        }
        #endregion

        #region GeneratePrimesBG Tests
        [TestMethod]
        public void GeneratePrimesBG_ValidInput_ReturnsDifferentCoprimePrimes()
        {
            // Arrange
            const int numberOfCodes = 5;

            // Act & Assert
            for (int i = 0; i < TestIterations; i++)
            {
                var (b, g) = CodeGenerator.GeneratePrimesBG(numberOfCodes);
                Assert.AreNotEqual(b, g, "B and G should be different");
                Assert.IsTrue(CodeGenerator.MutualSimplicity(b, g), "B and G should be coprime");
                Assert.IsTrue(IsPrime(b), "B should be prime");
                Assert.IsTrue(IsPrime(g), "G should be prime");
            }
        }

        [TestMethod]
        public void GeneratePrimesBG_PhiGCondition_ReturnsValidPair()
        {
            // Arrange
            const int numberOfCodes = 10;

            // Act & Assert
            for (int i = 0; i < TestIterations; i++)
            {
                var (_, g) = CodeGenerator.GeneratePrimesBG(numberOfCodes);
                long phiG = g - 1;
                Assert.IsTrue(phiG > 2 * numberOfCodes,
                    $"phi(g) should be greater than {2 * numberOfCodes}");
            }
        }
        #endregion

        #region CalculateKValues Tests
        [TestMethod]
        public void CalculateKValues_ValidInput_ReturnsUniqueValues()
        {
            // Arrange
            const int b = 5, g = 23, count = 5;

            // Act
            var kValues = CodeGenerator.CalculateKValues(b, g, count);

            // Assert
            Assert.AreEqual(count, kValues.Count, "Should return requested count of values");
            Assert.AreEqual(count, kValues.Distinct().Count(), "All values should be unique");
            Assert.IsTrue(kValues.All(k => k >= 1 && k < g),
                "All values should be in correct range");
        }

        [TestMethod]
        public void CalculateKValues_LargeCount_ReturnsCorrectNumberOfValues()
        {
            // Arrange
            const int b = 7, g = 101, count = 50;

            // Act
            var kValues = CodeGenerator.CalculateKValues(b, g, count);

            // Assert
            Assert.AreEqual(count, kValues.Count);
            Assert.AreEqual(count, kValues.Distinct().Count());
        }
        #endregion

        #region FindPrimeByBruteForce Tests
        [TestMethod]
        public void FindPrimeByBruteForce_ValidRange_ReturnsPrimeInRange()
        {
            // Arrange
            const int min = 100, max = 200;

            // Act & Assert
            for (int i = 0; i < TestIterations; i++)
            {
                int prime = CodeGenerator.FindPrimeByBruteForce(min, max);
                Assert.IsTrue(IsPrime(prime), $"{prime} should be prime");
                Assert.IsTrue(prime >= min && prime < max,
                    $"{prime} should be in range [{min}, {max})");
            }
        }

        [TestMethod]
        public void FindPrimeByBruteForce_SmallRange_ReturnsValidPrime()
        {
            // Arrange
            const int min = 2;
            const int max = 4; // Увеличиваем диапазон, чтобы было несколько простых чисел

            // Act
            int prime = CodeGenerator.FindPrimeByBruteForce(min, max);

            // Assert - проверяем, что результат простой и входит в диапазон
            Assert.IsTrue(prime >= min && prime < max, $"Prime {prime} should be in range [{min}, {max})");
            Assert.IsTrue(IsPrime(prime), $"Number {prime} should be prime");
        }
        #endregion

        #region MutualSimplicity Tests
        [TestMethod]
        public void MutualSimplicity_PrimeNumbers_ReturnsTrue()
        {
            // Arrange
            var testCases = new[]
            {
                (a: 5, b: 7),
                (a: 11, b: 13),
                (a: 17, b: 19)
            };

            // Act & Assert
            foreach (var (a, b) in testCases)
            {
                Assert.IsTrue(CodeGenerator.MutualSimplicity(a, b),
                    $"{a} and {b} should be coprime");
            }
        }

        [TestMethod]
        public void MutualSimplicity_NonCoprimeNumbers_ReturnsFalse()
        {
            // Arrange
            var testCases = new[]
            {
                (a: 4, b: 6),
                (a: 15, b: 20),
                (a: 8, b: 12)
            };

            // Act & Assert
            foreach (var (a, b) in testCases)
            {
                Assert.IsFalse(CodeGenerator.MutualSimplicity(a, b),
                    $"{a} and {b} should not be coprime");
            }
        }

        [TestMethod]
        public void MutualSimplicity_EdgeCases_HandlesCorrectly()
        {
            // Arrange & Act & Assert
            Assert.IsTrue(CodeGenerator.MutualSimplicity(1, 5));
            Assert.IsTrue(CodeGenerator.MutualSimplicity(0, 1));
            Assert.IsFalse(CodeGenerator.MutualSimplicity(0, 0));
            Assert.IsFalse(CodeGenerator.MutualSimplicity(2, 0));
        }
        #endregion

        #region FindValidModulus Tests
        [TestMethod]
        public void FindValidModulus_ValidInput_ReturnsCorrectModulus()
        {
            // Arrange
            const int lengthCode = 6;
            const int numberOfCodes = 10;
            long mMin = (long)Math.Pow(10, lengthCode - 1);
            long mMax = (long)Math.Pow(10, lengthCode) - 1;

            // Act
            long m = CodeGenerator.FindValidModulus(lengthCode, numberOfCodes,
                out long phi, out var divisors, out long p, out long q);

            // Assert
            Assert.IsTrue(m >= mMin && m <= mMax, $"m ({m}) should be in range [{mMin}, {mMax}]");
            Assert.IsTrue(phi > 2 * numberOfCodes, $"phi ({phi}) should be > {2 * numberOfCodes}");
            Assert.IsTrue(divisors.Count > 0, "Should return non-empty divisors list");
            Assert.IsTrue(IsPrime((int)p), "p should be prime");
            Assert.IsTrue(IsPrime((int)q), "q should be prime");
            Assert.AreEqual(m, p * q, "m should equal p * q");
            Assert.IsTrue(CodeGenerator.MutualSimplicity((int)p, (int)q), "p and q should be coprime");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FindValidModulus_InvalidLengthCode_ThrowsException()
        {
            // Arrange
            const int invalidLength = 5;
            const int numberOfCodes = 10;

            // Act
            CodeGenerator.FindValidModulus(invalidLength, numberOfCodes,
                out _, out _, out _, out _);
        }
        #endregion

        #region FindAllDivisors Tests
        [TestMethod]
        public void FindAllDivisors_ValidNumber_ReturnsAllDivisors()
        {
            // Arrange
            var testCases = new[]
            {
                (n: 12L, expected: new List<long> { 1, 2, 3, 4, 6, 12 }),
                (n: 17L, expected: new List<long> { 1, 17 }),
                (n: 36L, expected: new List<long> { 1, 2, 3, 4, 6, 9, 12, 18, 36 })
            };

            // Act & Assert
            foreach (var (n, expected) in testCases)
            {
                var result = CodeGenerator.FindAllDivisors(n);
                CollectionAssert.AreEqual(expected, result, $"Failed for n = {n}");
            }
        }

        [TestMethod]
        public void FindAllDivisors_EdgeCases_HandlesCorrectly()
        {
            // Arrange & Act & Assert
            var result1 = CodeGenerator.FindAllDivisors(1);
            CollectionAssert.AreEqual(new List<long> { 1 }, result1);

            var result0 = CodeGenerator.FindAllDivisors(0);
            Assert.AreEqual(0, result0.Count);

            var resultPrime = CodeGenerator.FindAllDivisors(13);
            CollectionAssert.AreEqual(new List<long> { 1, 13 }, resultPrime);
        }
        #endregion

        #region PowMod Tests
        [TestMethod]
        public void PowMod_ValidInput_ReturnsCorrectResult()
        {
            // Arrange
            var testCases = new[]
            {
                (a: 2L, k: 3L, n: 5L, expected: 3L),
                (a: 5L, k: 3L, n: 13L, expected: 8L),
                (a: 10L, k: 5L, n: 17L, expected: 6L)
            };

            // Act & Assert
            foreach (var (a, k, n, expected) in testCases)
            {
                var result = CodeGenerator.PowMod(a, k, n);
                Assert.AreEqual(expected, result,
                    $"{a}^{k} mod {n} should equal {expected}");
            }
        }

        [TestMethod]
        public void PowMod_LargeNumbers_ReturnsCorrectResult()
        {
            // Arrange
            long a = 123456789;
            long k = 10;
            long n = 1000000007;

            // Act
            long result = CodeGenerator.PowMod(a, k, n);

            // Assert - проверяем, что результат корректен математически
            Assert.IsTrue(result >= 0 && result < n,
                $"Result {result} should be in range [0, {n})");

            // Проверяем вычислением шаг за шагом
            long expected = 1;
            for (int i = 0; i < k; i++)
            {
                expected = (expected * a) % n;
            }

            Assert.AreEqual(expected, result,
                $"Expected {a}^{k} mod {n} = {expected}, but got {result}");
        }
        #endregion

        #region GenerateCodes Tests
        [TestMethod]
        public void GenerateCodes_ValidInput_ReturnsValidCodes()
        {
            // Arrange
            const int lengthCode = 6;
            const int numberOfCodes = 5;
            long minValue = (long)Math.Pow(10, lengthCode - 1);
            long maxValue = (long)Math.Pow(10, lengthCode) - 1;

            // Act
            var codes = CodeGenerator.GenerateCodes(lengthCode, numberOfCodes,
                out long m, out long a, out long p, out long q,
                out List<long> usedK, out int b, out int g);

            // Assert
            Assert.AreEqual(numberOfCodes, codes.Count,
                $"Should return {numberOfCodes} codes");
            Assert.IsTrue(codes.All(code => code >= minValue && code <= maxValue),
                "All codes should be in valid range");
            Assert.AreEqual(numberOfCodes, usedK.Count,
                "Should use same number of K values");
            Assert.IsTrue(CodeGenerator.MutualSimplicity((int)a, (int)m),
                "a and m should be coprime");
            Assert.IsTrue(IsPrime(b), "b should be prime");
            Assert.IsTrue(IsPrime(g), "g should be prime");
            Assert.AreEqual(m, p * q, "m should equal p * q");
        }

        [TestMethod]
        public void GenerateCodes_MultipleCalls_ReturnsDifferentCodes()
        {
            // Arrange
            const int lengthCode = 6;
            const int numberOfCodes = 5;
            var results = new HashSet<long>();

            // Act
            for (int i = 0; i < 10; i++)
            {
                var codes = CodeGenerator.GenerateCodes(lengthCode, numberOfCodes,
                    out _, out _, out _, out _, out _, out _, out _);
                foreach (var code in codes)
                {
                    results.Add(code);
                }
            }

            // Assert
            Assert.IsTrue(results.Count > numberOfCodes,
                "Should generate different codes on multiple calls");
        }
        #endregion

        #region FindOptimizedModulus Tests
        [TestMethod]
        public void FindOptimizedModulus_ValidInput_ReturnsUniqueModulus()
        {
            // Arrange
            const int lengthCode = 6;
            const int numberOfCodes = 10;
            var checkedM = new HashSet<long> { 123456, 654321 };

            // Act
            long m = CodeGenerator.FindOptimizedModulus(lengthCode, numberOfCodes, checkedM);

            // Assert
            Assert.IsFalse(checkedM.Contains(m), "Should return modulus not in checked set");
            Assert.IsTrue(m >= 100000 && m <= 999999, "Should return 6-digit modulus");
        }

        [TestMethod]
        public void FindOptimizedModulus_ExhaustedAttempts_StillReturnsValidModulus()
        {
            // Arrange
            const int lengthCode = 6;
            const int numberOfCodes = 10;
            var checkedM = new HashSet<long>(Enumerable.Range(100000, 900000).Select(x => (long)x));

            // Act
            long m = CodeGenerator.FindOptimizedModulus(lengthCode, numberOfCodes, checkedM);

            // Assert
            Assert.IsTrue(m >= 100000 && m <= 999999, "Should return valid modulus");
        }
        #endregion

        #region GenerateOptimizedPrime Tests
        [TestMethod]
        public void GenerateOptimizedPrime_ValidInput_ReturnsUniquePrime()
        {
            // Arrange
            const int lengthCode = 6;
            const long minValue = 100000;
            const long maxValue = 999999;
            var checkedA = new HashSet<long> { 100003, 100019 };

            // Act
            long prime = CodeGenerator.GenerateOptimizedPrime(lengthCode, minValue, maxValue, checkedA);

            // Assert
            Assert.IsFalse(checkedA.Contains(prime), "Should return prime not in checked set");
            Assert.IsTrue(prime >= minValue && prime <= maxValue, "Should be in range");
            Assert.IsTrue(IsPrime((int)prime), "Should return prime number");
        }

        [TestMethod]
        public void GenerateOptimizedPrime_AllAttemptsExhausted_ReturnsValidPrime()
        {
            // Arrange
            const int lengthCode = 6;
            const long minValue = 100000;
            const long maxValue = 100100;
            var primesInRange = Enumerable.Range((int)minValue, (int)(maxValue - minValue + 1))
                .Where(IsPrime)
                .Select(x => (long)x)
                .ToList();

            if (!primesInRange.Any())
                Assert.Inconclusive("No primes in test range");

            var checkedA = new HashSet<long>(primesInRange);

            // Act
            long prime = CodeGenerator.GenerateOptimizedPrime(lengthCode, minValue, maxValue, checkedA);

            // Assert
            Assert.IsTrue(IsPrime((int)prime), "Should return prime number");
            Assert.AreEqual(lengthCode, prime.ToString().Length, "Should have correct length");
        }

        [TestMethod]
        public void GenerateOptimizedPrime_DifferentLengths_ReturnsCorrectPrimes()
        {
            // Arrange
            var testCases = new[]
            {
                (length: 6, min: 100000L, max: 999999L),
                (length: 7, min: 1000000L, max: 9999999L),
                (length: 8, min: 10000000L, max: 99999999L),
                (length: 9, min: 100000000L, max: 999999999L)
            };

            foreach (var (length, min, max) in testCases)
            {
                // Act
                long prime = CodeGenerator.GenerateOptimizedPrime(length, min, max, new HashSet<long>());

                // Assert
                Assert.IsTrue(prime >= min && prime <= max, $"For length {length}");
                Assert.IsTrue(IsPrime((int)prime), $"For length {length}");
                Assert.AreEqual(length, prime.ToString().Length, $"For length {length}");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GenerateOptimizedPrime_InvalidLength_ThrowsException()
        {
            // Arrange
            const int invalidLength = 10;
            const long minValue = 1000000000;
            const long maxValue = 9999999999;
            var checkedA = new HashSet<long>();

            // Act
            CodeGenerator.GenerateOptimizedPrime(invalidLength, minValue, maxValue, checkedA);
        }
        #endregion

        #region GeneratePrime Tests
        [TestMethod]
        public void GeneratePrime_ValidLength_ReturnsCorrectPrime()
        {
            // Arrange
            var testCases = new[]
            {
                (length: 6, min: 100000L, max: 999999L),
                (length: 7, min: 1000000L, max: 9999999L),
                (length: 8, min: 10000000L, max: 99999999L),
                (length: 9, min: 100000000L, max: 999999999L)
            };

            foreach (var (length, min, max) in testCases)
            {
                // Act
                long prime = CodeGenerator.GeneratePrime(length);

                // Assert
                Assert.IsTrue(prime >= min && prime <= max, $"For length {length}");
                Assert.IsTrue(IsPrime((int)prime), $"For length {length}");
                Assert.AreEqual(length, prime.ToString().Length, $"For length {length}");
            }
        }

        [TestMethod]
        public void PowMod_ZeroExponent_ReturnsOne()
        {
            // Arrange
            long a = 123, k = 0, n = 456;

            // Act
            var result = CodeGenerator.PowMod(a, k, n);

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void GenerateCodes_InvalidLength_ThrowsException()
        {
            // Arrange
            int invalidLength = 5;
            int numberOfCodes = 10;

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                CodeGenerator.GenerateCodes(invalidLength, numberOfCodes,
                    out _, out _, out _, out _, out _, out _, out _));
        }

        [TestMethod]
        public void TryGenerateCodes_InvalidParameters_ReturnsNull()
        {
            // Arrange
            long a = 1, m = 2; // Некорректные параметры
            int lengthCode = 6, numberOfCodes = 5;
            long minValue = 100000, maxValue = 999999;

            // Act
            var result = CodeGenerator.TryGenerateCodes(a, m, lengthCode,
                numberOfCodes, minValue, maxValue);

            // Assert
            Assert.IsNull(result.codes);
        }

        [TestMethod]
        public void FindOptimizedModulus_AllAttemptsExhausted_ReturnsValidModulus()
        {
            // Arrange
            int lengthCode = 6;
            int numberOfCodes = 10;
            var checkedM = new HashSet<long>(Enumerable.Range(100000, 900000).Select(x => (long)x));

            // Act
            long m = CodeGenerator.FindOptimizedModulus(lengthCode, numberOfCodes, checkedM);

            // Assert
            Assert.IsTrue(m >= 100000 && m <= 999999);
        }

        [TestMethod]
        public void GenerateOptimizedPrime_OverrideGenerator_UsesCustomGenerator()
        {
            // Arrange
            int lengthCode = 6;
            long minValue = 100000, maxValue = 999999;
            var checkedA = new HashSet<long>();
            long expectedPrime = 100003;

            // Act
            long prime = CodeGenerator.GenerateOptimizedPrime(lengthCode, minValue, maxValue,
                checkedA, _ => expectedPrime);

            // Assert
            Assert.AreEqual(expectedPrime, prime);
        }

        [TestMethod]
        public void GenerateFirst2NPrimes_ExtremeInput_Coverage()
        {
            // Test large input
            var primes = CodeGenerator.GenerateFirst2NPrimes(1000);
            Assert.AreEqual(2000, primes.Length);
            Assert.IsTrue(primes.All(p => IsPrime(p)));
        }

        [TestMethod]
        public void MutualSimplicity_SameNumber_ReturnsTrueForOne()
        {
            Assert.IsTrue(CodeGenerator.MutualSimplicity(1, 1));
            Assert.IsFalse(CodeGenerator.MutualSimplicity(2, 2));
        }

        [TestMethod]
        public void FindAllDivisors_PrimeInput_ReturnsCorrectDivisors()
        {
            var divisors = CodeGenerator.FindAllDivisors(13);
            CollectionAssert.AreEqual(new List<long> { 1, 13 }, divisors);
        }

        [TestMethod]
        public void PowMod_ZeroModulus_ThrowsException()
        {
            Assert.ThrowsException<DivideByZeroException>(() =>
                CodeGenerator.PowMod(2, 3, 0));
        }
        #endregion
        #region GenerateFirst2NPrimes Additional Tests
        [TestMethod]
        public void GenerateFirst2NPrimes_InputOne_ReturnsFirstTwoPrimes()
        {
            // Arrange & Act
            var result = CodeGenerator.GenerateFirst2NPrimes(1);

            // Assert
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(2, result[0]);
            Assert.AreEqual(3, result[1]);
        }

        [TestMethod]
        public void GenerateFirst2NPrimes_InputTwo_ReturnsFirstFourPrimes()
        {
            // Arrange & Act
            var result = CodeGenerator.GenerateFirst2NPrimes(2);

            // Assert
            Assert.AreEqual(4, result.Length);
            CollectionAssert.AreEqual(new[] { 2, 3, 5, 7 }, result);
        }
        #endregion

        #region SelectRandomPrimeP Additional Tests
        [TestMethod]
        public void SelectRandomPrimeP_AlwaysReturnsPrimeFromGeneratedSet()
        {
            // Arrange
            const int numberOfCodes = 5;
            var primes = CodeGenerator.GenerateFirst2NPrimes(numberOfCodes);
            var results = new HashSet<int>();

            // Act
            for (int i = 0; i < 100; i++)
            {
                results.Add(CodeGenerator.SelectRandomPrimeP(numberOfCodes));
            }

            // Assert
            Assert.IsTrue(results.All(p => primes.Contains(p)));
            Assert.IsTrue(results.Count > 1, "Should return different primes");
        }
        #endregion

        #region GeneratePrimesBG Additional Tests
        [TestMethod]
        public void GeneratePrimesBG_AlwaysReturnsDifferentCoprimePrimes()
        {
            // Arrange
            const int numberOfCodes = 5;
            var results = new HashSet<(int, int)>();

            // Act
            for (int i = 0; i < 100; i++)
            {
                var (b, g) = CodeGenerator.GeneratePrimesBG(numberOfCodes);
                results.Add((b, g));
            }

            // Assert
            Assert.IsTrue(results.All(pair => pair.Item1 != pair.Item2));
            Assert.IsTrue(results.All(pair => CodeGenerator.MutualSimplicity(pair.Item1, pair.Item2)));
            Assert.IsTrue(results.Count > 1, "Should return different pairs");
        }
        #endregion

        #region CalculateKValues Additional Tests
        

        [TestMethod]
        public void CalculateKValues_WhenCountIsZero_ReturnsEmptyList()
        {
            // Arrange
            int b = 3, g = 7, count = 0;

            // Act
            var result = CodeGenerator.CalculateKValues(b, g, count);

            // Assert
            Assert.AreEqual(0, result.Count);
        }
        #endregion

        #region FindPrimeByBruteForce Additional Tests
        [TestMethod]
        public void FindPrimeByBruteForce_WhenRangeHasSinglePrime_ReturnsThatPrime()
        {
            // Arrange
            int min = 2, max = 4;

            // Act
            int result = CodeGenerator.FindPrimeByBruteForce(min, max);

            // Assert
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void FindPrimeByBruteForce_WhenRangeIsSmall_ReturnsValidPrime()
        {
            // Arrange
            int min = 10, max = 12;

            // Act
            int result = CodeGenerator.FindPrimeByBruteForce(min, max);

            // Assert
            Assert.AreEqual(11, result);
        }
        #endregion

        #region FindValidModulus Additional Tests
        [TestMethod]
        public void FindValidModulus_WhenCalledMultipleTimes_ReturnsDifferentValues()
        {
            // Arrange
            const int lengthCode = 6;
            const int numberOfCodes = 10;
            var results = new HashSet<long>();

            // Act
            for (int i = 0; i < 10; i++)
            {
                long m = CodeGenerator.FindValidModulus(lengthCode, numberOfCodes,
                    out _, out _, out _, out _);
                results.Add(m);
            }

            // Assert
            Assert.IsTrue(results.Count > 1, "Should return different moduli");
        }

        [TestMethod]
        public void FindValidModulus_ResultHasCorrectLength()
        {
            // Arrange
            const int lengthCode = 7;
            const int numberOfCodes = 10;

            // Act
            long m = CodeGenerator.FindValidModulus(lengthCode, numberOfCodes,
                out _, out _, out _, out _);

            // Assert
            Assert.IsTrue(m.ToString().Length == lengthCode);
        }
        #endregion

        #region GenerateCodes Additional Tests
        [TestMethod]
        public void GenerateCodes_WhenCalledMultipleTimes_ReturnsDifferentCodes()
        {
            // Arrange
            const int lengthCode = 6;
            const int numberOfCodes = 5;
            var allCodes = new HashSet<long>();

            // Act
            for (int i = 0; i < 10; i++)
            {
                var codes = CodeGenerator.GenerateCodes(lengthCode, numberOfCodes,
                    out _, out _, out _, out _, out _, out _, out _);
                foreach (var code in codes)
                {
                    allCodes.Add(code);
                }
            }

            // Assert
            Assert.IsTrue(allCodes.Count > numberOfCodes);
        }

        [TestMethod]
        public void GenerateCodes_AllCodesHaveCorrectLength()
        {
            // Arrange
            const int lengthCode = 8;
            const int numberOfCodes = 10;

            // Act
            var codes = CodeGenerator.GenerateCodes(lengthCode, numberOfCodes,
                out _, out _, out _, out _, out _, out _, out _);

            // Assert
            Assert.IsTrue(codes.All(c => c.ToString().Length == lengthCode));
        }
        #endregion

        #region TryGenerateCodes Additional Tests
        [TestMethod]
        public void TryGenerateCodes_WithInvalidParameters_ReturnsNull()
        {
            // Arrange
            long a = 1, m = 2; // Invalid parameters
            int lengthCode = 6, numberOfCodes = 5;
            long minValue = 100000, maxValue = 999999;

            // Act
            var result = CodeGenerator.TryGenerateCodes(a, m, lengthCode,
                numberOfCodes, minValue, maxValue);

            // Assert
            Assert.IsNull(result.codes);
        }

        [TestMethod]
        public void TryGenerateCodes_WithValidParameters_ReturnsCodes()
        {
            // Arrange
            long a = 100003, m = 999983;
            int lengthCode = 6, numberOfCodes = 5;
            long minValue = 100000, maxValue = 999999;

            // Act
            var result = CodeGenerator.TryGenerateCodes(a, m, lengthCode,
                numberOfCodes, minValue, maxValue);

            // Assert
            Assert.IsNotNull(result.codes);
            Assert.AreEqual(numberOfCodes, result.codes.Count);
        }
        #endregion

        #region GenerateOptimizedPrime Additional Tests
        [TestMethod]
        public void GenerateOptimizedPrime_WithOverrideGenerator_UsesCustomGenerator()
        {
            // Arrange
            int lengthCode = 6;
            long minValue = 100000, maxValue = 999999;
            var checkedA = new HashSet<long>();
            long expectedPrime = 100003;

            // Act
            long prime = CodeGenerator.GenerateOptimizedPrime(lengthCode, minValue, maxValue,
                checkedA, _ => expectedPrime);

            // Assert
            Assert.AreEqual(expectedPrime, prime);
        }

        [TestMethod]
        public void GenerateOptimizedPrime_WhenAllPrimesChecked_StillReturnsPrime()
        {
            // Arrange
            int lengthCode = 6;
            long minValue = 100000, maxValue = 100100;
            var primesInRange = Enumerable.Range((int)minValue, (int)(maxValue - minValue + 1))
                .Where(IsPrime)
                .Select(x => (long)x)
                .ToList();
            var checkedA = new HashSet<long>(primesInRange);

            // Act
            long prime = CodeGenerator.GenerateOptimizedPrime(lengthCode, minValue, maxValue, checkedA);

            // Assert
            Assert.IsTrue(IsPrime((int)prime));
        }
        #endregion

        #region Edge Case Tests
        

        [TestMethod]
        public void PowMod_ZeroBase_ReturnsZero()
        {
            // Arrange
            long a = 0, k = 5, n = 10;

            // Act
            var result = CodeGenerator.PowMod(a, k, n);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void MutualSimplicity_OneAndAnyNumber_ReturnsTrue()
        {
            // Arrange & Act & Assert
            Assert.IsTrue(CodeGenerator.MutualSimplicity(1, 5));
            Assert.IsTrue(CodeGenerator.MutualSimplicity(1, 10));
            Assert.IsTrue(CodeGenerator.MutualSimplicity(1, 1));
        }
        #endregion

        #region Valid Input Tests
        [TestMethod]
        public void GeneratePrime_Length6_Returns6DigitPrime()
        {
            // Arrange
            const int length = 6;

            // Act
            long prime = CodeGenerator.GeneratePrime(length);

            // Assert
            Assert.IsTrue(prime >= 100000 && prime <= 999999);
            Assert.IsTrue(IsPrime((int)prime));
            Assert.AreEqual(length, prime.ToString().Length);
        }

        [TestMethod]
        public void GeneratePrime_Length7_Returns7DigitPrime()
        {
            // Arrange
            const int length = 7;

            // Act
            long prime = CodeGenerator.GeneratePrime(length);

            // Assert
            Assert.IsTrue(prime >= 1000000 && prime <= 9999999);
            Assert.IsTrue(IsPrime((int)prime));
            Assert.AreEqual(length, prime.ToString().Length);
        }

        [TestMethod]
        public void GeneratePrime_Length8_Returns8DigitPrime()
        {
            // Arrange
            const int length = 8;

            // Act
            long prime = CodeGenerator.GeneratePrime(length);

            // Assert
            Assert.IsTrue(prime >= 10000000 && prime <= 99999999);
            Assert.IsTrue(IsPrime((int)prime));
            Assert.AreEqual(length, prime.ToString().Length);
        }

        [TestMethod]
        public void GeneratePrime_Length9_Returns9DigitPrime()
        {
            // Arrange
            const int length = 9;

            // Act
            long prime = CodeGenerator.GeneratePrime(length);

            // Assert
            Assert.IsTrue(prime >= 100000000 && prime <= 999999999);
            Assert.IsTrue(IsPrime((int)prime));
            Assert.AreEqual(length, prime.ToString().Length);
        }
        #endregion

      

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GeneratePrime_Length10_ThrowsArgumentException()
        {
            // Arrange
            const int invalidLength = 10;

            // Act
            CodeGenerator.GeneratePrime(invalidLength);
        }
        

        #region Stress Tests
        [TestMethod]
        public void GeneratePrime_MultipleCalls_ReturnsDifferentPrimes()
        {
            // Arrange
            const int length = 6;
            var primes = new HashSet<long>();
            const int iterations = 10;

            // Act
            for (int i = 0; i < iterations; i++)
            {
                primes.Add(CodeGenerator.GeneratePrime(length));
            }

            // Assert
            Assert.IsTrue(primes.Count > 1, "Should return different primes on multiple calls");
        }

        [TestMethod]
        public void GeneratePrime_AllPrimesInRangeAreValid()
        {
            // Arrange
            const int length = 6;
            const int iterations = 100;

            // Act & Assert
            for (int i = 0; i < iterations; i++)
            {
                long prime = CodeGenerator.GeneratePrime(length);
                Assert.IsTrue(IsPrime((int)prime), $"Generated number {prime} is not prime");
            }
        }
        #endregion


    }
}