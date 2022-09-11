using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace CsvHandler
{
    class Program
    {
        static void Main(string[] args)
        {
            string validFilePath = "";
            while (true)
            {
                Console.WriteLine("Enter path to file including full file name or type quit to exit and then hit enter:");
                var filePath = Console.ReadLine();
                if (filePath.ToLower() == "quit")
                    break;

                if (!File.Exists(filePath))
                {
                    Console.WriteLine("File could not be found. Please try again.");
                    continue;
                }

                // This is not a robust check as file extensions can be changed but without specific requirements
                // this simple check will prevent most mismatches.
                if (Path.GetExtension(filePath).ToLower() != ".csv")
                {
                    Console.WriteLine("File must be a CSV. Please try again.");
                    continue;
                }

                validFilePath = filePath;
                break;
            }

            if (validFilePath == "")
                return;

            var service = new CsvService();
            var records = new List<CsvModel>();
            try
            {
                records = service.ReadCsv(validFilePath);
            }
            catch (Exception)
            {
                Console.WriteLine("Something went wrong reading the import file, please check your environment and filetype. Press any key to exit.");
                Console.ReadKey();
                return;
            }

            var insuranceCompanies = records
                .Select(c => c.InsuranceCompany)
                .Distinct(StringComparer.CurrentCultureIgnoreCase).ToList();

            foreach (var company in insuranceCompanies)
            {
                var enrollees = records
                    .Where(e => e.InsuranceCompany == company).ToList();

                var duplicateGroups = enrollees
                    .GroupBy(e => e.UserId)
                    .Where(e => e.Count() > 1)
                    .Select(p => new List<CsvModel>(p)).ToList();

                if (duplicateGroups.Any())
                {
                    foreach (var group in duplicateGroups)
                    {
                        var newestVersion = group.Select(e => e.Version).Max();
                        var current = group.Where(e => e.Version == newestVersion).First();
                        enrollees.RemoveAll(e => e.UserId == current.UserId && e.Version != newestVersion);
                    }
                }

                var writeFilePath = String.Format("{0}\\{1}.csv", Path.GetDirectoryName(validFilePath), company);
                var sorted = enrollees.OrderBy(e => e.LastName).ThenBy(e => e.FirstName).ToList();
                try
                {
                    service.WriteCsv(sorted, writeFilePath);
                    Console.WriteLine("File {0} written.", writeFilePath);
                }
                catch (Exception)
                {
                    Console.WriteLine("Something went wrong writing file {0}. Press x to exit the program or any other key to attempt the next file.", writeFilePath);
                    if (Console.ReadKey().Key == ConsoleKey.X)
                        return;
                }
            }

            Console.WriteLine("Operation complete, press any key to exit.");
            Console.ReadKey();
        }
    }
}