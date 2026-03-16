using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var ollama = builder
    .AddExternalService("ollama", "http://192.168.0.132:11434");

/*var ollama = builder.AddOllama("ollama")
    .WithGPUSupport();

ollama.AddModel("");
ollama.AddModel("");*/

/*builder
    .AddProject<Projects.ReceiptRecognition_API>("receiptrecognition-api")
    .WaitForStart(ollama);*/

builder.Build().Run();
