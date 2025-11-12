using Microsoft.AspNetCore.Mvc;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// 서비스 등록
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Azure OpenAI 클라이언트 등록
builder.Services.AddSingleton<AzureOpenAIClient>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var endpoint = configuration["AzureAI:ProjectEndpoint"];
    var apiKey = configuration["AzureAI:ApiKey"];
    
    if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
    {
        throw new InvalidOperationException("Azure AI credentials not configured. Please set AzureAI:ProjectEndpoint and AzureAI:ApiKey in appsettings.json");
    }
    
    return new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apiKey));
});

// CORS 설정 - 프론트엔드 연결용
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5001", "https://localhost:5001")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// HTTP 요청 파이프라인 설정
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

// Health check 엔드포인트
app.MapGet("/health", () => Results.Ok(new 
{ 
    status = "healthy", 
    timestamp = DateTime.UtcNow,
    service = "FabricAgent.Backend"
}))
   .WithName("HealthCheck")
   .WithOpenApi();

// 채팅 엔드포인트 - Azure OpenAI 스트리밍
app.MapPost("/api/agent/chat", async (
    HttpContext context, 
    [FromBody] ChatRequest request, 
    AzureOpenAIClient client, 
    IConfiguration config,
    ILogger<Program> logger) =>
{
    if (string.IsNullOrWhiteSpace(request.Message))
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsJsonAsync(new { error = "Message cannot be empty" });
        return;
    }

    try
    {
        var deploymentName = config["AzureAI:DeploymentName"] ?? "gpt-4o";
        logger.LogInformation("Processing chat request with deployment: {DeploymentName}", deploymentName);
        
        var chatClient = client.GetChatClient(deploymentName);

        var chatMessages = new List<ChatMessage>
        {
            new SystemChatMessage("You are a helpful AI assistant for Microsoft Fabric compliance reporting. You analyze data and provide insights in clear, professional Markdown format."),
            new UserChatMessage(request.Message)
        };

        context.Response.ContentType = "text/event-stream";
        context.Response.Headers.Append("Cache-Control", "no-cache");
        context.Response.Headers.Append("X-Accel-Buffering", "no");

        await foreach (var update in chatClient.CompleteChatStreamingAsync(chatMessages))
        {
            foreach (var contentPart in update.ContentUpdate)
            {
                if (!string.IsNullOrEmpty(contentPart.Text))
                {
                    var escapedText = contentPart.Text.Replace("\n", "\\n").Replace("\r", "");
                    await context.Response.WriteAsync($"data: {escapedText}\n\n");
                    await context.Response.Body.FlushAsync();
                }
            }
        }
        
        await context.Response.WriteAsync("data: [DONE]\n\n");
        await context.Response.Body.FlushAsync();
        
        logger.LogInformation("Chat request completed successfully");
    }
    catch (ClientResultException ex)
    {
        logger.LogError(ex, "Azure OpenAI request failed");
        context.Response.StatusCode = 502;
        await context.Response.WriteAsync($"data: [ERROR] Azure AI service error: {ex.Message}\n\n");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Unexpected error in chat endpoint");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync($"data: [ERROR] {ex.Message}\n\n");
    }
})
.WithName("ChatWithAgent")
.WithOpenApi();

app.Run();

// 요청/응답 모델
record ChatRequest(string Message);
record ChatResponse(string Reply);
