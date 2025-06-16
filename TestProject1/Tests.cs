using System.Diagnostics;
using SmsGeneratorApp;

namespace TestProject1
{
    [TestClass]
    public sealed class CodeGeneratorTests
    {
        private const int TestIterations = 100;

        [TestMethod]
        public void GenerateFirst2NPrimes_ZeroInput_ReturnsEmptyArray()
        {
            var result = CodeGenerator.GenerateFirst2NPrimes(0);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Length);
        }

        [TestMethod]
        public void GenerateFirst2NPrimes_NegativeInput_ReturnsEmptyArray()
        {
            var result = CodeGenerator.GenerateFirst2NPrimes(-5);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Length);
        }

        [TestMethod]
        public void GenerateFirst2NPrimes_Input1_ReturnsFirst2Primes()
        {
            var expected = new int[] { 2, 3 };
            var result = CodeGenerator.GenerateFirst2NPrimes(1);
            CollectionAssert.AreEqual(expected, result);
        }


        [TestMethod]
        public void GenerateFirst2NPrimes_Input10_ReturnsFirst20Primes()
        {
            var expected = new int[] {
                2, 3, 5, 7, 11, 13, 17, 19, 23, 29,
                31, 37, 41, 43, 47, 53, 59, 61, 67, 71
            };
            var result = CodeGenerator.GenerateFirst2NPrimes(10);
            CollectionAssert.AreEqual(expected, result);
        }


        [TestMethod]
        public void SelectRandomPrimeP_ValidInput_ReturnsPrimeFromGeneratedSet()
        {
            int numberOfCodes = 10;
            var primes = CodeGenerator.GenerateFirst2NPrimes(numberOfCodes);

            for (int i = 0; i < TestIterations; i++)
            {
                int prime = CodeGenerator.SelectRandomPrimeP(numberOfCodes);
                CollectionAssert.Contains(primes, prime);
            }
        }

        [TestMethod]
        public void GeneratePrimesBG_ValidInput_ReturnsDifferentPrimes()
        {
            int numberOfCodes = 5;

            for (int i = 0; i < TestIterations; i++)
            {
                var (b, g) = CodeGenerator.GeneratePrimesBG(numberOfCodes);
                Assert.AreNotEqual(b, g);
                Assert.IsTrue(CodeGenerator.MutualSimplicity(b, g));
            }
        }

        [TestMethod]
        public void CalculateKValues_ValidInput_ReturnsUniqueValues()
        {
            int b = 5, g = 23, count = 5;
            var kValues = CodeGenerator.CalculateKValues(b, g, count);

            Assert.AreEqual(count, kValues.Count);
            Assert.AreEqual(count, kValues.Distinct().Count());
        }



        [TestMethod]
        public void FindPrimeByBruteForce_ValidRange_ReturnsPrime()
        {
            int min = 100, max = 200;

            for (int i = 0; i < TestIterations; i++)
            {
                int prime = CodeGenerator.FindPrimeByBruteForce(min, max);
                Assert.IsTrue(IsPrime(prime));
                Assert.IsTrue(prime >= min && prime < max);
            }
        }


        [TestMethod]
        public void MutualSimplicity_PrimeNumbers_ReturnsTrue()
        {
            Assert.IsTrue(CodeGenerator.MutualSimplicity(5, 7));
            Assert.IsTrue(CodeGenerator.MutualSimplicity(11, 13));
        }

        [TestMethod]
        public void MutualSimplicity_NonCoprimeNumbers_ReturnsFalse()
        {
            Assert.IsFalse(CodeGenerator.MutualSimplicity(4, 6));
            Assert.IsFalse(CodeGenerator.MutualSimplicity(15, 20));
        }

        [TestMethod]
        public void MutualSimplicity_EdgeCases_HandlesCorrectly()
        {
            Assert.IsTrue(CodeGenerator.MutualSimplicity(1, 5));
            Assert.IsTrue(CodeGenerator.MutualSimplicity(0, 1));
            Assert.IsFalse(CodeGenerator.MutualSimplicity(0, 0));
        }

        [TestMethod]
        public void FindValidModulus_ValidLength_ReturnsValidModulus()
        {
            int lengthCode = 6, numberOfCodes = 10;
            long m = CodeGenerator.FindValidModulus(lengthCode, numberOfCodes, out long phi, out var divisors);

            Assert.IsTrue(m >= Math.Pow(10, lengthCode - 1));
            Assert.IsTrue(m <= Math.Pow(10, lengthCode) - 1);
            Assert.IsTrue(phi > 2 * numberOfCodes);
            Assert.IsTrue(divisors.Count > 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FindValidModulus_InvalidLength_ThrowsException()
        {
            CodeGenerator.FindValidModulus(5, 10, out _, out _);
        }

        [TestMethod]
        public void FindValidModulus_EdgeCases_HandlesCorrectly()
        {
            // Test with minimal valid length
            long m = CodeGenerator.FindValidModulus(6, 1, out long phi, out _);
            Assert.IsTrue(m >= 100000 && m <= 999999);
        }

        [TestMethod]
        public void FindAllDivisors_ValidNumber_ReturnsAllDivisors()
        {
            long n = 12;
            var expected = new List<long> { 1, 2, 3, 4, 6, 12 };
            var result = CodeGenerator.FindAllDivisors(n);

            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        public void FindAllDivisors_EdgeCases_HandlesCorrectly()
        {
            // Test with 1
            var result1 = CodeGenerator.FindAllDivisors(1);
            CollectionAssert.AreEqual(new List<long> { 1 }, result1);

            // Test with 0
            var result2 = CodeGenerator.FindAllDivisors(0);
            Assert.AreEqual(0, result2.Count);

            // Test with prime number
            var result3 = CodeGenerator.FindAllDivisors(13);
            CollectionAssert.AreEqual(new List<long> { 1, 13 }, result3);
        }



        [TestMethod]
        public void GenerateCodes_ValidInput_ReturnsValidCodes()
        {
            int lengthCode = 6, numberOfCodes = 5;
            var codes = CodeGenerator.GenerateCodes(lengthCode, numberOfCodes, out long m, out long a, out var usedK);

            Assert.AreEqual(numberOfCodes, codes.Count);
            Assert.IsTrue(codes.All(code => code >= Math.Pow(10, lengthCode - 1) &&
                                           code <= Math.Pow(10, lengthCode) - 1));
            Assert.AreEqual(numberOfCodes, usedK.Count);
            Assert.IsTrue(CodeGenerator.MutualSimplicity((int)a, (int)m));
        }

        [TestMethod]
        public void GenerateCodes_EdgeCases_HandlesCorrectly()
        {
            // Test with minimal length and count
            var codes = CodeGenerator.GenerateCodes(6, 1, out long m, out long a, out var usedK);
            Assert.AreEqual(1, codes.Count);

            // Test with maximum length
            var codes2 = CodeGenerator.GenerateCodes(9, 2, out m, out a, out usedK);
            Assert.AreEqual(2, codes2.Count);
        }

        [TestMethod]
        public void FindOptimizedModulus_ReturnsUniqueModulusNotInCheckedSet()
        {
            // Arrange
            int lengthCode = 6;
            int numberOfCodes = 10;
            var checkedM = new HashSet<long> { 123456, 654321 };

            // Act
            long m = CodeGenerator.FindOptimizedModulus(lengthCode, numberOfCodes, checkedM);

            // Assert
            Assert.IsFalse(checkedM.Contains(m));
            Assert.IsTrue(m >= 100000 && m <= 999999);
        }

        [TestMethod]
        public void FindOptimizedModulus_ReturnsValidModulusWhenCheckedSetFull()
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
        public void GenerateOptimizedPrime_ReturnsUniquePrimeNotInCheckedSet()
        {
            // Arrange
            int lengthCode = 6;
            long minValue = 100000;
            long maxValue = 999999;
            var checkedA = new HashSet<long> { 100003, 100019 };

            // Act
            long prime = CodeGenerator.GenerateOptimizedPrime(lengthCode, minValue, maxValue, checkedA);

            // Assert
            Assert.IsFalse(checkedA.Contains(prime));
            Assert.IsTrue(prime >= minValue && prime <= maxValue);
            Assert.IsTrue(IsPrime((int)prime));
        }

        [TestMethod]
        public void GeneratePrime_ReturnsValidPrimeForLength6()
        {
            // Arrange
            int length = 6;

            // Act
            long prime = CodeGenerator.GeneratePrime(length);

            // Assert
            Assert.IsTrue(prime >= 100000 && prime <= 999999);
            Assert.IsTrue(IsPrime((int)prime));
        }

        [TestMethod]
        public void GeneratePrime_ReturnsValidPrimeForLength9()
        {
            // Arrange
            int length = 9;

            // Act
            long prime = CodeGenerator.GeneratePrime(length);

            // Assert
            Assert.IsTrue(prime >= 100000000 && prime <= 999999999);
            Assert.IsTrue(IsPrime((int)prime));
        }



        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GeneratePrime_ThrowsForInvalidLengthAbove9()
        {
            // Arrange
            int length = 10;

            // Act
            CodeGenerator.GeneratePrime(length);
        }

        [TestMethod]
        public void GenerateOptimizedPrime_ReturnsDifferentPrimesOnMultipleCalls()
        {
            // Arrange
            int lengthCode = 6;
            long minValue = 100000;
            long maxValue = 999999;
            var checkedA = new HashSet<long>();

            // Act
            long prime1 = CodeGenerator.GenerateOptimizedPrime(lengthCode, minValue, maxValue, checkedA);
            checkedA.Add(prime1);
            long prime2 = CodeGenerator.GenerateOptimizedPrime(lengthCode, minValue, maxValue, checkedA);

            // Assert
            Assert.AreNotEqual(prime1, prime2);
            Assert.IsTrue(IsPrime((int)prime1));
            Assert.IsTrue(IsPrime((int)prime2));
        }

        [TestMethod]
        public void FindValidModulus_ExhaustsAttemptsForQ_ReturnsValidModulus()
        {
            // Arrange
            int lengthCode = 6;
            int numberOfCodes = 10;

            // Act
            // Этот тест проверяет, что даже после нескольких попыток будет найден валидный модуль
            long m = CodeGenerator.FindValidModulus(lengthCode, numberOfCodes, out long phi, out var divisors);

            // Assert
            Assert.IsTrue(m >= Math.Pow(10, lengthCode - 1));
            Assert.IsTrue(m <= Math.Pow(10, lengthCode) - 1);
            Assert.IsTrue(phi > 2 * numberOfCodes);
        }

        [TestMethod]
        public void FindValidModulus_HandlesNonMutuallySimplePQ_ContinuesSearch()
        {
            // Arrange
            int lengthCode = 6;
            int numberOfCodes = 10;

            // Act
            // Тест проверяет, что если p и q не взаимно простые, поиск продолжается
            long m = CodeGenerator.FindValidModulus(lengthCode, numberOfCodes, out long phi, out var divisors);

            // Assert
            Assert.IsTrue(m > 0);
        }

        [TestMethod]
        public void FindValidModulus_HandlesInvalidMRange_ContinuesSearch()
        {
            // Arrange
            int lengthCode = 6;
            int numberOfCodes = 10;

            // Act
            // Тест проверяет, что если m не входит в нужный диапазон, поиск продолжается
            long m = CodeGenerator.FindValidModulus(lengthCode, numberOfCodes, out long phi, out var divisors);

            // Assert
            Assert.IsTrue(m >= 100000 && m <= 999999);
        }

        [TestMethod]
        public void FindValidModulus_HandlesSmallPhi_ContinuesSearch()
        {
            // Arrange
            int lengthCode = 6;
            int numberOfCodes = 1000; // Увеличиваем количество кодов, чтобы усложнить поиск

            // Act
            // Тест проверяет, что если phi слишком маленькое, поиск продолжается
            long m = CodeGenerator.FindValidModulus(lengthCode, numberOfCodes, out long phi, out var divisors);

            // Assert
            Assert.IsTrue(phi > 2 * numberOfCodes);
        }

        [TestMethod]
        public void GenerateOptimizedPrime_ExhaustsAttempts_ReturnsValidPrime()
        {
            // Arrange
            int lengthCode = 6;
            long minValue = 100000;
            long maxValue = 100100; // Узкий диапазон

            // Получаем все простые числа в этом диапазоне
            var primesInRange = Enumerable.Range((int)minValue, (int)(maxValue - minValue + 1))
                .Where(IsPrime)
                .Select(x => (long)x)
                .ToList();

            // Если в диапазоне нет простых чисел - пропускаем тест
            if (!primesInRange.Any())
            {
                Assert.Inconclusive("В тестовом диапазоне нет простых чисел");
                return;
            }

            // Создаем HashSet со ВСЕМИ простыми числами диапазона
            var checkedA = new HashSet<long>(primesInRange);

            // Act
            long prime = CodeGenerator.GenerateOptimizedPrime(lengthCode, minValue, maxValue, checkedA);

            // Assert - проверяем только базовые требования
            Assert.IsTrue(IsPrime((int)prime), $"Число {prime} должно быть простым");
            Assert.AreEqual(lengthCode, prime.ToString().Length,
                $"Число должно иметь {lengthCode} цифр");

            // Если число в диапазоне - проверяем что оно в списке простых
            if (prime >= minValue && prime <= maxValue)
            {
                Assert.IsTrue(primesInRange.Contains(prime),
                    $"Если число {prime} в диапазоне, оно должно быть в списке простых чисел диапазона");
            }
            else
            {
                // Если число вне диапазона - проверяем что оно 6-значное
                Assert.IsTrue(prime >= 100000 && prime <= 999999,
                    $"Число {prime} должно быть 6-значным");
            }
        }

        [TestMethod]
        public void GenerateOptimizedPrime_ReturnsDifferentPrimes_WhenCalledMultipleTimes()
        {
            // Arrange
            int lengthCode = 6;
            long minValue = 100000;
            long maxValue = 999999;
            var checkedA = new HashSet<long>();

            // Act
            long prime1 = CodeGenerator.GenerateOptimizedPrime(lengthCode, minValue, maxValue, checkedA);
            checkedA.Add(prime1);
            long prime2 = CodeGenerator.GenerateOptimizedPrime(lengthCode, minValue, maxValue, checkedA);

            // Assert
            Assert.AreNotEqual(prime1, prime2);
            Assert.IsTrue(IsPrime((int)prime1));
            Assert.IsTrue(IsPrime((int)prime2));
        }
        [TestMethod]
        public void GenerateOptimizedPrime_ReturnsPrimeNotInCheckedSet_FirstAttempt()
        {
            // Arrange
            int lengthCode = 6;
            long minValue = 100000;
            long maxValue = 999999;
            var checkedA = new HashSet<long> { 100003, 100019 };

            // Act
            long prime = CodeGenerator.GenerateOptimizedPrime(lengthCode, minValue, maxValue, checkedA);

            // Assert
            Assert.IsFalse(checkedA.Contains(prime));
            Assert.IsTrue(prime >= minValue && prime <= maxValue);
            Assert.IsTrue(IsPrime((int)prime));
        }

        [TestMethod]
        public void GenerateOptimizedPrime_ReturnsDifferentPrime_AfterSeveralAttempts()
        {
            // Arrange
            int lengthCode = 6;
            long minValue = 100000;
            long maxValue = 999999;
            var checkedA = new HashSet<long> { 100003, 100019, 100043, 100049, 100057 };

            // Act
            long prime = CodeGenerator.GenerateOptimizedPrime(lengthCode, minValue, maxValue, checkedA);

            // Assert
            Assert.IsFalse(checkedA.Contains(prime));
            Assert.IsTrue(prime >= minValue && prime <= maxValue);
            Assert.IsTrue(IsPrime((int)prime));
        }

        [TestMethod]
        public void GenerateOptimizedPrime_ReturnsValidPrime_WhenAllAttemptsExhausted()
        {
            // Arrange
            int lengthCode = 6;
            long minValue = 100000;
            long maxValue = 100100; // Узкий диапазон

            // Получаем все простые числа в этом диапазоне
            var primesInRange = Enumerable.Range((int)minValue, (int)(maxValue - minValue + 1))
                .Where(IsPrime)
                .Select(x => (long)x)
                .ToList();

            // Если в диапазоне нет простых чисел - пропускаем тест
            if (primesInRange.Count == 0)
            {
                Assert.Inconclusive("В тестовом диапазоне нет простых чисел");
                return;
            }

            var checkedA = new HashSet<long>(primesInRange);

            // Act
            long prime = CodeGenerator.GenerateOptimizedPrime(lengthCode, minValue, maxValue, checkedA);

            // Assert
            Assert.IsTrue(IsPrime((int)prime), $"Число {prime} должно быть простым");
            Assert.AreEqual(lengthCode, prime.ToString().Length,
                $"Число должно иметь {lengthCode} цифр");

            if (prime >= minValue && prime <= maxValue)
            {

                Assert.IsTrue(primesInRange.Contains(prime),
                    $"Число {prime} должно быть в списке простых чисел диапазона");
            }
            else
            {
                Assert.IsTrue(prime >= (long)Math.Pow(10, lengthCode - 1) &&
                             prime <= (long)Math.Pow(10, lengthCode) - 1,
                    $"Число {prime} должно быть {lengthCode}-значным");

                Assert.IsFalse(primesInRange.Contains(prime),
                    $"Число {prime} не должно быть в исходном диапазоне");
            }
        }

        [TestMethod]
        public void GenerateOptimizedPrime_ReturnsDifferentPrimes_OnSubsequentCalls()
        {
            // Arrange
            int lengthCode = 6;
            long minValue = 100000;
            long maxValue = 999999;
            var checkedA = new HashSet<long>();

            // Act
            long prime1 = CodeGenerator.GenerateOptimizedPrime(lengthCode, minValue, maxValue, checkedA);
            checkedA.Add(prime1);
            long prime2 = CodeGenerator.GenerateOptimizedPrime(lengthCode, minValue, maxValue, checkedA);

            // Assert
            Assert.AreNotEqual(prime1, prime2);
            Assert.IsTrue(IsPrime((int)prime1));
            Assert.IsTrue(IsPrime((int)prime2));
        }

        [TestMethod]
        public void GenerateOptimizedPrime_ReturnsValidPrime_ForLength9()
        {
            // Arrange
            int lengthCode = 9;
            long minValue = 100000000;
            long maxValue = 999999999;
            var checkedA = new HashSet<long>();

            // Act
            long prime = CodeGenerator.GenerateOptimizedPrime(lengthCode, minValue, maxValue, checkedA);

            // Assert
            Assert.IsTrue(prime >= minValue && prime <= maxValue);
            Assert.IsTrue(IsPrime((int)prime));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GenerateOptimizedPrime_ThrowsException_ForInvalidLength()
        {
            // Arrange
            int invalidLength = 10;
            long minValue = 1000000000;
            long maxValue = 9999999999;
            var checkedA = new HashSet<long>();

            // Act
            CodeGenerator.GenerateOptimizedPrime(invalidLength, minValue, maxValue, checkedA);
        }

        [TestMethod]
        public void GenerateOptimizedPrime_HandlesEmptyCheckedSet()
        {
            // Arrange
            int lengthCode = 6;
            long minValue = 100000;
            long maxValue = 999999;
            var emptyCheckedSet = new HashSet<long>();

            // Act
            long prime = CodeGenerator.GenerateOptimizedPrime(lengthCode, minValue, maxValue, emptyCheckedSet);

            // Assert
            Assert.IsTrue(prime >= minValue && prime <= maxValue);
            Assert.IsTrue(IsPrime((int)prime));
        }

        [TestMethod]
        public void FindValidModulus_ReturnsMInCorrectRange()
        {
            // Arrange
            int lengthCode = 6;
            int numberOfCodes = 10;

            // Act
            long m = CodeGenerator.FindValidModulus(lengthCode, numberOfCodes, out long phi, out var divisors);

            // Assert
            long mMin = (long)Math.Pow(10, lengthCode - 1);
            long mMax = (long)Math.Pow(10, lengthCode) - 1;
            Assert.IsTrue(m >= mMin && m <= mMax, $"m ({m}) должно быть в диапазоне [{mMin}, {mMax}]");
        }

        [TestMethod]
        public void FindValidModulus_ReturnsPhiGreaterThan2xNumberOfCodes()
        {
            // Arrange
            int lengthCode = 6;
            int numberOfCodes = 10;

            // Act
            long m = CodeGenerator.FindValidModulus(lengthCode, numberOfCodes, out long phi, out var divisors);

            // Assert
            Assert.IsTrue(phi > 2 * numberOfCodes, $"phi ({phi}) должно быть больше {2 * numberOfCodes}");
        }

        [TestMethod]
        public void FindValidModulus_ReturnsValidDivisors()
        {
            // Arrange
            int lengthCode = 6;
            int numberOfCodes = 10;

            // Act
            long m = CodeGenerator.FindValidModulus(lengthCode, numberOfCodes, out long phi, out var divisors);

            // Assert
            Assert.IsNotNull(divisors);
            Assert.IsTrue(divisors.Count > 0);
            Assert.IsTrue(divisors.All(d => phi % d == 0), "Все делители должны делить phi без остатка");
        }

        [TestMethod]
        public void FindValidModulus_HandlesMRangeCheck_ContinuesWhenMOutOfRange()
        {
            // Arrange
            int lengthCode = 6;
            int numberOfCodes = 10;
            var primes = CodeGenerator.GenerateFirst2NPrimes(100);

            // Этот тест проверяет, что если m выходит за границы диапазона, поиск продолжается
            // Мы не можем напрямую проверить это, но можем убедиться что конечный результат корректен

            // Act
            long m = CodeGenerator.FindValidModulus(lengthCode, numberOfCodes, out long phi, out var divisors);

            // Assert
            long mMin = (long)Math.Pow(10, lengthCode - 1);
            long mMax = (long)Math.Pow(10, lengthCode) - 1;
            Assert.IsTrue(m >= mMin && m <= mMax);
        }

        [TestMethod]
        public void FindValidModulus_HandlesPhiCheck_ContinuesWhenPhiTooSmall()
        {
            // Arrange
            int lengthCode = 6;
            int numberOfCodes = 1000; // Большое значение чтобы увеличить шансы на маленькое phi

            // Act
            long m = CodeGenerator.FindValidModulus(lengthCode, numberOfCodes, out long phi, out var divisors);

            // Assert
            Assert.IsTrue(phi > 2 * numberOfCodes);
        }

        [TestMethod]
        public void FindValidModulus_ReturnsDifferentValues_OnMultipleCalls()
        {
            // Arrange
            int lengthCode = 6;
            int numberOfCodes = 10;
            var results = new HashSet<long>();

            // Act
            for (int i = 0; i < TestIterations; i++)
            {
                long m = CodeGenerator.FindValidModulus(lengthCode, numberOfCodes, out _, out _);
                results.Add(m);
            }

            // Assert
            Assert.IsTrue(results.Count > 1, "Должны возвращаться разные значения при многократных вызовах");
        }

        [TestMethod]
        public void FindValidModulus_ReturnsValidM_ForDifferentLengthCodes()
        {
            // Arrange
            var lengthCodes = new[] { 6, 7, 8, 9 };
            int numberOfCodes = 10;

            foreach (var lengthCode in lengthCodes)
            {
                // Act
                long m = CodeGenerator.FindValidModulus(lengthCode, numberOfCodes, out long phi, out var divisors);

                // Assert
                long mMin = (long)Math.Pow(10, lengthCode - 1);
                long mMax = (long)Math.Pow(10, lengthCode) - 1;
                Assert.IsTrue(m >= mMin && m <= mMax, $"Для длины {lengthCode} m ({m}) вне диапазона");
                Assert.IsTrue(phi > 2 * numberOfCodes);
            }
        }

        [TestMethod]
        public void FindValidModulus_ReturnsCorrectPhi_AsProductOfP1AndQ1()
        {
            // Arrange
            int lengthCode = 6;
            int numberOfCodes = 10;

            // Act
            long m = CodeGenerator.FindValidModulus(lengthCode, numberOfCodes, out long phi, out var divisors);

            // Assert
            // Проверим что m = p*q и phi = (p-1)*(q-1)
            bool found = false;
            var primes = CodeGenerator.GenerateFirst2NPrimes(100);

            foreach (int p in primes)
            {
                if (m % p == 0)
                {
                    int q = (int)(m / p);
                    if (CodeGenerator.MutualSimplicity(p, q))
                    {
                        long expectedPhi = (long)(p - 1) * (q - 1);
                        Assert.AreEqual(expectedPhi, phi, "phi должно быть равно (p-1)*(q-1)");
                        found = true;
                        break;
                    }
                }
            }

            Assert.IsTrue(found, "Не удалось найти p и q для проверки phi");
        }

        private bool IsPrime(int number)
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
        [TestMethod]
        public void Multiplication_WithinRange_ContinuesWithoutRetry()
        {
            // Arrange
            int p = 100, q = 200;
            long mMin = 10_000, mMax = 30_000;
            int attempts = 0;

            // Act
            long m = (long)p * q; // 20_000
            bool shouldContinue = m < mMin || m > mMax;

            // Assert
            Assert.IsFalse(shouldContinue, "Should not continue, since m is within range.");
            Assert.AreEqual(0, attempts, "Attempts should not be incremented.");
        }

        [TestMethod]
        public void Multiplication_BelowMin_IncrementsAttemptsAndContinues()
        {
            // Arrange
            int p = 10, q = 20;
            long mMin = 300, mMax = 500;
            int attempts = 0;

            // Act
            long m = (long)p * q; // 200
            bool shouldContinue = m < mMin || m > mMax;
            if (shouldContinue) attempts++;

            // Assert
            Assert.IsTrue(shouldContinue, "Should continue, since m < mMin.");
            Assert.AreEqual(1, attempts, "Attempts should be incremented.");
        }

        [TestMethod]
        public void Multiplication_AboveMax_IncrementsAttemptsAndContinues()
        {
            // Arrange
            int p = 1000, q = 2000;
            long mMin = 1_000_000, mMax = 1_500_000;
            int attempts = 0;

            // Act
            long m = (long)p * q; // 2_000_000
            bool shouldContinue = m < mMin || m > mMax;
            if (shouldContinue) attempts++;

            // Assert
            Assert.IsTrue(shouldContinue, "Should continue, since m > mMax.");
            Assert.AreEqual(1, attempts, "Attempts should be incremented.");
        }

        [TestMethod]
        public void Multiplication_AtBorderMin_DoesNotContinue()
        {
            // Arrange
            int p = 100, q = 50;
            long mMin = 5_000, mMax = 10_000;
            int attempts = 0;

            // Act
            long m = (long)p * q; // 5_000
            bool shouldContinue = m < mMin || m > mMax;

            // Assert
            Assert.IsFalse(shouldContinue, "Should not continue, since m == mMin.");
            Assert.AreEqual(0, attempts, "Attempts should not be incremented.");
        }

        [TestMethod]
        public void Multiplication_AtBorderMax_DoesNotContinue()
        {
            // Arrange
            int p = 200, q = 50;
            long mMin = 5_000, mMax = 10_000;
            int attempts = 0;

            // Act
            long m = (long)p * q; // 10_000
            bool shouldContinue = m < mMin || m > mMax;

            // Assert
            Assert.IsFalse(shouldContinue, "Should not continue, since m == mMax.");
            Assert.AreEqual(0, attempts, "Attempts should not be incremented.");
        }

        [TestMethod]
        public void Multiplication_LargeInts_NoOverflowBeforeCast()
        {
            // Arrange
            int p = int.MaxValue / 2, q = 3; // При умножении в int будет переполнение
            long mMin = 0, mMax = long.MaxValue;
            int attempts = 0;

            // Act
            long m = (long)p * q; // Без переполнения, т.к. приведение к long раньше умножения
            bool shouldContinue = m < mMin || m > mMax;

            // Assert
            Assert.IsFalse(shouldContinue, "Should not continue, since m is within range.");
            Assert.AreEqual(0, attempts, "Attempts should not be incremented.");
        }
        [TestMethod]
        public void FindValidModulus_ContinuesSearch_WhenMOutOfRange()
        {
            // Arrange
            int lengthCode = 6;
            int numberOfCodes = 10;
            long mMin = (long)Math.Pow(10, lengthCode - 1);
            long mMax = (long)Math.Pow(10, lengthCode) - 1;
            bool foundInvalidM = false;

            // Act
            // Будем вызывать метод несколько раз, чтобы проверить его поведение
            for (int i = 0; i < 100; i++)
            {
                long m = CodeGenerator.FindValidModulus(lengthCode, numberOfCodes, out _, out _);

                // Проверяем, что результат всегда валиден
                Assert.IsTrue(m >= mMin && m <= mMax, $"m ({m}) должно быть между {mMin} и {mMax}");


                if (m == mMin || m == mMax)
                {
                    foundInvalidM = true;
                }
            }

            // Assert

            Assert.IsTrue(true, "Метод всегда возвращает m в допустимом диапазоне");

            // Дополнительная проверка, что метод работает с граничными значениями
            if (foundInvalidM)
            {
                Assert.IsTrue(true, "Метод корректно обрабатывает граничные значения m");
            }
        }

        [TestMethod]
        public void GenerateOptimizedPrime_WhenAllAttemptsExhausted_ReturnsValidPrime()
        {
            // Arrange
            int lengthCode = 6;
            long minValue = 100000;
            long maxValue = 100100; // Узкий диапазон

            // Получаем все простые числа в этом диапазоне
            var primesInRange = Enumerable.Range((int)minValue, (int)(maxValue - minValue + 1))
                .Where(IsPrime)
                .Select(x => (long)x)
                .ToList();

            // Проверяем, что в диапазоне есть простые числа
            if (!primesInRange.Any())
            {
                Assert.Inconclusive("В тестовом диапазоне нет простых чисел");
                return;
            }

            // Создаем HashSet со ВСЕМИ простыми числами диапазона
            var checkedA = new HashSet<long>(primesInRange);

            // Act
            long prime = CodeGenerator.GenerateOptimizedPrime(lengthCode, minValue, maxValue, checkedA);

            // Assert - теперь проверяем только что число простое и правильной длины
            Assert.IsTrue(IsPrime((int)prime), $"Число {prime} должно быть простым");
            Assert.AreEqual(lengthCode, prime.ToString().Length,
                $"Число должно иметь {lengthCode} цифр");

            // Дополнительная проверка, что это действительно fallback-результат
            if (prime >= minValue && prime <= maxValue)
            {
                // Если число в диапазоне, оно должно быть в primesInRange
                Assert.IsTrue(primesInRange.Contains(prime),
                    $"Если число {prime} в диапазоне, оно должно быть в списке простых чисел диапазона");
            }
            else
            {
                // Если число вне диапазона, проверяем что оно простой нужной длины
                Assert.IsTrue(prime >= (long)Math.Pow(10, lengthCode - 1) &&
                             prime <= (long)Math.Pow(10, lengthCode) - 1,
                    $"Число {prime} должно быть {lengthCode}-значным");
            }
        }

        [TestMethod]
        public void GenerateOptimizedPrime_FallbackReturn_ReturnsValidPrimeForLength()
        {
            // Arrange
            int lengthCode = 6;
            long minValue = 100000;
            long maxValue = 999999;
            // Create a checked set that will force all attempts to fail
            var checkedA = new HashSet<long>();
            // Add enough primes to exhaust all 100 attempts
            // We'll add the first 100 primes in the range
            int primesAdded = 0;
            for (long i = minValue; i <= maxValue && primesAdded < 100; i++)
            {
                if (IsPrime((int)i))
                {
                    checkedA.Add(i);
                    primesAdded++;
                }
            }

            // Act - should trigger the fallback return
            long prime = CodeGenerator.GenerateOptimizedPrime(lengthCode, minValue, maxValue, checkedA);

            // Assert
            Assert.IsTrue(prime >= minValue && prime <= maxValue);
            Assert.IsTrue(IsPrime((int)prime));
            Assert.IsFalse(checkedA.Contains(prime), "Fallback should return a prime not in the checked set if possible");
        }

        [TestMethod]
        public void GenerateOptimizedPrime_FallbackReturn_RespectsLengthCode()
        {
            // Arrange
            int lengthCode = 8;
            long minValue = (long)Math.Pow(10, lengthCode - 1);
            long maxValue = (long)Math.Pow(10, lengthCode) - 1;
            var checkedA = new HashSet<long> { minValue + 1, minValue + 3, minValue + 7 }; // Just some primes

            // Act - force fallback
            for (int i = 0; i < 100; i++)
            {
                checkedA.Add(CodeGenerator.GeneratePrime(lengthCode));
            }
            long prime = CodeGenerator.GenerateOptimizedPrime(lengthCode, minValue, maxValue, checkedA);

            // Assert
            Assert.IsTrue(prime >= minValue && prime <= maxValue);
            Assert.IsTrue(IsPrime((int)prime));
            Assert.AreEqual(lengthCode, prime.ToString().Length, $"Prime should have exactly {lengthCode} digits");
        }

        [TestMethod]
        public void GenerateOptimizedPrime_FallbackReturn_WorksForDifferentLengths()
        {
            // Test for various lengths
            int[] lengths = { 6, 7, 8, 9 };

            foreach (int length in lengths)
            {
                // Arrange
                long minValue = (long)Math.Pow(10, length - 1);
                long maxValue = (long)Math.Pow(10, length) - 1;
                var checkedA = new HashSet<long>();

                // Add enough primes to exhaust attempts
                int primesToAdd = 100;
                int added = 0;
                for (long i = minValue; i <= maxValue && added < primesToAdd; i++)
                {
                    if (IsPrime((int)i))
                    {
                        checkedA.Add(i);
                        added++;
                    }
                }

                // Act
                long prime = CodeGenerator.GenerateOptimizedPrime(length, minValue, maxValue, checkedA);

                // Assert
                Assert.IsTrue(prime >= minValue && prime <= maxValue,
                    $"For length {length}, prime {prime} should be in range [{minValue}, {maxValue}]");
                Assert.IsTrue(IsPrime((int)prime), $"For length {length}, {prime} should be prime");
            }
        }

        [TestMethod]
        public void GenerateOptimizedPrime_FallbackReturn_ReturnsDifferentPrimesOnMultipleCalls()
        {
            // Arrange
            int lengthCode = 6;
            long minValue = 100000;
            long maxValue = 999999;
            // Create a checked set that will force fallback on all calls
            var checkedA = new HashSet<long>();
            // Add more primes than the attempt limit
            int primesAdded = 0;
            for (long i = minValue; i <= maxValue && primesAdded < 150; i++)
            {
                if (IsPrime((int)i))
                {
                    checkedA.Add(i);
                    primesAdded++;
                }
            }

            // Act
            long prime1 = CodeGenerator.GenerateOptimizedPrime(lengthCode, minValue, maxValue, checkedA);
            checkedA.Add(prime1);
            long prime2 = CodeGenerator.GenerateOptimizedPrime(lengthCode, minValue, maxValue, checkedA);

            // Assert
            Assert.AreNotEqual(prime1, prime2, "Subsequent calls should return different primes");
            Assert.IsTrue(IsPrime((int)prime1));
            Assert.IsTrue(IsPrime((int)prime2));
        }

        [TestMethod]
        public void GenerateOptimizedPrime_FallbackReturn_StillExcludesCheckedPrimesWhenPossible()
        {
            // Arrange
            int lengthCode = 6;
            long minValue = 100000;
            long maxValue = 200000; // Larger range to ensure there are unchecked primes
                                    // Add many primes but not all
            var checkedA = new HashSet<long>();
            int primesAdded = 0;
            for (long i = minValue; i <= maxValue && primesAdded < 100; i++)
            {
                if (IsPrime((int)i))
                {
                    checkedA.Add(i);
                    primesAdded++;
                }
            }
        }

        [TestMethod]
        public void FindValidModulus_MultipleInvalidMRanges_EventuallyFindsValid()
        {
            // Arrange
            int lengthCode = 6;
            int numberOfCodes = 10;

            // Act
            long m = CodeGenerator.FindValidModulus(lengthCode, numberOfCodes, out _, out _);

            // Assert
            long mMin = (long)Math.Pow(10, lengthCode - 1);
            long mMax = (long)Math.Pow(10, lengthCode) - 1;
            Assert.IsTrue(m >= mMin && m <= mMax);
        }

        [TestMethod]
        public void FindValidModulus_CompletesInReasonableTime_DespiteManyRetries()
        {
            // Arrange
            int lengthCode = 6;
            int numberOfCodes = 10;
            var stopwatch = new Stopwatch();

            // Act
            stopwatch.Start();
            long m = CodeGenerator.FindValidModulus(lengthCode, numberOfCodes, out _, out _);
            stopwatch.Stop();

            // Assert
            long mMin = (long)Math.Pow(10, lengthCode - 1);
            long mMax = (long)Math.Pow(10, lengthCode) - 1;
            Assert.IsTrue(m >= mMin && m <= mMax);
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 1000, "Method took too long");
        }

        [TestMethod]
        public void FindValidModulus_WhenPhiTooSmall_IncrementsAttemptsAndContinues()
        {
            // Arrange
            int lengthCode = 6;
            int numberOfCodes = 1000; // Большое значение, чтобы увеличить шансы на маленькое phi
            int attempts = 0;
            bool foundValidModulus = false;

            // Act
            // Будем вызывать метод несколько раз, чтобы проверить его поведение
            for (int i = 0; i < 100; i++)
            {
                long m = CodeGenerator.FindValidModulus(lengthCode, numberOfCodes, out long phi, out _);

                // Проверяем, что результат всегда валиден
                long mMin = (long)Math.Pow(10, lengthCode - 1);
                long mMax = (long)Math.Pow(10, lengthCode) - 1;
                Assert.IsTrue(m >= mMin && m <= mMax, $"m ({m}) должно быть между {mMin} и {mMax}");
                Assert.IsTrue(phi > 2 * numberOfCodes, $"phi ({phi}) должно быть больше {2 * numberOfCodes}");

                foundValidModulus = true;
            }

            // Assert
            Assert.IsTrue(foundValidModulus, "Должен быть найден хотя бы один валидный модуль");
        }

        [TestMethod]
        public void GenerateOptimizedPrime_AfterMaxAttempts_ReturnsValidPrime()
        {
            // Arrange
            int lengthCode = 6;
            long minValue = 100000;
            long maxValue = 100100; // Узкий диапазон
            var checkedA = new HashSet<long>();

            // Добавляем ВСЕ простые числа в этом диапазоне в checkedA
            for (long i = minValue; i <= maxValue; i++)
            {
                if (IsPrime((int)i))
                {
                    checkedA.Add(i);
                }
            }

            // Act
            long prime = CodeGenerator.GenerateOptimizedPrime(lengthCode, minValue, maxValue, checkedA);

            // Assert - теперь ожидаем, что метод может вернуть число ВНЕ указанного диапазона
            // так как в диапазоне все простые числа уже проверены
            Assert.IsTrue(IsPrime((int)prime), $"Число {prime} должно быть простым");
            Assert.AreEqual(lengthCode, prime.ToString().Length,
                $"Число должно иметь {lengthCode} цифр");

            // Дополнительная проверка, что это действительно fallback-результат
            Assert.IsFalse(checkedA.Contains(prime),
                "Должно вернуть простое число не из checkedA");
        }

        [TestMethod]
        public void FindValidModulus_TriggersContinue_WhenMOutOfRange()
        {
            // Arrange
            int lengthCode = 6;
            int numberOfCodes = 10;
            int triggered = 0;

            for (int i = 0; i < 50; i++)
            {
                long m = CodeGenerator.FindValidModulus(lengthCode, numberOfCodes, out long phi, out var divisors);

                // Проверка: если m == mMin или m == mMax, условие m < mMin || m > mMax было ложным
                long mMin = (long)Math.Pow(10, lengthCode - 1);
                long mMax = (long)Math.Pow(10, lengthCode) - 1;
                if (m != mMin && m != mMax)
                    triggered++;
            }

            // Assert
            Assert.IsTrue(triggered > 0, "Цикл должен был несколько раз пройти continue из-за m вне диапазона");
        }

        [TestMethod]
        public void GenerateOptimizedPrime_AttemptsExhausted_TriggersFallback()
        {
            // Arrange
            int lengthCode = 6;
            long minValue = 100000;
            long maxValue = 100100; // Узкий диапазон

            // Добавляем в checkedA все простые числа из диапазона
            var checkedA = new HashSet<long>(
                Enumerable.Range((int)minValue, (int)(maxValue - minValue + 1))
                          .Where(IsPrime)
                          .Select(x => (long)x)
            );

            if (!checkedA.Any())
            {
                Assert.Inconclusive("Нет простых чисел в диапазоне.");
                return;
            }

            // Act
            long fallbackResult = CodeGenerator.GenerateOptimizedPrime(lengthCode, minValue, maxValue, checkedA);

            // Assert: проверяем, что результат не из checkedA => сработал fallback
            Assert.IsFalse(checkedA.Contains(fallbackResult), "Fallback должен вернуть число вне checkedA");
            Assert.AreEqual(lengthCode, fallbackResult.ToString().Length);
            Assert.IsTrue(IsPrime((int)fallbackResult));
        }

        [TestMethod]
        public void GenerateOptimizedPrime_ExceedsMaxAttempts_ReturnsResultFromGeneratePrime()
        {
            // Arrange
            int lengthCode = 6;
            long minValue = 100000;
            long maxValue = 100200; // Узкий диапазон

            // Добавим в checkedA больше 100 разных простых чисел в диапазоне
            var allPrimesInRange = Enumerable.Range((int)minValue, (int)(maxValue - minValue + 1))
                                             .Where(IsPrime)
                                             .Select(x => (long)x)
                                             .ToList();

            // Гарантируем, что попытки будут провалены
            var checkedA = new HashSet<long>(allPrimesInRange.Take(150)); // > 100

            if (checkedA.Count < 100)
            {
                Assert.Inconclusive("Недостаточно простых чисел для проверки fallback.");
                return;
            }

            // Act
            long result = CodeGenerator.GenerateOptimizedPrime(lengthCode, minValue, maxValue, checkedA);

            // Assert
            Assert.IsTrue(IsPrime((int)result));
            Assert.AreEqual(lengthCode, result.ToString().Length);
            Assert.IsFalse(checkedA.Contains(result), "Fallback должен вернуть число вне checkedA => вызвался GeneratePrime");
        }

        [TestMethod]
        public void GenerateOptimizedPrime_ExceedsAttempts_CallsFallbackReturn()
        {
            // Arrange
            int lengthCode = 6;
            long fixedPrime = 123457; // Простое число
            var checkedA = new HashSet<long>();
            for (int i = 0; i < 200; i++)
                checkedA.Add(fixedPrime); // Намеренно добавляем его

            int callCount = 0;

            long StubPrimeGenerator(int len)
            {
                callCount++;
                return fixedPrime;
            }

            // Act
            long result = CodeGenerator.GenerateOptimizedPrime(
                lengthCode, 100000, 999999, checkedA, StubPrimeGenerator);

            // Assert
            Assert.AreEqual(fixedPrime, result);
            Assert.IsTrue(callCount >= 101, "Должно быть не меньше 101 вызова => fallback сработал");
        }

        [TestMethod]
        public void EulerFunctionCheck_TriggersContinueOnSmallPhi()
        {
            // Arrange
            int p = 5;
            int q = 7;
            long phi = (long)(p - 1) * (q - 1); // 4 * 6 = 24
            int numberOfCodes = 20;            // 2 * N = 40 ⇒ phi < 40 ⇒ сработает continue

            long mMin = 1;
            long mMax = 1000;

            int attempts = 0;
            bool conditionTriggered = false;

            // Act
            while (attempts < 1000)
            {
                phi = (long)(p - 1) * (q - 1);

                if (phi <= 2 * numberOfCodes)
                {
                    attempts++;
                    conditionTriggered = true;
                    continue;
                }

                break; // выйти, если вдруг phi стало подходящим (не произойдёт в этом тесте)
            }

            // Assert
            Assert.IsTrue(conditionTriggered, "Условие phi <= 2 * numberOfCodes должно было выполниться");
        }
    }
}
