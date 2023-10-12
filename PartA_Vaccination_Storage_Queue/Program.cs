using Azure.Storage.Queues;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;

Console.ForegroundColor= ConsoleColor.Green;
Console.WriteLine("******Welcome to the Queue Storage Function App******");

//connection string to storage queue
string connection = "DefaultEndpointsProtocol=https;AccountName=saluciany;AccountKey=mWm/9/G4F04OQX/G/XJWTqbx3o92aYlE5uxA1i3AV/s6GYdEpj36S3wDNgCvjQ2kwzcOKRofxPhB+AStH6CW5w==;EndpointSuffix=core.windows.net";

Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("\nCreating queue...");

//gets or creates the queue
string queueName = "vaccination-queue";
QueueClient client = new QueueClient(connection, queueName);
await client.CreateIfNotExistsAsync();

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("\nQueue Created.");


//inserting into queue

//random number generator
Random generator = new Random();

//diff center names
string[] vaccinationCenters = { "La Lucia Clinic","Kingsburg Clinic", "Adams Clinic", "Bluff Clinic", "Cape Canal Clinic" };

//random date
int range = 5 * 365; //5 years          

Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine($"\nAdding to: {queueName}");

//sends message to queue
for (int i = 0; i < 5; i++)
{
    String id = generator.Next(0, 999999).ToString("D13");
    DateTime date = DateTime.Today.AddDays(-generator.Next(range));
    string serialNumber = "" + generator.Next(100000, 1000000);

    await client.SendMessageAsync($"{id}:{vaccinationCenters[i]}:{date}:{serialNumber}");
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"\nMessages added to {queueName}");