using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace PartB_Vaccination_Queue_Function
{
    public class Function1
    {
        private IConfiguration _config;
        public Function1(IConfiguration config)
        {
            _config = config;
        }

        public class Values
        {
            public string id { get; set; }
            public string vaccineCenter { get; set; }
            public string date { get; set; }
            public int serialNumber { get; set; }

        }

        [FunctionName("Function1")]
        public void Run([QueueTrigger("vaccination-queue", Connection = "connection")] string myQueueItem, ILogger log)
        {
            Console.ForegroundColor= ConsoleColor.Yellow;
            Console.WriteLine("Establishing Database Connection...");
            //string databaseConnection = _config.GetConnectionString("db_connection");
            string databaseConnection = "Server=tcp:vaccination-queue-server.database.windows.net,1433;Initial Catalog=VaccinationQueue_DB;Persist Security Info=False;User ID=lucian;Password=Rainpush58!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            if (string.IsNullOrEmpty(databaseConnection))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nConnection to SQL Database Failed.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nConnection Established");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nProcessing Message(s) in Queue...");
                try
                {
                    Values obj = new Values();
                    Console.WriteLine(myQueueItem);

                    string[] split = myQueueItem.Split(':');
                    Console.WriteLine(split[0].Length);
                    if (split[0].Length == 13)
                    {
                        obj.id = split[0];
                        obj.vaccineCenter = split[1];
                        obj.date = split[2];
                        obj.serialNumber = int.Parse(split[3]);
                    }
                    else if (split[0].Length == 6)
                    {
                        obj.serialNumber = int.Parse(split[0]);
                        obj.date = split[1];
                        obj.vaccineCenter = split[2];
                        obj.id = split[3];
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        log.LogError($"Invaild Format: {myQueueItem}");
                    }

                    log.LogInformation("Processing message queue...");

                    using (SqlConnection con = new SqlConnection(databaseConnection))
                    {
                        con.Open();
                        using (SqlCommand com = con.CreateCommand())
                        {
                            com.CommandText = "INSERT INTO VACCINATOR (ID, Center, Date, Serial_Number) VALUES (@ID, @Center, @Date, @Serial_Number) ";
                            com.Parameters.AddWithValue("ID", obj.id);
                            com.Parameters.AddWithValue("Center", obj.vaccineCenter);
                            com.Parameters.AddWithValue("Date", obj.date);
                            com.Parameters.AddWithValue("Serial_Number", obj.serialNumber);
                            com.ExecuteNonQuery();
                        }
                    }
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine($"\nMessage queue stored sucessfully. Information stored:\nId: {obj.id}\nVaccination Center: {obj.vaccineCenter}\nDate: {obj.date}: Serial Number: {obj.serialNumber}");
                    log.LogInformation($"Message queue stored sucessfully. Information stored:\nId: {obj.id}\nVaccination Center: {obj.vaccineCenter}\nDate: {obj.date}: Serial Number: {obj.serialNumber}");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nError processing message(s) in queue: {ex}");
                    log.LogError($"Error processing message in queue: {ex}");
                }
            }
            Console.ReadLine();
        }
    }
}

