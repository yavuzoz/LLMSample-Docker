using Microsoft.Extensions.AI;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Collections.Generic;

var builder = Host.CreateApplicationBuilder();

// Register Ollama chat client
builder.Services.AddChatClient(
    new OllamaChatClient(new Uri("http://localhost:11434"), "llama3")
);

var app = builder.Build();

var chatClient = app.Services.GetRequiredService<IChatClient>();

List<ChatMessage> chatHistory = new();

// ✅ System role: controls behavior
chatHistory.Add(new ChatMessage(ChatRole.System,
    "You are a helpful assistant. Always answer in Turkish and give scientifically correct answers."));

Console.Clear();
Console.WriteLine("Hackathon ChatBot");

while (true)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.Write("Your question: ");
    Console.ResetColor();

    string? userInput = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(userInput))
        continue;

    chatHistory.Add(new ChatMessage(ChatRole.User, userInput));

    Console.ForegroundColor = ConsoleColor.Blue;
    Console.WriteLine("Answer >>");
    Console.ResetColor();

    // Single response
    ChatResponse response = await chatClient.GetResponseAsync(chatHistory);
    var assistantMessage = response.Messages[^1];
    Console.WriteLine(assistantMessage.Text);
    chatHistory.Add(new ChatMessage(ChatRole.Assistant, assistantMessage.Text));

    Console.WriteLine();
}
