// Program.cs
using Azure.Storage.Queues;

var builder = WebApplication.CreateBuilder(args);

// --- START: Added for Azure Queues ---

// 1. Get the connection string from appsettings.json
string connectionString = builder.Configuration["StorageConnectionString"];
string queueName = "bertoldstestqueue";

// 2. Register the QueueClient as a singleton service for Dependency Injection
//    This creates one client instance that the entire application will share.
builder.Services.AddSingleton(x => new QueueClient(connectionString, queueName));

// --- END: Added for Azure Queues ---


var app = builder.Build();

app.MapGet("/", () => "Hello, Skedda!");

// --- START: New Endpoint to Send Messages ---

// 3. Add a new POST endpoint to send a message to the queue.
//    It accepts a simple text body.
app.MapPost("/sendmessage", async (HttpRequest request, QueueClient queueClient) =>
{
    // Read the message from the request body
    using var reader = new StreamReader(request.Body);
    string message = await reader.ReadToEndAsync();

    if (string.IsNullOrWhiteSpace(message))
    {
        return Results.BadRequest("Message body cannot be empty.");
    }

    // The QueueClient is automatically injected by the framework.
    // Ensure the queue exists before sending a message.
    await queueClient.CreateIfNotExistsAsync();

    // Send the message
    await queueClient.SendMessageAsync(message);

    return Results.Ok($"Message sent to queue '{queueClient.Name}': '{message}'");
});

// --- END: New Endpoint to Send Messages ---


app.Run();