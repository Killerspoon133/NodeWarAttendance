using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceApp
{
    public class MainWorkflow
    {
        private IConfigurationRoot config;
        private List<Record> records;

        public MainWorkflow(IConfigurationRoot config)
        {
            this.config = config;
            records = new List<Record>();
        }

        public void Gather(string inputFolder)
        {
            var fileList = Directory.GetFiles(inputFolder, "*.txt");

            Console.WriteLine("num: " + fileList.Length);
            foreach (var file in fileList)
            {
                Console.WriteLine("File: " + file);
            }

            ReadInputs(fileList);
            AddRecordsToDB(records);
        }

        public void Export(string date)
        {
            SQLiteUtil sqliteUtil = new SQLiteUtil();
            SqliteConnection connection = sqliteUtil.CreateConnection();

            var familyNames = sqliteUtil.GetDistinctListOfFamilyNames(connection);
            StringBuilder finalCsvData = new StringBuilder();

            finalCsvData.AppendLine(BuildCsvHeader(date));

            foreach (var familyName in familyNames)
            {
                var records = sqliteUtil.GetRecordsForFamilyName(connection, familyName);
                finalCsvData.AppendLine(BuildCsvData(records, date));
            }

            connection.Close();

            CreateCsv(finalCsvData);
        }

        private void CreateCsv(StringBuilder finalCsvData)
        {
            var path = Environment.CurrentDirectory;

            using (StreamWriter writer = new StreamWriter(Path.Combine(path, "Export.csv")))
            {
                writer.Write(finalCsvData.ToString());
            }
        }

        private string BuildCsvHeader(string date)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Family Name,Guild Name,");

            DateTime myDate = DateTime.ParseExact(date, "MMyyyy", System.Globalization.CultureInfo.InvariantCulture);
            var days = DateTime.DaysInMonth(myDate.Year, myDate.Month);

            for (int i = 1; i <= days; i++)
            {
                if (i == days)
                {
                    sb.Append($"{myDate.Month}/{i}");
                }
                else
                {
                    sb.Append($"{myDate.Month}/{i},");
                }
            }

            return sb.ToString();
        }

        private string BuildCsvData(List<Record> records, string exportDate)
        {
            DateTime myDate = DateTime.ParseExact(exportDate, "MMyyyy", System.Globalization.CultureInfo.InvariantCulture);
            var days = DateTime.DaysInMonth(myDate.Year, myDate.Month);

            StringBuilder sb = new StringBuilder();
            var firstRecord = records.First();

            sb.Append(firstRecord.FamilyName);
            sb.Append(',');
            sb.Append(firstRecord.GuildName);
            sb.Append(',');

            for(int i = 1; i <= days; i++) 
            {
                if (i == days)
                {
                    if (HasRecordForDay(records, i, myDate.Month))
                    {
                        sb.Append("o");
                    }
                    else
                    {
                        sb.Append("x");
                    }
                }
                else
                {                    
                    if (HasRecordForDay(records, i, myDate.Month))
                    {
                        sb.Append("o,");
                    }
                    else
                    {
                        sb.Append("x,");
                    }
                }
            }

            return sb.ToString();
        }

        private bool HasRecordForDay(List<Record> records, int day, int month) 
        {
            foreach (var record in records) 
            {
                DateTime recordDate = new DateTime(record.WarDate);
                if( day == recordDate.Day && month == recordDate.Month ) 
                {
                    return true;
                }
            }

            return false;
        }

        private void AddRecordsToDB(List<Record> records)
        {
            SQLiteUtil sqliteUtil = new SQLiteUtil();
            SqliteConnection connection = sqliteUtil.CreateConnection();
            sqliteUtil.CreateTable(connection);

            foreach (var record in records)
            {
                sqliteUtil.InsertData(connection, record);
            }

            connection.Close();
        }

        private void ReadInputs(string[] fileList)
        {
            foreach (var file in fileList)
            {
                List<string> names = new List<string>();
                var filename = Path.GetFileNameWithoutExtension(file);
                var guild = filename.Split('_')[0];
                var date = filename.Split('_')[1];

                using (var streamReader = new StreamReader(file))
                {
                    while (!streamReader.EndOfStream)
                    {
                        names.Add(streamReader.ReadLine());
                    }
                }
                var FamilyNames = GetFamilyNames(names);
                CreateRecords(FamilyNames, guild, date);

                // Move file to processed after?

            }
        }

        private List<string> GetFamilyNames(List<string> rawNames)
        {
            List<string> familyNames = new List<string>();

            foreach (var name in rawNames)
            {
                var index = name.IndexOf('(');
                familyNames.Add(name.Substring(0, index - 1).Replace(" ", string.Empty));
            }

            return familyNames;
        }

        private void CreateRecords(List<string> familyNames, string guild, string date)
        {
            var dateTime = DateTime.ParseExact(date, "MMddyyyy", System.Globalization.CultureInfo.InvariantCulture);
            foreach (var familyName in familyNames)
            {
                var record = new Record()
                {
                    FamilyName = familyName,
                    GuildName = guild,
                    WarDate = dateTime.Ticks

                };
                records.Add(record);
            }
        }
    }
}
