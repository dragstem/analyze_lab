using System.Diagnostics;
using System.Drawing;

namespace SoLE
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите режим работы алгоритма:\n1.Ручной\n2.Автоматический");
            int number = int.Parse(Console.ReadLine());
            bool input_mode = false;
            if (number == 1 ) { 
                input_mode = true;
            }
            if (number == 2)
            {
                input_mode = false;
            }
            if (input_mode)
            {
                Console.WriteLine("Введите размерность первой квадратной матрицы одной цифрой");
                int size1 = int.Parse(Console.ReadLine());
                Console.WriteLine("Введите размерность второй квадратной матрицы одной цифрой");
                int size2 = int.Parse(Console.ReadLine());

                var a = new Nmatrix(size1, size1, 100);
                var b = new Nmatrix(size2, size2, 100);

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var result = a * b;
                stopwatch.Stop();
                Console.WriteLine("Тривиальный алгоритм занял в миллисекундах: ");
                Console.WriteLine(stopwatch.ElapsedMilliseconds);

                stopwatch.Reset();

                stopwatch.Start();
                var result2 = a % b;
                Console.WriteLine("Алгоритм Штрассена занял в миллисекундах: ");
                Console.WriteLine(stopwatch.ElapsedMilliseconds);
            }
            else
            {
                using (StreamWriter writer = new StreamWriter("output.txt"))
                {
                    writer.WriteLine("размерность матриц | тривиальный способ | метод штрассена");
                    for (int i = 2; i <= 2048; i *= 2)
                    {
                        var a = new Nmatrix(i, i, 100);
                        var b = new Nmatrix(i, i, 100);

                        var stopwatch = new Stopwatch();

                        stopwatch.Start();
                        var result1 = a * b;
                        stopwatch.Stop();
                        var time1 = stopwatch.ElapsedMilliseconds;

                        stopwatch.Reset();

                        stopwatch.Start();
                        var result2 = a % b;
                        stopwatch.Stop();
                        var time2 = stopwatch.ElapsedMilliseconds;

                        writer.WriteLine($"{i} | {time1} | {time2}");
                    }
                }
            }
        }
    }
}
