using CsvHelper.Configuration.Attributes;

namespace CsvHandler
{
    public class CsvModel
    {
        [Index(0)]
        public string UserId { get; set; }
        [Index(1)]
        public string FirstName { get; set; }
        [Index(2)]
        public string LastName { get; set; }
        [Index(3)]
        public int Version { get; set; }
        [Index(4)]
        public string InsuranceCompany { get; set; }
    }
}
