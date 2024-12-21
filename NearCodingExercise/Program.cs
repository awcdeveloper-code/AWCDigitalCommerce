using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.IO.Ports;
using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace NearCodingExercise
{
    class Program
    {
        static Dictionary<string, List<double[]>> shapes = new Dictionary<string, List<double[]>>();

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                LoadFile(args[0]);
            }

            while (true)
            {
                Console.WriteLine("Enter the name of the shape, followed by its numeric parameters (type EXIT to finish)");

                string input = Console.ReadLine();

                string[] parts = input.Split(' ');
                string shapeName = parts[0].ToLower();

                if (shapeName.Contains("exit"))
                {
                    Console.WriteLine("Exiting program.");
                    Thread.Sleep(1000);
                    break;
                }

                switch (shapeName)
                {
                    case "circle":
                        if (parts.Length != 4)
                        {
                            Console.WriteLine("Invalid input, for the circle the numbers are the X and Y coordinates of the centre, followed by the radius.");
                            continue;
                        }
                        StoreShape(parts);
                        break;
                    case "square":
                        if (parts.Length != 4)
                        {
                            Console.WriteLine("Invalid input, for the square it is X and Y of one corner, followed by the length of the side.");
                            continue;
                        }
                        StoreShape(parts);
                        break;
                    case "rectangle":
                        if (parts.Length != 5)
                        {
                            Console.WriteLine("Invalid input, for the rectangle it is X and Y of one corner, followed by the two sides.");
                            continue;
                        }
                        StoreShape(parts);
                        break;
                    case "triangle":
                        if (parts.Length != 7)
                        {
                            Console.WriteLine("Invalid input, for the triangle it is the X and Y coordinates of the three vertices (six numbers in total).");
                            continue;
                        }
                        StoreShape(parts);
                        break;
                    case "donut":
                        if (parts.Length != 5)
                        {
                            Console.WriteLine("Invalid input, For the donut it is the X and Y of the centre followed by the two radiuses.");
                            continue;
                        }
                        StoreShape(parts);
                        break;
                    case "shapes":
                        Console.WriteLine("Reading stored shapes and their parameters:");
                        foreach (var shape in shapes)
                        {
                            Console.WriteLine($"Shape: {shape.Key}");

                            foreach (var paramArray in shape.Value)
                            {
                                Console.WriteLine($"  Parameters: [{string.Join(", ", paramArray)}]");
                            }
                        }
                        break;
                    case "help":
                        Console.WriteLine("");
                        Console.WriteLine("Find below the list of valid shapes:");
                        Console.WriteLine("");
                        Console.WriteLine("Circle, the numbers are the x and y coordinates of the centre followed by the radius.");
                        Console.WriteLine("Square, it is x and y of one corner followed by the length of the side.");
                        Console.WriteLine("Rectangle, it is x and y of one corner followed by the two sides.");
                        Console.WriteLine("Triangle, it is the x and y coordinates of the three vertices (six numbers in total).");
                        Console.WriteLine("Donut, it is the x and y of the centre followed by the two radiuses.");
                        Console.WriteLine("");
                        break;
                    case "load":
                        LoadFile(parts[1].ToLower());
                        break;
                }
            }
        }

        static void StoreShape(string[] parts)
        {
            string shapeName = parts[0].ToLower();
            List<double> parameters = new List<double>();

            for (int i = 1; i < parts.Length; i++)
            {
                if (double.TryParse(parts[i], out double param))
                {
                    parameters.Add(param);
                }
                else
                {
                    Console.WriteLine($"Invalid numeric parameter: {parts[i]}. Skipping.");
                }
            }

            if (parameters.Count == 0)
            {
                Console.WriteLine("No valid numeric parameters provided. Please try again.");
                return;
            }

            if (!shapes.ContainsKey(parts[0].ToLower()))
            {
                shapes[shapeName] = new List<double[]>();
            }

            shapes[shapeName].Add(parameters.ToArray());
        }

        static void LoadFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                Console.WriteLine(Environment.NewLine + "Loading file, please wait...");

                using (StreamReader sr = new StreamReader(fileName))
                {
                    string line;
                    string[] parts;

                    while ((line = sr.ReadLine()) != null)
                    {
                        Console.WriteLine($"Loading shape {line}");
                        parts = line.Split(' ');
                        StoreShape(parts);
                    }
                }
                Console.WriteLine(Environment.NewLine + "File loaded." + Environment.NewLine);
            }
            else
            {
                Console.WriteLine(Environment.NewLine + "File was not found, please check.");
            }
        }
    }
}

