using System;
using System.Collections.Generic;

namespace SmsGenerator
{
    class Program
    {
        static Random rnd = new Random();

        //функция для проверки пользовательского ввода
        static int ReadInput(string request)
        {
            Console.WriteLine(request);
            int validInput;
            while (!int.TryParse(Console.ReadLine(), out validInput) || validInput <=0)
            {
                Console.WriteLine("Неверный ввод! Введите целое положительное число!");
            }

            return validInput;
        }

        //Расчет остатка от деления
        static int EuclidsAlgorithm(int a, int n)
        {
            while (n != 0)
            {
                int r = a % n;
                a = n;
                n = r;
            }

            return a;
        }

        //функция Эйлера для подсчета количества делителей n
        static List<int> EulerFunction(int n)
        {
            List<int> list = new List<int>();
            for (int i = 1; i <= n; i++)
                if (n % i == 0)
                    list.Add(i);
            return list;
        }


        static void Main(string[] args)
        {
            //Рандомная генерация взаимнопростых чисел
            int a, n;
            do
            {
                a = rnd.Next(1, 100);
                n = rnd.Next(1, 100);
            } while ((EuclidsAlgorithm(a,n) != 1)); //пока не станет равным 1, то есть не станут взаимно простыми

            //Только для разработчиков(строку не должен видеть пользователь)
            Console.WriteLine($"a = {a}, n = {n} — взаимно простые!");

            //делители n
            var divs = EulerFunction(n);
            Console.WriteLine($"Делители n ({divs.Count} шт.): {string.Join(", ", divs)}");

            int codeLength = ReadInput("Введите длину одного кода (в цифрах):");
            int codeNumber = ReadInput("Сколько кодов сгенерировать?");
        }
    }
}
