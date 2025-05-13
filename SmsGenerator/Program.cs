using System;
using System.Collections.Generic;


namespace SmsGenerator
{
    class Program
    {
        static Random rnd = new Random();

        //функция для проверки пользовательского ввода
        static long ReadInput(string request)
        {
            Console.WriteLine(request);
            long validInput;
            while (!long.TryParse(Console.ReadLine(), out validInput) || validInput <=0)
            {
                Console.WriteLine("Неверный ввод! Введите целое положительное число!");
            }

            return validInput;
        }

        //Расчет остатка от деления
        static long EuclidsAlgorithm(long a, long n)
        {
            while (n != 0)
            {
                long r = a % n;
                a = n;
                n = r;
            }

            return a;
        }

        //функция Эйлера для вычисляет сколько взаимно простых чисел есть  с заданным числом
        static long EulerFunction(long n)
        {
            long cnt = 0;
            for (long i = 1; i <= n; i++)
                if (EuclidsAlgorithm(i, n) == 1) cnt++;
            return cnt;
        }

        //Возведение в степень
        static long PowMod(long a, long k, long n)
        {
            long res = 1;
            for (long i = 0; i < k; i++)
                res = (res * a) % n;
            return res;
        }

        //Создание кодов

        static List<long> MakeCodes(long a, long n, long len, long amount)
        {
            long phi = EulerFunction(n);
            long min = 1;
            for (int i = 1; i < len; i++) min *= 10;
            long maxPossible = Math.Min(n - 1, (long)Math.Pow(10, len) - 1);

            if (min > maxPossible)
                throw new ArgumentException($"При n = {n} невозможно получить {len}-значные остатки.");
            var codes = new HashSet<long>();
            long tries = 0, limit = amount * 100; //исключение цикла
            while (codes.Count < amount && tries < limit)
            {
                long k = rnd.Next(1, (int)(phi > int.MaxValue ? int.MaxValue : phi + 1));
                long value = PowMod(a, k, n);

                if (value >= min && value <= maxPossible)
                    codes.Add(value);

                tries++; 
            }

            if (codes.Count < amount)
                Console.WriteLine($"Предупреждение: удалось набрать только {codes.Count} уникальных кодов.");

            return new List<long>(codes);
        }

        static bool FindSuitableParameters(long len, long amount, out long a, out long n)
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
                    if (EuclidsAlgorithm(candidateA, candidateN) == 1)
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

        static void Main(string[] args)
        {
            //Рандомная генерация взаимнопростых чисел

            long codeLength = ReadInput("Введите длину одного кода от 1 до 9: ");
            while (codeLength < 1 || codeLength > 9)
            {
                Console.WriteLine("Длина кода должна быть от 1 до 9 цифр!");
                codeLength = ReadInput("Введите длину одного кода от 1 до 9: ");
            }

            long codeCount = ReadInput("Сколько кодов сгенерировать? ");

            // Подбираем подходящие параметры a и n
            if (!FindSuitableParameters(codeLength, codeCount, out long a, out long n))
            {
                Console.WriteLine("Не удалось найти подходящие параметры для генерации запрошенного количества кодов.");
                Console.WriteLine("Попробуйте уменьшить количество кодов или увеличить их длину.");
                return;
            }

            Console.WriteLine($"a = {a}, n = {n} — взаимно-простые");

            Console.WriteLine($"Функция Эйлера φ(n) = {EulerFunction(n)}");
            Console.WriteLine($"Минимальное значение: {Math.Pow(10, codeLength - 1)}, Максимальное: {Math.Pow(10, codeLength) - 1}");

            try
            {
                var codes = MakeCodes(a, n, codeLength, codeCount);

                Console.WriteLine($"\nГотово! Сгенерировано {codes.Count} уникальных {codeLength}-значных кодов:");
                foreach (var c in codes) Console.WriteLine(c.ToString().PadLeft((int)codeLength, '0'));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}
