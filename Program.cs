using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

var builder = Host.CreateApplicationBuilder(args);


// 1. Kernel 설정 (Ollama 사용)
IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
#pragma warning disable SKEXP0070 // 형식은 평가 목적으로 제공되며, 이후 업데이트에서 변경되거나 제거될 수 있습니다. 계속하려면 이 진단을 표시하지 않습니다.
kernelBuilder.AddOllamaChatCompletion(
    modelId: "deepseek-r1:14b",
    endpoint: new Uri("http://localhost:11434"),
    serviceId: "OLLAMA"
);
#pragma warning restore SKEXP0070 // 형식은 평가 목적으로 제공되며, 이후 업데이트에서 변경되거나 제거될 수 있습니다. 계속하려면 이 진단을 표시하지 않습니다.
// 2. DI 등록
builder.Services.AddTransient((serviceProvider) => new Kernel(serviceProvider));
var host = builder.Build();
Kernel kernel = kernelBuilder.Build();

// 3. ChatCompletion 서비스 가져오기
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();


// 4. ChatHistory 생성 및 초기 메시지 추가
var history = new ChatHistory();
history.AddSystemMessage("You are a helpful IT assistant.");
history.AddUserMessage("Please tell me how to check which port is being used in Windows.");


// 5. 사용자 식별 포함한 메시지 추가 (AuthorName 사용 예시)
#pragma warning disable SKEXP0001 // 형식은 평가 목적으로 제공되며, 이후 업데이트에서 변경되거나 제거될 수 있습니다. 계속하려면 이 진단을 표시하지 않습니다.
history.Add(new ChatMessageContent
{
    Role = AuthorRole.User,
    AuthorName = "James",
    Content = "Is there a way to filter by process name?"
});
#pragma warning restore SKEXP0001 // 형식은 평가 목적으로 제공되며, 이후 업데이트에서 변경되거나 제거될 수 있습니다. 계속하려면 이 진단을 표시하지 않습니다.

// 6. 함수 호출 시뮬레이션 (예: get_user_preferences)
history.Add(new ChatMessageContent
{
    Role = AuthorRole.Assistant,
    Items = new ChatMessageContentItemCollection
    {
        new FunctionCallContent(
            functionName: "get_user_preferences",
            pluginName: "UserProfile",
            id: "pref001",
            arguments: new () { { "username", "James" } }
        )
    }
});

// 7. 함수 호출 결과 시뮬레이션 (Tool 역할)
history.Add(new ChatMessageContent
{
    Role = AuthorRole.Tool,
    Items = new ChatMessageContentItemCollection
    {
        new FunctionResultContent(
            functionName: "get_user_preferences",
            pluginName: "UserProfile",
            callId: "pref001",
            result: "{ \"preferred_os\": \"Windows 11\" }"
        )
    }
});

// 8. 감축 전략 적용 (마지막 5개 메시지만 유지)
var reducer = new ChatHistoryTruncationReducer(targetCount: 5);
var reduced = await reducer.ReduceAsync(history);
if (reduced is not null)
{
    history = new ChatHistory(reduced);
}

// 9. 스트리밍 방식으로 응답 받기
Console.WriteLine("\n▶ Assistant Response:");
await foreach (var chunk in chatCompletionService.GetStreamingChatMessageContentsAsync(
    chatHistory: history,
    kernel: kernel))
{
    Console.Write(chunk);
}

Console.WriteLine("\n\n=== Chat History ===");
foreach (var msg in history)
{
#pragma warning disable SKEXP0001 // 형식은 평가 목적으로 제공되며, 이후 업데이트에서 변경되거나 제거될 수 있습니다. 계속하려면 이 진단을 표시하지 않습니다.
    Console.WriteLine($"[{msg.Role}] {(msg.AuthorName ?? "")}: {msg.Content}");
#pragma warning restore SKEXP0001 // 형식은 평가 목적으로 제공되며, 이후 업데이트에서 변경되거나 제거될 수 있습니다. 계속하려면 이 진단을 표시하지 않습니다.
}

Console.ReadLine();















// 1. First, create a new instance of the ChatHistory class
ChatHistory chatHistory = [];

//chatHistory.AddSystemMessage("You are a helpful assistant.");
//chatHistory.AddUserMessage("What's available to order?");
//chatHistory.AddAssistantMessage("We have Pizza, pasta, and salad available to order. What would you like to order?");
//chatHistory.AddUserMessage("I'd like to have the first option, please.");

// 2. Adding richer messages to a chat history

chatHistory.Add(
    new()
    {
        Role = AuthorRole.System,
        Content = "You are a helpful assistant.",
    }
);

#pragma warning disable SKEXP0001 // 형식은 평가 목적으로 제공되며, 이후 업데이트에서 변경되거나 제거될 수 있습니다. 계속하려면 이 진단을 표시하지 않습니다.
// Add user message with an image
chatHistory.Add(
    new()
    {
        Role = AuthorRole.User,
        AuthorName = "Laimonis Dumins",
        Items = [
            new TextContent { Text = "What's available to order?" },
            new ImageContent { Uri = new Uri("https://raw.githubusercontent.com/ssallem/SemanticKernelTutorials/refs/heads/main/Assets/CoffeeMenu.png") },
        ]
    }
);

// Add assistant message
chatHistory.Add(
    new() {
        Role = AuthorRole.Assistant,
        AuthorName = "Cafe Assistant",
        Content = "We have Americano, Latte, Ade and Tea available to order. What would you like to order?",
    }
);

// Add additional message from a different user
chatHistory.Add(
    new() {
        Role = AuthorRole.User,
        AuthorName = "Ema Vargova",
        Content = "I'd like to have the first option, please.",
    }
);

#pragma warning restore SKEXP0001 // 형식은 평가 목적으로 제공되며, 이후 업데이트에서 변경되거나 제거될 수 있습니다. 계속하려면 이 진단을 표시하지 않습니다.