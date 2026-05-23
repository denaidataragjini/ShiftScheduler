using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace ShiftScheduler.Services
{
    public class CsvLoaderService
    {
        public List<T> Load<T>(string path)
        {
            using var reader = new StreamReader(path);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null
            };

            using var csv = new CsvReader(reader, config);

            return csv.GetRecords<T>().ToList();
        }
    }
}
