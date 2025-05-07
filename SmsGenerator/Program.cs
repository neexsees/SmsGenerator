using System;

namespace SmsGenerator
{
    class Program
    {
        //функция для проверки пользовательского ввода
        static int ReadInput(string request)
        {
            Console.WriteLine(request);
            int validInput;
            while (!int.TryParse(Console.ReadLine(), out validInput))
            {
                Console.WriteLine("Неверный ввод! Введите целое положительное число!");
            }
            return validInput;
        }

        static void Main(string[] args)
        {
            int codeLength = ReadInput("Какой длины создать код?");
            int codeNumber = ReadInput("Сколько кодов создать?");
        }
    }
}
