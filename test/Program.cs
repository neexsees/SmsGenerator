using System;
using System.Collections.Generic;

namespace SmsGenerator
{
    class Program
    {
        static Random rnd = new Random();

        static int ReadInput(string request)
        {
            Console.WriteLine(request);
            int validInput;
            while (!int.TryParse(Console.ReadLine(), out validInput) || validInput <= 0)
            {
                Console.WriteLine("Введите положительное целое число!");
            }
            return validInput;
        }

        public static int GCD(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return Math.Abs(a);
        }

        // Функция Эйлера
        public static int EulerTotient(int n)
        {
            int result = n;
            for (int i = 2; i * i <= n; i++)
            {
                if (n % i == 0)
                {
                    while (n % i == 0)
                        n /= i;
                    result -= result / i;
                }
            }
            if (n > 1)
                result -= result / n;
            return result;
        }

        // Возведение в степень по модулю
        public static int ModularPow(int baseValue, int exponent, int mod)
        {
            int result = 1;
            baseValue %= mod;
            while (exponent > 0)
            {
                if ((exponent & 1) == 1)
                    result = (result * baseValue) % mod;
                exponent >>= 1;
                baseValue = (baseValue * baseValue) % mod;
            }
            return result;
        }

        // Генерация кодов
        public static List<string> GenerateCodes(int a, int n, int count, int length)
        {
            List<string> codes = new List<string>();
            int current = 1;

            for (int i = 0; i < count; i++)
            {
                current = (current * a) % n;
                string code = current.ToString().PadLeft(length, '0');

                if (code.Length > length)
                    code = code.Substring(code.Length - length);

                codes.Add(code);
            }

            return codes;
        }

        static void Main(string[] args)
        {
            int codeLength = ReadInput("Введите длину одного кода (в цифрах):");
            int codeNumber = ReadInput("Сколько кодов сгенерировать?");

            int minMod = (int)Math.Pow(10, codeLength); // Минимальный n, чтобы коды были нужной длины

            int a, n;
            do
            {
                a = rnd.Next(2, 1000000);
                n = rnd.Next(minMod, minMod * 10); // n точно больше 10^длина
            }
            while (GCD(a, n) != 1);

            Console.WriteLine($"\nВыбранные параметры:\na = {a}, n = {n} (взаимно просты)");

            var codes = GenerateCodes(a, n, codeNumber, codeLength);

            Console.WriteLine("\nСгенерированные коды:");
            foreach (var code in codes)
            {
                Console.WriteLine(code);
            }
        }
    }
}
