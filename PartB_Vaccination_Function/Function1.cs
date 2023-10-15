using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace PartB_Vaccination_Queue_Function
{
    public class Function1
    {
        private IConfiguration _config;
        public Function1(IConfiguration config)
        {
            _config = config;
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
                    string[] split = myQueueItem.Split(':');
                    string id = split[0];
                    string vaccineCenter = split[1];

                    //add tryparse
                    string date = split[2].ToString();
                    int serialNumber = int.Parse(split[3]);

                    log.LogInformation("Processing message queue...");

                    using (SqlConnection con = new SqlConnection(databaseConnection))
                    {
                        con.Open();
                        using (SqlCommand com = con.CreateCommand())
                        {
                            com.CommandText = "INSERT INTO VACCINATOR (ID, Center, Date, Serial_Number) VALUES (@ID, @Center, @Date, @Serial_Number) ";
                            com.Parameters.AddWithValue("ID", id);
                            com.Parameters.AddWithValue("Center", vaccineCenter);
                            com.Parameters.AddWithValue("Date", date);
                            com.Parameters.AddWithValue("Serial_Number", serialNumber);
                            com.ExecuteNonQuery();
                        }
                    }
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine($"\nMessage queue stored sucessfully. Information stored:\nId: {id}\nVaccination Center: {vaccineCenter}\nDate: {date}: Serial Number: {serialNumber}");
                    log.LogInformation($"Message queue stored sucessfully. Information stored:\nId: {id}\nVaccination Center: {vaccineCenter}\nDate: {date}: Serial Number: {serialNumber}");
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

