﻿using System;
using System.Linq;
using RepeaterBook;
using RepeaterBook.Export;

namespace RepeaterBookConsole
{
    internal class Program
    {
        private static DataManager DataManager = new DataManager();

        private static void Main(string[] args)
        {
            DataManager.Initialize();

            if (args != null && args.Any() && args[0] == "custom")
            {
                var coordinates = new Coordinates(53.582710, -123.407596);
                var filterByLocation = DataManager.FilterByLocation(coordinates, 100, UnitOfLength.Kilometers);

                var chirp = new ChirpExporter();
                chirp.ExportFolders(@"C:\Users\rchartier\Desktop\fingerlake.csv", filterByLocation);

                var kml = new KMLExporter();
                kml.ExportFolders(@"C:\Users\rchartier\Desktop\fingerlake.kml", filterByLocation);

                Console.WriteLine("Done");
                Console.ReadLine();
            }
            else
            {
                ExportByLocation();
            }
        }

        private static double GetDoubleFromUser(string title)
        {
            while (true)
            {
                Console.WriteLine(title);
                var latInput = Console.ReadLine();
                if (!string.IsNullOrEmpty(latInput))
                {
                    double input = 0;
                    if (double.TryParse(latInput, out input))
                    {
                        if (input != 0)
                        {
                            return input;
                        }
                    }
                }
                Console.WriteLine("Invalid input.  Try again.");
            }
        }

        private static string GetStringFromUser(string title, string[] allowedInputs = null)
        {
            var input = "";
            while (string.IsNullOrEmpty(input))
            {
                Console.WriteLine(title);
                input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Invalid input, Try again.");
                    continue;
                }

                if (allowedInputs != null)
                {
                    if (!allowedInputs.Contains(input))
                    {
                        Console.WriteLine("Invalid input, Try again.");
                        input = "";
                        continue;
                    }
                }
            }

            return input;
        }

        private static void ExportByLocation()
        {
            var lat = GetDoubleFromUser("Latitude?");
            var lon = GetDoubleFromUser("Longitude?");
            var distance = GetDoubleFromUser("Radius (in kilometers)?");
            var fileName = GetStringFromUser("Filename?");
            var format = GetStringFromUser("Format (KML or CHIRP)?");

            var coordinates = new Coordinates(lat, lon);

            Console.WriteLine($"Filtering data by: Latitude:{lat}, Longitude:{lon} with a radius of {distance}.");

            var filterByLocation = DataManager.FilterByLocation(coordinates, distance, UnitOfLength.Kilometers);

            Console.WriteLine($"Filtering data complete.  Found:{filterByLocation.Count} locations");

            if (filterByLocation.Count <= 0)
            {
                Console.WriteLine("Nothing to export.  Aborting...");
                return;
            }

            if (format.Equals("KML", StringComparison.InvariantCultureIgnoreCase))
            {
                var kml = new KMLExporter();
                kml.ExportFolders(fileName, filterByLocation);
            }
            else
            {
                var chirp = new ChirpExporter();
                chirp.ExportFolders(fileName, filterByLocation);
            }

            Console.WriteLine($"Done writing data to the file:{fileName}, exiting.");
        }
    }
}