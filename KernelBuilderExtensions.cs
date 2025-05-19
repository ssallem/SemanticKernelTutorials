
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OllamaSharp;
using System;
using System.Net.Http;

namespace SK_Tutorials
{
    public static class KernelBuilderExtensions
    {
        /// <summary>
        /// Ollama 서비스를 커스텀 Timeout으로 등록합니다.
        /// </summary>
        public static IKernelBuilder WithOllamaService(
            this IKernelBuilder builder,
            string baseUrl = "http://localhost:11434",
            TimeSpan? timeout = null)
        {
            // 기본값으로 5분 사용
            timeout ??= TimeSpan.FromMinutes(5);

            // 1. 커스텀 HttpClient 생성 및 Base Address 설정
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:11434"); // Base Address 반드시 설정 필요
            httpClient.Timeout = TimeSpan.FromMinutes(10); // Timeout을 10분으로 설정


            // 여기서 HttpClient를 매개변수로 전달
            var ollamaApiClient = new OllamaApiClient(httpClient, defaultModel: "llama3.3");

#pragma warning disable SKEXP0001 // 형식은 평가 목적으로 제공되며, 이후 업데이트에서 변경되거나 제거될 수 있습니다. 계속하려면 이 진단을 표시하지 않습니다.
            builder.Services.AddSingleton<IChatCompletionService>(sp =>
                ollamaApiClient.AsChatCompletionService());
#pragma warning restore SKEXP0001 // 형식은 평가 목적으로 제공되며, 이후 업데이트에서 변경되거나 제거될 수 있습니다. 계속하려면 이 진단을 표시하지 않습니다.

            return builder;
        }
    }
}
