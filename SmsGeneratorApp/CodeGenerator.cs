
namespace SmsGeneratorApp
{
    public static class CodeGenerator
    {
        static Random rnd = new Random();


        // Метод для нахождения первых 2N простых чисел с помощью Решета Эратосфена
        public static int[] GenerateFirst2NPrimes(int numberOfCodes)
        {
            if (numberOfCodes <= 0) return Array.Empty<int>();

            int EstimateUpperBound(int n) => n < 6 ? 20 :
                (int)(n * (Math.Log(n) + Math.Log(Math.Log(n)) - 0.5));

            int upperBound = EstimateUpperBound(2 * numberOfCodes);
            var primes = new List<int>(2 * numberOfCodes);

            while (primes.Count < 2 * numberOfCodes)
            {
                int sieveBound = (upperBound - 1) / 2;
                var isPrime = new bool[sieveBound + 1];
                Array.Fill(isPrime, true);

                if (upperBound >= 2 && primes.Count < 2 * numberOfCodes)
                    primes.Add(2);

                for (int i = 1; i <= sieveBound; i++)
                {
                    if (isPrime[i])
                    {
                        int currentPrime = 2 * i + 1;
                        if (primes.Count < 2 * numberOfCodes)
                            primes.Add(currentPrime);

                        for (long j = (long)currentPrime * currentPrime; j <= upperBound; j += 2 * currentPrime)
                        {
                            if (j % 2 == 1)
                                isPrime[(j - 1) / 2] = false;
                        }
                    }
                }

                if (primes.Count >= 2 * numberOfCodes)
                    break;


                upperBound *= 2;
            }

            return primes.Take(2 * numberOfCodes).ToArray();
        }

        public static int SelectRandomPrimeP(int NumberOfCodes)
        {
            // Генерируем первые 2N простых чисел с помощью Решета Эратосфена
            int[] primes = GenerateFirst2NPrimes(NumberOfCodes);

            // Выбираем случайное простое число p из этого множества
            int randomIndex = rnd.Next(0, primes.Length);
            return primes[randomIndex];
        }

        // Метод для генерации простых чисел B и G
        public static (int b, int g) GeneratePrimesBG(int numberOfCodes)
        {
            while (true)
            {
                int[] primes = GenerateFirst2NPrimes(numberOfCodes * 2);

                int bIndex = rnd.Next(0, primes.Length);
                int gIndex = rnd.Next(0, primes.Length);
                while (gIndex == bIndex && primes.Length > 1)
                {
                    gIndex = rnd.Next(0, primes.Length);
                }

                int b = primes[bIndex];
                int g = primes[gIndex];

                long phiG = g - 1;
                if (phiG > 2 * numberOfCodes)
                {
                    return (b, g);
                }
            }
        }
        //нахождение стпени k
        public static List<long> CalculateKValues(int b, int g, int count)
        {
            HashSet<long> uniqueKValues = new HashSet<long>();

            for (int d = 1; uniqueKValues.Count < count && d <= count * 2; d++)
            {
                long k = PowMod(b, d, g);
                uniqueKValues.Add(k);
            }

            return uniqueKValues.Take(count).ToList();
        }

        //Метод "грубой силы" для проверки простоты числа
        public static int FindPrimeByBruteForce(int min, int max)
        {
            while (true)
            {
                int q = rnd.Next(min, max);
                if (q % 2 == 0)
                    q++;

                bool isPrime = true;
                int qSqrt = (int)Math.Sqrt(q);

                for (int i = 2; i <= qSqrt; i++)
                {
                    if (q % i == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }

                if (isPrime)
                    return q;
            }
        }

        //Проверка взаимной простоты числел
        public static bool MutualSimplicity(int a, int b)
        {
            if (a < b)
            {
                int temp = a;
                a = b;
                b = temp;
            }

            while (b != 0)
            {
                int r = a % b;
                a = b;
                b = r;
            }

            return a == 1;
        }

        //подбор M
        public static long FindValidModulus(int lengthCode, int numberOfCodes, out long phi, out List<long> phiDivisors, out long p, out long q)
        {
            if (lengthCode < 6 || lengthCode > 9)
                throw new ArgumentException("Длина кода должна быть от 6 до 9");

            long mMin = (long)Math.Pow(10, lengthCode - 1);
            long mMax = (long)Math.Pow(10, lengthCode) - 1;

            while (true)
            {
                p = SelectRandomPrimeP(100);

                long minQ = mMin / p + 1;
                long maxQ = mMax / p;

                if (minQ >= maxQ) continue;

                int attempts = 0;
                while (attempts < 1000)
                {

                    q = FindPrimeByBruteForce((int)minQ, (int)maxQ);

                    if (!MutualSimplicity((int)p, (int)q))
                    {
                        attempts++;
                        continue;
                    }

                    long m = (long)p * q;

                    if (m < mMin || m > mMax)
                    {
                        attempts++;
                        continue;
                    }

                    phi = (long)(p - 1) * (q - 1);

                    if (phi <= 2 * numberOfCodes)
                    {
                        attempts++;
                        continue;
                    }

                    phiDivisors = FindAllDivisors(phi);

                    return m;

                }
            }
        }

        //Делители функции Эйлера
        public static List<long> FindAllDivisors(long n)
        {
            List<long> divisors = new List<long>();
            for (long i = 1; i * i <= n; i++)
            {
                if (n % i == 0)
                {
                    divisors.Add(i);
                    if (i != n / i)
                        divisors.Add(n / i);
                }
            }
            divisors.Sort();
            return divisors;
        }

        //Возведение в степень
        public static long PowMod(long a, long k, long n)
        {
            long res = 1;
            a %= n;
            while (k > 0)
            {
                if ((k & 1) == 1)
                    res = (res * a) % n;
                a = (a * a) % n;
                k >>= 1;
            }
            return res;
        }


        public static List<long> GenerateCodes(int lengthCode, int numberOfCodes, out long m, out long a, out long p, out long q, out List<long> usedK, out int b, out int g)
        {

            long minValue = (long)Math.Pow(10, lengthCode - 1);
            long maxValue = (long)Math.Pow(10, lengthCode) - 1;

            var checkedA = new HashSet<long>();
            var checkedM = new HashSet<long>();

            while (true)
            {

                m = CodeGenerator.FindValidModulus(lengthCode, numberOfCodes, out _, out _, out p, out q);
                checkedM.Add(m);


                a = GenerateOptimizedPrime(lengthCode, minValue, maxValue, checkedA);
                checkedA.Add(a);

                if (!MutualSimplicity((int)a, (int)m)) continue;


                var (codes, generatedK, generatedB, generatedG) = TryGenerateCodes(a, m, lengthCode, numberOfCodes, minValue, maxValue);
                if (codes != null && codes.Count >= numberOfCodes)
                {
                    usedK = generatedK;
                    b = generatedB;
                    g = generatedG;
                    return codes;
                }

            }
        }

        public static (List<long> codes, List<long> usedK, int b, int g) TryGenerateCodes(long a, long m, int lengthCode, int numberOfCodes, long minValue, long maxValue)
        {
            var codes = new List<long>(numberOfCodes);
            var usedK = new List<long>(numberOfCodes);
            var seen = new HashSet<long>();

            (int b, int g) = GeneratePrimesBG(numberOfCodes);
            var kValues = CalculateKValues(b, g, numberOfCodes);

            foreach (var k in kValues)
            {
                long code = PowMod(a, k, m);

                if (code >= minValue && code <= maxValue && seen.Add(code))
                {
                    codes.Add(code);
                    usedK.Add(k);

                    if (codes.Count >= numberOfCodes)
                        return (codes, usedK, b, g);
                }
            }
            return (null, null, 0, 0);
        }

        public static long FindOptimizedModulus(int lengthCode, int numberOfCodes, HashSet<long> checkedM)
        {
            int attempts = 0;
            long p, q;
            while (attempts < 1000)
            {
                long m = FindValidModulus(lengthCode, numberOfCodes, out long phi, out _, out p, out q);
                if (!checkedM.Contains(m) && phi % lengthCode == 0)
                    return m;
                attempts++;
            }

            return FindValidModulus(lengthCode, numberOfCodes, out _, out _, out p, out q);
        }

        public static long GenerateOptimizedPrime(
            int lengthCode, long minValue, long maxValue, HashSet<long> checkedA,
            Func<int, long> generatePrimeOverride = null)
        {
            var generator = generatePrimeOverride ?? GeneratePrime;

            int attempts = 0;
            while (attempts < 100)
            {
                long a = generator(lengthCode);
                if (!checkedA.Contains(a))
                    return a;
                attempts++;
            }
            return generator(lengthCode);
        }

        public static long GeneratePrime(int length)
        {
            long min = (long)Math.Pow(10, length - 1);
            long max = (long)Math.Pow(10, length) - 1;

            if (min < int.MinValue || max > int.MaxValue)
                throw new ArgumentException("Диапазон слишком большой для int");

            int prime = FindPrimeByBruteForce((int)min, (int)max);
            return prime;
        }
    }
}
