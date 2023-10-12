using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace PartB_Vaccination_Queue_Function
{
    public class Function1
    {
        private string dbConnectionString()
        {
            string conString = "";
            return conString;
        }
        [FunctionName("Function1")]
        public void Run([QueueTrigger("vaccination-queue", Connection = "connection")]string myQueueItem, ILogger log)
        {
            string databaseConnection = dbConnectionString();

            try
            {
                string[] split = myQueueItem.Split(':'); 
                string id = split[0];
                string vaccineCenter = split[1];
                string date = split[2].ToString();
                int serialNumber = int.Parse(split[3]);

                log.LogInformation("Processing message queue...");

                using (SqlConnection con = new SqlConnection(databaseConnection))
                {
                    con.Open();
                    using (SqlCommand com = con.CreateCommand())
                    {
                        com.CommandText = "INSERT INTO VACCINATOR () VALUES (@Id, @Center, @Date, @Serial_Number) ";
                        com.Parameters.AddWithValue("Id", id);
                        com.Parameters.AddWithValue("Center", vaccineCenter);
                        com.Parameters.AddWithValue("Date", date);
                        com.Parameters.AddWithValue("Serial_Number", serialNumber);
                        com.ExecuteNonQuery();
                    }
                }

                log.LogInformation($"Message queue stored sucessfully. Information stored:\nId: {id}\nVaccination Center: {vaccineCenter}\nDate: {date}: Serial Number: {serialNumber}");
            }
            catch (Exception ex)
            {
                log.LogError($"Error processing message in queue: {ex}");
            }
        }
    }
}
