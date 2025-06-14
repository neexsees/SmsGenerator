
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

        //Добавление проверки простоты чисел 
        static bool IsPrime(long num)
        {
            if (num <= 1) return false;
            for (long i = 2; i * i <= num; i++)
                if (num % i == 0) return false;
            return true;
        }

        //функция Эйлера для вычисляет сколько взаимно простых чисел есть с заданным числом
        static long EulerFunction(long n)
        {
            if (IsPrime(n)) return n - 1;

            // Стандартный расчет для составных чисел
            long result = n;
            for (long p = 2; p * p <= n; ++p)
            {
                if (n % p == 0)
                {
                    while (n % p == 0)
                        n /= p;
                    result -= result / p;
                }
            }
            if (n > 1)
                result -= result / n;
            return result;
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

        //Создание кодов
        public static List<long> MakeCodes(long a, long n, long len, long amount)
        {
            long phi = EulerFunction(n);
            long min = 1;
            for (int i = 1; i < len; i++) min *= 10;
            long maxPossible = Math.Min(n - 1, (long)Math.Pow(10, len) - 1);

            if (min > maxPossible)
                throw new ArgumentException($"При n = {n} невозможно получить {len}-значные остатки.");
            var codes = new HashSet<long>();
            long tries = 0, limit = amount * 1000; //исключение цикла
            while (codes.Count < amount && tries < limit)
            {
                long k = rnd.Next(1, (int)(phi > int.MaxValue ? int.MaxValue : phi + 1));
                long value = PowMod(a, k, n);

                if (value >= min && value <= maxPossible)
                    codes.Add(value);

                tries++;
            }

            if (codes.Count < amount)
                throw new ArgumentException($"Предупреждение: удалось набрать только {codes.Count} уникальных кодов.");

            return new List<long>(codes);
        }

        public static bool FindSuitableParameters(long len, long amount, out long a, out long n)
        {
            a = 0;
            n = 0;

            long minN = (long)Math.Pow(10, len - 1);
            long maxN = (long)Math.Pow(10, len) - 1;

            for (long candidateN = maxN; candidateN >= minN; candidateN--)
            {
                if (candidateN % 2 == 0) continue;// Проверяем только нечетные числа (увеличивает шансы на взаимную простоту)

                long phi = EulerFunction(candidateN);
                if (phi < amount) continue;// Если количество возможных кодов меньше требуемого, пропускаем

                // Пытаемся найти подходящее a
                for (long attempt = 0; attempt < 100; attempt++)
                {
                    long candidateA = rnd.Next(2, (int)(candidateN > int.MaxValue ? int.MaxValue : candidateN));
                    if (MutualSimplicity(candidateA, candidateN) == 1)
                    {
                        try // Проверяем, можно ли получить достаточно кодов с этими параметрами
                        {
                            var testCodes = MakeCodes(candidateA, candidateN, len, amount);
                            if (testCodes.Count == amount)
                            {
                                a = candidateA;
                                n = candidateN;
                                return true;
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
            }

            return false;
        }

    }
}
