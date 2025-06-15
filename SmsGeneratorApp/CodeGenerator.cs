
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

                // Увеличиваем границу для следующей попытки
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
                    return 1;
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
        public static long FindValidModulus(int lengthCode, int numberOfCodes, out long phi, out List<long> phiDivisors)
        {
            if (lengthCode < 6 || lengthCode > 9)
                throw new ArgumentException("Длина кода должна быть от 6 до 9");

            long mMin = (long)Math.Pow(10, lengthCode - 1);
            long mMax = (long)Math.Pow(10, lengthCode) - 1;

            while (true)
            {
                int p = SelectRandomPrimeP(100);

                long minQ = mMin / p + 1;
                long maxQ = mMax / p;

                if (minQ >= maxQ) continue;

                int attempts = 0;
                while (attempts < 1000)
                {
                    int q = FindPrimeByBruteForce((int)minQ, (int)maxQ);

                    if (!MutualSimplicity(p, q))
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

                    phi = (long)(p - 1) * (q - 1); // функция Эйлера

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
        static long PowMod(long a, long k, long n)
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


        public static List<long> GenerateCodes(int lengthCode, int numberOfCodes,
                                              out long m, out long a, out List<long> usedK)
        {
            // 1. Генерация модуля m (10^(n-1) < m < 10^n)
            m = GenerateModulus(lengthCode);

            // 2. Выбор простого a нужной длины
            a = GeneratePrime(lengthCode);

            // 3. Генерация простых чисел b и g
            (int b, int g) = GeneratePrimesBG(numberOfCodes);

            // 4. Генерация списка k
            usedK = CalculateKValues(b, g, numberOfCodes);

            // 5. Генерация кодов: a^k mod m
            var codes = new List<long>();
            var seen = new HashSet<long>();

            foreach (var k in usedK)
            {
                long code = PowMod(a, k, m);
                if (seen.Add(code)) // Уникальные коды
                {
                    codes.Add(code);
                    if (codes.Count >= numberOfCodes)
                        break;
                }
            }

            return codes;
        }

        private static long GenerateModulus(int length)
        {
            long min = (long)Math.Pow(10, length - 1);
            long max = (long)Math.Pow(10, length) - 1;
            return rnd.Next((int)min, (int)max);
        }

        private static long GeneratePrime(int length)
        {
            long min = (long)Math.Pow(10, length - 1);
            long max = (long)Math.Pow(10, length) - 1;

            for (int i = 0; i < 1000; i++)
            {
                long num = rnd.Next((int)min, (int)max);
                if (IsPrime(num))
                    return num;
            }

            throw new Exception("Не удалось найти простое число за 1000 попыток");
        }


        private static bool IsPrime(long number)
        {
            if (number <= 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            var boundary = (long)Math.Sqrt(number);
            for (long i = 3; i <= boundary; i += 2)
                if (number % i == 0)
                    return false;

            return true;
        }

    }
}
