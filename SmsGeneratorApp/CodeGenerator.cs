
namespace SmsGeneratorApp
{
    public static class CodeGenerator
    {
        static Random rnd = new Random();


        // Метод для нахождения первых 2N простых чисел с помощью Решета Эратосфена
        public static int[] GenerateFirst2NPrimes(int numberOfCodes)
        {
            if (numberOfCodes <= 0) return new int[0];

            // Оценка верхней границы для 2N простых чисел 
            int upperBound = numberOfCodes > 6 ? (int)(2 * numberOfCodes * Math.Log(2 * numberOfCodes) + 2 * numberOfCodes * Math.Log(Math.Log(2 * numberOfCodes))) : 20;

            bool[] isPrime = new bool[upperBound + 1];
            for (int i = 2; i <= upperBound; i++) isPrime[i] = true;

            for (int p = 2; p * p <= upperBound; p++)
            {
                if (isPrime[p])
                {
                    for (int i = p * p; i <= upperBound; i += p)
                    {
                        isPrime[i] = false;
                    }
                }
            }

            List<int> primes = new List<int>();
            for (int i = 2; i <= upperBound && primes.Count < 2 * numberOfCodes; i++)
            {
                if (isPrime[i]) primes.Add(i);
            }

            // Если не набрали достаточно простых чисел, увеличиваем границу и повторяем
            while (primes.Count < 2 * numberOfCodes)
            {
                upperBound *= 2;
                isPrime = new bool[upperBound + 1];
                for (int i = 2; i <= upperBound; i++) isPrime[i] = true;

                for (int p = 2; p * p <= upperBound; p++)
                {
                    if (isPrime[p])
                    {
                        for (int i = p * p; i <= upperBound; i += p)
                        {
                            isPrime[i] = false;
                        }
                    }
                }

                primes.Clear();
                for (int i = 2; i <= upperBound && primes.Count < 2 * numberOfCodes; i++)
                {
                    if (isPrime[i]) primes.Add(i);
                }
            }

            return primes.GetRange(0, 2 * numberOfCodes).ToArray();
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
        public static (int B, int G) GeneratePrimesBG(int numberOfCodes)
        {
            // Генерируем простые числа, пока не найдем подходящие
            while (true)
            {
                // Генерируем множество простых чисел
                int[] primes = GenerateFirst2NPrimes(numberOfCodes * 2);

                // Выбираем два разных простых числа
                int bIndex = rnd.Next(0, primes.Length);
                int gIndex = rnd.Next(0, primes.Length);
                while (gIndex == bIndex && primes.Length > 1)
                {
                    gIndex = rnd.Next(0, primes.Length);
                }

                int B = primes[bIndex];
                int G = primes[gIndex];

                // Проверяем условие: функция Эйлера от G должна быть больше 2N
                // Для простого G: φ(G) = G - 1
                long phiG = G - 1;
                if (phiG > 2 * numberOfCodes)
                {
                    return (B, G);
                }
            }
        }

        // Метод для вычисления k = B^d mod G для d от 1 до 1000
        public static List<long> CalculateKValues(int B, int G)
        {
            List<long> kValues = new List<long>();

            for (int d = 1; d <= 1000; d++)
            {
                long k = PowMod(B, d, G);
                kValues.Add(k);
            }

            return kValues;
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

            // 3. Генерация списка k
            usedK = GenerateKValues(numberOfCodes);

            // 4. Генерация кодов: a^k mod m
            var codes = new List<long>();
            var seen = new HashSet<long>();

            foreach (var k in usedK)
            {
                long code = ModPow(a, k, m);
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

        private static List<long> GenerateKValues(int count)
        {
            var values = new List<long>();
            for (int i = 0; i < count * 2; i++)
            {
                values.Add(rnd.Next(1000, 100000));
            }
            return values.Distinct().Take(count).ToList();
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

        private static long ModPow(long a, long k, long m)
        {
            long result = 1;
            a %= m;

            while (k > 0)
            {
                if ((k & 1) == 1)
                    result = (result * a) % m;
                a = (a * a) % m;
                k >>= 1;
            }

            return result;
        }

    }
}
