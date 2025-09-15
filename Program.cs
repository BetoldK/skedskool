using Azure.Storage.Queues;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration["StorageConnectionString"];
string queueName = "bertoldstestqueue";
builder.Services.AddSingleton(x => new QueueClient(connectionString, queueName));

var app = builder.Build();

app.MapGet("/", async (QueueClient queueClient) =>
{
    string message = $"API was visited at: {DateTime.UtcNow:o}";

    await queueClient.CreateIfNotExistsAsync();
    await queueClient.SendMessageAsync(message);
    return $"✅ Message sent to the queue: '{message}'";
});

app.Run();