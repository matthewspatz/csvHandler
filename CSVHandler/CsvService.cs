using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace CsvHandler
{
    public class CsvService
    {
        public List<CsvModel> ReadCsv(string filePath)
        {
            // Assuming .csv has no header row since specific names for the columns were not included in the excercise
            // If a header does exist, the Read() method would just need to be invoked once before the while loop
            // is entered by uncommenting line 27
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };

            var records = new List<CsvModel>();

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                //csv.Read();
                while (csv.Read())
                {
                    var record = new CsvModel
                    {
                        UserId = csv.GetField(0),
                        // I wasn't sure from the wording of the excercise if the First and Last names are in separate 
                        // columns in the import file or if it was expected they would need to be parsed from a single
                        // column.  If they are separated in the import, these lines of code should be used:
                        FirstName = csv.GetField(1),
                        LastName = csv.GetField(2),
                        Version = csv.GetField<int>(3),
                        InsuranceCompany = csv.GetField(4)

                        // If they are not separate, the following lines would be used instead
                        //FirstName = csv.GetField(1).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0],
                        //LastName = csv.GetField(1).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1],
                        //Version = csv.GetField<int>(2),
                        //InsuranceCompany = csv.GetField(3)
                    };
                    records.Add(record);
                }
            }
            return records;
        }

        public void WriteCsv(List<CsvModel> records, string filePath)
        {
            // In this case I chose to write the new csv files keeping the first and last name distinct since
            // in practical application that would almost certainly be preferrable.
            // If that were not desired, a conversion like the one in the ReadCsv method could be added to this logic
            // to create a matching column set, just in this case the first and last name columns would be concatenated.

            // Also, as above no header is included in the new files but could be added by uncommenting my code at
            // lines 57-58
            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                //csv.WriteHeader<CsvModel>();
                //csv.NextRecord();
                foreach (var record in records)
                {
                    csv.WriteRecord(record);
                    csv.NextRecord();
                }
            }
        }
    }
}
