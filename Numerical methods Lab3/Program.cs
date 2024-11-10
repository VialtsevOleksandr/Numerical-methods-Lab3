using System;
using System.Dynamic;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Web;
using System.Xml.Linq;

// { x^2 - y^2 / 9 + z^2 = 0
// { x^2 / 4 + y^2 + z^2 / 9 = 1

// x = sqrt(y^2 / 9 - z^2)
// y = sqrt(1 - x^2 / 4 - z^2 / 9)

// J:
// { 0 y }
// { x 0 }
class Program
{
    class res
    {
        public double x;
        public double y;
        public double z;
        public res(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public void PrintRes(int i)
        {
            Console.WriteLine($"x{i+1} = {x.ToString("F8")}, y{i + 1} = {y.ToString("F8")}, z{i + 1} = {z.ToString("F2")}");
        }

        public double INFNorm(double x, double y)
        {
            return Math.Max(Math.Abs(x),Math.Abs(y));
        }
    }
    static double GetZm()
    {
        double Zm;
        Console.Write("Введіть Zm: ");
        var position = Console.GetCursorPosition();
        bool res = false;
        do
        {
            res = double.TryParse(Console.ReadLine(), out Zm);
            if (!res)
            {
                ConsoleClear(position);
            }
        } while (!res);
        position = Console.GetCursorPosition();
        Console.Write("                       ");
        Console.SetCursorPosition(position.Left, position.Top);
        return Zm;
    }

    static double GetX0()
    {
        double x0;
        Console.Write("Введіть x0: ");
        var position = Console.GetCursorPosition();
        bool res = false;
        do
        {
            res = double.TryParse(Console.ReadLine(), out x0);
            if (!res)
            {
                ConsoleClear(position);
            }
        } while (!res);
        position = Console.GetCursorPosition();
        Console.Write("                       ");
        Console.SetCursorPosition(position.Left, position.Top);
        return x0;
    }
    static double GetY0()
    {
        double y0;
        Console.Write("Введіть y0: ");
        var position = Console.GetCursorPosition();
        bool res = false;
        do
        {
            res = double.TryParse(Console.ReadLine(), out y0);
            if (!res)
            {
                ConsoleClear(position);
            }
        } while (!res);
        position = Console.GetCursorPosition();
        Console.Write("                       ");
        Console.SetCursorPosition(position.Left, position.Top);
        return y0;
    }

    static double xPositive(double y, double Zm)
    {
        return Math.Sqrt(Math.Pow(y, 2) / 9 - Math.Pow(Zm, 2));
    }

    static double x1Positive(double x, double Zm) //ф2(x`)
    {
        return -(3 * x / (2 * Math.Sqrt(36 - 9 * Math.Pow(x, 2) - 4 * Math.Pow(Zm, 2))));
    }

    static double xNegative(double y, double Zm)
    {
        return -Math.Sqrt(Math.Pow(y, 2) / 9 - Math.Pow(Zm, 2));
    }

    static double x1Negative(double x, double Zm)
    {
        return 3 * x / (2 * Math.Sqrt(36 - 9 * Math.Pow(x, 2) - 4 * Math.Pow(Zm, 2)));
    }
    static double yPositive(double x, double Zm)
    {
        return Math.Sqrt(1 - Math.Pow(x, 2) / 4 - Math.Pow(Zm, 2) / 9);
    }

    static double y1Positive(double y, double Zm) //ф1(y`)
    {
        return y / (3 * Math.Sqrt(Math.Pow(y, 2) - 9 * Math.Pow(Zm, 2)));
    }

    static double yNegative(double x, double Zm)
    {
        return -Math.Sqrt(1 - Math.Pow(x, 2) / 4 - Math.Pow(Zm, 2) / 9);
    }

    static double y1Negative(double y, double Zm)
    {
        return -y / (3 * Math.Sqrt(Math.Pow(y, 2) - 9 * Math.Pow(Zm, 2)));
    }


    static void FindSolutions(double Zm, double x0, double y0, double ε)
    {
        double[,] J = new double[2, 2];
        res[] results = new res[4];
        results[0] = new res(x0, y0, Zm);
        results[1] = new res(x0, y0, Zm);
        results[2] = new res(x0, y0, Zm);
        results[3] = new res(x0, y0, Zm);

        for (int i = 0; i < 4; i++)
        {
            double xi, yi;
            do
            {
                xi = results[i].x;
                yi = results[i].y;

                switch (i)
                {
                    case 0:
                        J[0, 1] = y1Positive(y0, Zm);
                        J[1, 0] = x1Positive(x0, Zm);

                        results[i].x = xPositive(yi, Zm);
                        results[i].y = yPositive(xi, Zm);
                        break;
                    case 1:
                        J[0, 1] = y1Negative(y0, Zm);
                        J[1, 0] = x1Positive(x0, Zm);

                        results[i].x = xPositive(yi, Zm);
                        results[i].y = yNegative(xi, Zm);
                        break;
                    case 2:
                        J[0, 1] = y1Positive(y0, Zm);
                        J[1, 0] = x1Negative(x0, Zm);

                        results[i].x = xNegative(yi, Zm);
                        results[i].y = yPositive(xi, Zm);
                        break;
                    case 3:
                        J[0, 1] = y1Negative(y0, Zm);
                        J[1, 0] = x1Negative(x0, Zm);

                        results[i].x = xNegative(yi, Zm);
                        results[i].y = yNegative(xi, Zm);
                        break;
                }
            } while (results[i].INFNorm(results[i].x - xi, results[i].y - yi) >= ε && ConvergenceCheck(J));
            results[i].PrintRes(i);
        }
    }

    static void ConsoleClear((int Left, int Top) position)
    {
        Console.Write("Коректно введіть число!");
        Console.SetCursorPosition(position.Left, position.Top);
        Console.Write("                       ");
        Console.SetCursorPosition(position.Left, position.Top);
    }
    static void GetInfo()
    {
        Console.WriteLine("Натисніть:\n" +
           "1 -- для вводу Zm\n" +
           "2 -- для очищення консолі\n" +
           "3 -- для виходу\n" +
           "4 -- для перегляду інформації\n");
    }

    static double GetE()
    {
        double ε;
        Console.Write("Вкажiть точнiсть ε з проміжку (0;1).       ε = ");
        var position = Console.GetCursorPosition();
        bool res = false;
        do
        {
            res = double.TryParse(Console.ReadLine(), out ε);
            if (!res || ε <= 0 || ε >= 1)
            {
                ConsoleClear(position);
            }
        } while (!res || ε <= 0 || ε >= 1);
        {
            position = Console.GetCursorPosition();
            Console.Write("                       ");
            Console.SetCursorPosition(position.Left, position.Top);
        }
        return ε;
    }

    static void PrintMatrix(double[,] matrix)
    {
        int n = matrix.GetLength(0);
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (matrix[i, j] == 0)
                    Console.Write("0\t");
                else
                    Console.Write(matrix[i, j].ToString("F2") + "\t");
            }
            Console.WriteLine();
        }
    }

    static double NormMatrix(double[,] matrix)
    {
        int n = matrix.GetLength(0);
        double max = 0;
        for (int i = 0; i < n; i++)
        {
            double sum = 0;
            for (int j = 0; j < n; j++)
                sum += Math.Abs(matrix[i, j]);
            if (sum > max)
                max = sum;
        }
        return max;
    }

    static bool ConvergenceCheck(double[,] J)
    {
        if (NormMatrix(J) < 1)
        {
            //Console.WriteLine("Норма матриці Якобі менше 1, а отже є збіжність");
            return true;
        }
        else
        {
            Console.WriteLine("Норма матриці Якобі більше 1, а отже немає збіжності");
            return false;
        }
    }

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        GetInfo();
        Console.WriteLine("---------------Метод простої ітерації---------------\n");
        char choice;
        res res = new res(0, 0, 0);
        do
        {
            Console.Write("Обрана клавіша -- ");
            var position = Console.GetCursorPosition();
            choice = Console.ReadKey().KeyChar;
            Console.WriteLine();
            switch (choice)
            {
                case '1':
                    res.z = GetZm();
                    res.x = GetX0();
                    res.y = GetY0();

                    double ε = GetE();
                    FindSolutions(res.z, res.x, res.y, ε);

                    break;
                case '2':
                    Console.Clear();
                    GetInfo();
                    break;
                case '3':
                    Console.WriteLine("---------------Вихід---------------");
                    break;
                case '4':
                    GetInfo();
                    break;
                default:
                    Console.SetCursorPosition(position.Left, position.Top);
                    Console.WriteLine("                     ");
                    Console.SetCursorPosition(0, position.Top);
                    break;
            }
        } while (choice != '3');
    }
}
