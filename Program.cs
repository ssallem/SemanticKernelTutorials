using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Embeddings;

var builder = Host.CreateApplicationBuilder(args);

//IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
//#pragma warning disable SKEXP0070 // 형식은 평가 목적으로 제공되며, 이후 업데이트에서 변경되거나 제거될 수 있습니다. 계속하려면 이 진단을 표시하지 않습니다.
//kernelBuilder.AddOllamaChatCompletion(
//    modelId: "deepseek-r1:1.5b",
//    endpoint: new Uri("http://localhost:11434"),
//    serviceId: "TEST_ID"
//    );
//#pragma warning restore SKEXP0070 // 형식은 평가 목적으로 제공되며, 이후 업데이트에서 변경되거나 제거될 수 있습니다. 계속하려면 이 진단을 표시하지 않습니다.

#pragma warning disable SKEXP0070 // 형식은 평가 목적으로 제공되며, 이후 업데이트에서 변경되거나 제거될 수 있습니다. 계속하려면 이 진단을 표시하지 않습니다.
builder.Services.AddOllamaTextEmbeddingGeneration(
    modelId: "mxbai-embed-large",
    endpoint: new Uri("http://localhost:11434"),
    serviceId: "TEST_ID"
);
#pragma warning restore SKEXP0070 // 형식은 평가 목적으로 제공되며, 이후 업데이트에서 변경되거나 제거될 수 있습니다. 계속하려면 이 진단을 표시하지 않습니다.

builder.Services.AddTransient((serviceProvider) =>
{
    return new Kernel(serviceProvider);
});

#pragma warning disable SKEXP0001 // 형식은 평가 목적으로 제공되며, 이후 업데이트에서 변경되거나 제거될 수 있습니다. 계속하려면 이 진단을 표시하지 않습니다.
ITextEmbeddingGenerationService textEmbeddingGenerationService = builder.Services.BuildServiceProvider().GetRequiredService<x>();
#pragma warning restore SKEXP0001 // 형식은 평가 목적으로 제공되며, 이후 업데이트에서 변경되거나 제거될 수 있습니다. 계속하려면 이 진단을 표시하지 않습니다.
IList<ReadOnlyMemory<float>> embeddings =
    await textEmbeddingGenerationService.GenerateEmbeddingsAsync(
    [
        "sample text 1",
        "sample text 2"
    ]);

ReadOnlyMemory<float> embedding =
    await textEmbeddingGenerationService.GenerateEmbeddingAsync("sample text");

Console.ReadLine();
