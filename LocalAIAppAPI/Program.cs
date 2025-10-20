using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configure the Ollama chat client
builder.Services.AddSingleton<IChatClient>(serviceProvider =>
    new OllamaChatClient(new Uri("http://localhost:11434/"), "phi3:mini"));

var app = builder.Build();

// Configure the HTTP request pipeline.

// Use CORS policy BEFORE other middleware
app.UseCors("AllowReactApp");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Only use HTTPS redirection in production to avoid CORS preflight issues
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// Chat API endpoint
app.MapPost("/chat", async (ChatRequest request, IChatClient chatClient) =>
{
    try
    {
        // Create chat history from the request
        List<ChatMessage> chatHistory = new();

        // Add conversation history if provided
        if (request.ChatHistory != null)
        {
            foreach (var msg in request.ChatHistory)
            {
                chatHistory.Add(new ChatMessage(
                    msg.Role == "user" ? ChatRole.User : ChatRole.Assistant,
                    msg.Content));
            }
        }

        // Add the current user prompt
        chatHistory.Add(new ChatMessage(ChatRole.User, request.Prompt));

        // Get streaming response from the AI model
        var response = "";
        await foreach (ChatResponseUpdate item in chatClient.GetStreamingResponseAsync(chatHistory))
        {
            response += item.Text;
        }

        // Add AI response to chat history for return
        chatHistory.Add(new ChatMessage(ChatRole.Assistant, response));

        return Results.Ok(new ChatResponse
        {
            Response = response,
            ChatHistory = chatHistory.Select(msg => new ChatHistoryItem
            {
                Role = msg.Role == ChatRole.User ? "user" : "assistant",
                Content = msg.Contents.FirstOrDefault()?.ToString() ?? ""
            }).ToList()
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("Chat")
.WithOpenApi();



app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

// Chat API models
record ChatRequest(string Prompt, List<ChatHistoryItem>? ChatHistory = null);

record ChatResponse
{
    public string Response { get; set; } = "";
    public List<ChatHistoryItem> ChatHistory { get; set; } = new();
}

record ChatHistoryItem
{
    public string Role { get; set; } = "";
    public string Content { get; set; } = "";
}
