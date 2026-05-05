using Serilog;
using System;
using System.Collections.Generic;

namespace Lab1_Smirnov
{
    class Program
    {
        static void Main(string[] args)
        {
            string templ = "{Timestamp:HH:mm:ss} | [{Level:u3}] | {Message:lj}{NewLine}{Exception}";
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(outputTemplate: templ)
                .WriteTo.File("logs/log.txt", outputTemplate: templ)
                .CreateLogger();

            Log.Information("Программа запущена");

            while (true)
            {
                Console.WriteLine("\nВведите стороны треугольника (exit для выхода):");
                Console.Write("Сторона A: ");
                string a = Console.ReadLine();
                if (a == "exit") break;
                Console.Write("Сторона B: ");
                string b = Console.ReadLine();
                Console.Write("Сторона C: ");
                string c = Console.ReadLine();

                var result = CalculateTriangle(a, b, c);

                Console.WriteLine($"Тип треугольника: {result.type}");
                Console.WriteLine($"Координаты: A({result.coords[0].x},{result.coords[0].y}) B({result.coords[1].x},{result.coords[1].y}) C({result.coords[2].x},{result.coords[2].y})");
            }

            Log.Information("Программа завершена");
            Log.CloseAndFlush();
            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }

        public static (string type, List<(int x, int y)> coords) CalculateTriangle(string s1, string s2, string s3)
        {
            // проверка на числа
            if (!float.TryParse(s1, out float a) || !float.TryParse(s2, out float b) || !float.TryParse(s3, out float c))
            {
                Log.Warning("Нечисловые данные: {A} {B} {C}", s1, s2, s3);
                return ("", new List<(int, int)> { (-2, -2), (-2, -2), (-2, -2) });
            }

            // проверка на положительность
            if (a <= 0 || b <= 0 || c <= 0)
            {
                Log.Warning("Отрицательные или нулевые стороны: {A} {B} {C}", a, b, c);
                return ("не треугольник", new List<(int, int)> { (-1, -1), (-1, -1), (-1, -1) });
            }

            // неравенство треугольника
            if (a + b <= c || a + c <= b || b + c <= a)
            {
                Log.Warning("Не треугольник: {A} {B} {C}", a, b, c);
                return ("не треугольник", new List<(int, int)> { (-1, -1), (-1, -1), (-1, -1) });
            }

            // определение типа
            string type;
            if (Math.Abs(a - b) < 0.001f && Math.Abs(b - c) < 0.001f)
                type = "равносторонний";
            else if (Math.Abs(a - b) < 0.001f || Math.Abs(a - c) < 0.001f || Math.Abs(b - c) < 0.001f)
                type = "равнобедренный";
            else
                type = "разносторонний";

            // вычисление координат
            var coords = CalculateCoordinates(a, b, c);

            Log.Information("УСПЕХ: стороны={A},{B},{C} тип={Type} координаты=({AX},{AY}),({BX},{BY}),({CX},{CY})",
                a, b, c, type,
                coords[0].x, coords[0].y,
                coords[1].x, coords[1].y,
                coords[2].x, coords[2].y);

            return (type, coords);
        }

        static List<(int x, int y)> CalculateCoordinates(float a, float b, float c)
        {
            // вершина A в (0,0), вершина B в (c,0), вершина C по формуле
            float xC = (b * b + c * c - a * a) / (2 * c);
            float yC = (float)Math.Sqrt(b * b - xC * xC);

            // масштабирование под поле 100х100 с отступами
            float maxDim = Math.Max(c, Math.Max(xC, yC));
            float scale = 80f / maxDim;

            int ax = 10;
            int ay = 90;
            int bx = 10 + (int)(c * scale);
            int by = 90;
            int cx = 10 + (int)(xC * scale);
            int cy = 90 - (int)(yC * scale);

            return new List<(int, int)> { (ax, ay), (bx, by), (cx, cy) };
        }
    }
}