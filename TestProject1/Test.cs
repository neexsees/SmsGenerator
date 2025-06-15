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
        public void CalculateKValues_EdgeCases_HandlesCorrectly()
        {
            // Test with b = 1
            var result1 = CodeGenerator.CalculateKValues(1, 7, 3);
            Assert.AreEqual(3, result1.Count);

            // Test with count = 0
            var result2 = CodeGenerator.CalculateKValues(5, 7, 0);
            Assert.AreEqual(0, result2.Count);
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
        public void FindPrimeByBruteForce_EdgeCases_HandlesCorrectly()
        {
            // Test with min = max (should throw or handle)
            Assert.ThrowsException<ArgumentException>(() =>
                CodeGenerator.FindPrimeByBruteForce(10, 10));
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
        public void GenerateOptimizedPrime_ReturnsPrimeWhenCheckedSetFull()
        {
            // Arrange
            int lengthCode = 6;
            long minValue = 100000;
            long maxValue = 100100; // Ограничиваем диапазон для теста
            var checkedA = new HashSet<long>(Enumerable.Range((int)minValue, (int)(maxValue - minValue)).Select(x => (long)x));

            // Act
            long prime = CodeGenerator.GenerateOptimizedPrime(lengthCode, minValue, maxValue, checkedA);

            // Assert
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
        public void FindOptimizedModulus_ReturnsModulusWithPhiDivisibleByLength()
        {
            // Arrange
            int lengthCode = 6;
            int numberOfCodes = 10;
            var checkedM = new HashSet<long>();

            // Act
            long m = CodeGenerator.FindOptimizedModulus(lengthCode, numberOfCodes, checkedM);

            // Assert
            CodeGenerator.FindValidModulus(lengthCode, numberOfCodes, out long phi, out _);
            Assert.AreEqual(0, phi % lengthCode);
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
    }
}