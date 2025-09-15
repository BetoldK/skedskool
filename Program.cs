using Azure.Storage.Queues;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration["StorageConnectionString"];
string queueName = "bertoldstestqueue";
builder.Services.AddSingleton(x => new QueueClient(connectionString, queueName));

var app = builder.Build();

app.MapGet("/", async (QueueClient queueClient) =>
{
    string originalMessage = $"API was visited at: {DateTime.UtcNow:o}";
    
    byte[] messageBytes = Encoding.UTF8.GetBytes(originalMessage);
    string encodedMessage = System.Convert.ToBase64String(messageBytes);

    await queueClient.CreateIfNotExistsAsync();
    await queueClient.SendMessageAsync(encodedMessage);
    return $"✅ Encoded message sent to the queue: '{encodedMessage}'";
});

app.Run();