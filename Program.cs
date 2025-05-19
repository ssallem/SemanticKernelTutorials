using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SK_Tutorials;
using SK_Tutorials.Interfaces;
using System.ComponentModel;
using System.Text.Json;
using Microsoft.SemanticKernel.Connectors.Ollama;

var builder = Host.CreateApplicationBuilder(args);

// 1. DI 등록
builder.Services.AddTransient<IPizzaService, DummyPizzaService>();
builder.Services.AddTransient<IUserContext, DummyUserContext>();
builder.Services.AddTransient<IPaymentService, DummyPaymentService>();

// 2. 호스트 빌드 및 서비스 제공자 가져오기
var host = builder.Build();
var serviceProvider = host.Services;

// 3. Kernel 설정 (Ollama 사용) - 서비스 제공자 연결
IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.Services.AddSingleton(serviceProvider); // 서비스 제공자를 Kernel에 연결

//#pragma warning disable SKEXP0070 // 형식은 평가 목적으로 제공되며, 이후 업데이트에서 변경되거나 제거될 수 있습니다. 계속하려면 이 진단을 표시하지 않습니다.
//kernelBuilder.AddOllamaChatCompletion(
//    modelId: "llama3.3",
//    endpoint: new Uri("http://localhost:11434"),
//    serviceId: "OLLAMA"
//);
//#pragma warning restore SKEXP0070 // 형식은 평가 목적으로 제공되며, 이후 업데이트에서 변경되거나 제거될 수 있습니다. 계속하려면 이 진단을 표시하지 않습니다.

kernelBuilder.WithOllamaService(timeout: TimeSpan.FromMinutes(10));


// 4. 플러그인 직접 인스턴스화
var pizzaService = serviceProvider.GetRequiredService<IPizzaService>();
var userContext = serviceProvider.GetRequiredService<IUserContext>();
var paymentService = serviceProvider.GetRequiredService<IPaymentService>();
var plugin = new OrderPizzaPlugin(pizzaService, userContext, paymentService);

// kernelBuilder.Plugins.AddFromType<OrderPizzaPlugin>("OrderPizza");
// 5. 커널 빌드
kernelBuilder.Plugins.AddFromObject(plugin, "OrderPizza");

Kernel kernel = kernelBuilder.Build();
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// 5. 초기 대화 셋팅
var history = new ChatHistory();

history.AddSystemMessage(@"You are a pizza ordering assistant. 
When a user wants to order a pizza, ALWAYS use the OrderPizza.AddToCart function to add it to their cart.
Available pizza sizes: small, medium, large
Available toppings: Cheese, Pepperoni, Mushrooms
After adding to cart, offer to checkout or recommend more items.");

history.AddUserMessage("I'd like to order a medium pizza with cheese and pepperoni please");

// 7. 자동 함수 호출 설정 (Ollama에 맞게 설정)
var executionSettings = new OpenAIPromptExecutionSettings
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
    Temperature = 0.7,
    MaxTokens = 2000,
    TopP = 0.95
};

// 6. 자동 함수 호출 생성
//OpenAIPromptExecutionSettings executionSettings = new()
//{
//    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
//};

try
{
    Console.WriteLine("\n▶ Assistant Response:");
    var result = await chatCompletionService.GetChatMessageContentAsync(
            chatHistory: history,
            executionSettings: executionSettings,            
            kernel: kernel);

    // 9. 응답 및 잠재적 함수 호출 결과 출력
    Console.WriteLine("\n▶ 응답:");
    Console.WriteLine(result);

    // 10. 대화 히스토리에 응답 추가
    history.AddAssistantMessage(result.ToString());

    Console.WriteLine("\n=== Chat History ===");
    foreach (var msg in history)
    {
        Console.WriteLine($"[{msg.Role}]: {msg.Content}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"오류 발생: {ex.Message}");
    Console.WriteLine($"스택 추적: {ex.StackTrace}");

    if (ex.InnerException != null)
    {
        Console.WriteLine($"내부 예외: {ex.InnerException.Message}");
    }
}

// 11. 함수 호출 수동 확인
Console.WriteLine("\n\n=== 함수 호출 수동 확인 ===");
try
{
    // 플러그인에서 함수 직접 실행해보기
    var addToCartFunction = kernel.Plugins["OrderPizza"]["add_pizza_to_cart"];
    if (addToCartFunction != null)
    {
        Console.WriteLine("AddToCart 함수 직접 실행 시도...");
        var arguments = new KernelArguments
        {
            ["size"] = "medium",
            ["toppings"] = "Pepperoni" // 열거형 값과 정확히 일치하도록 수정
        };

        var functionResult = await kernel.InvokeAsync(addToCartFunction, arguments);
        Console.WriteLine($"함수 호출 결과: {functionResult}");
    }
    else
    {
        Console.WriteLine("AddToCart 함수를 찾을 수 없습니다.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"함수 직접 호출 오류: {ex.Message}");
}

Console.WriteLine("\n\n=== Chat History ===");
foreach (var msg in history)
{
#pragma warning disable SKEXP0001 // 형식은 평가 목적으로 제공되며, 이후 업데이트에서 변경되거나 제거될 수 있습니다. 계속하려면 이 진단을 표시하지 않습니다.
    Console.WriteLine($"[{msg.Role}] {(msg.AuthorName ?? "")}: {msg.Content}");
#pragma warning restore SKEXP0001 // 형식은 평가 목적으로 제공되며, 이후 업데이트에서 변경되거나 제거될 수 있습니다. 계속하려면 이 진단을 표시하지 않습니다.
}

Console.ReadLine();

#region chatHistory example
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
#endregion