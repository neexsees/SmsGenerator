using SmsGeneratorApp;
namespace TestProject1
{
    [TestClass]
    public sealed class Test
    {
        [TestMethod]
        public void GenerateFirst2NPrimes_ZeroInput_ReturnsEmptyArray()
        {
            // Act
            var result = CodeGenerator.GenerateFirst2NPrimes(0);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Length);
        }

        [TestMethod]
        public void GenerateFirst2NPrimes_NegativeInput_ReturnsEmptyArray()
        {
            // Act
            var result = CodeGenerator.GenerateFirst2NPrimes(-5);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Length);
        }

        private bool IsPrime(int number)
        {
            if (number <= 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            var boundary = (int)Math.Floor(Math.Sqrt(number));

            for (int i = 3; i <= boundary; i += 2)
            {
                if (number % i == 0)
                    return false;
            }

            return true;

        }
    }
}