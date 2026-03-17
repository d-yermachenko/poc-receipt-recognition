using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

//var passwordResource = builder.AddParameter("redisPassword", Guid.NewGuid().ToString(), publishValueAsDefault:false, secret: true);

var redis = builder.
    AddRedis("recognitionTasksStorage") ;

var ollama = builder
    .AddExternalService("ollama", "http://192.168.0.132:11434")
    .WithHttpHealthCheck("/api/version", 200);

var receiptRecognitionApi = builder
    .AddProject<Projects.ReceiptRecognition_API>("receiptrecognition-api")
    .WithReference(ollama)
    .WithReference(redis)
    .WaitForStart(ollama)
    .WaitForStart(redis);

/*var ollama = builder.AddOllama("ollama")
   .WithGPUSupport();

ollama.AddModel("");
ollama.AddModel("");*/

/*builder
    .AddProject<Projects.ReceiptRecognition_API>("receiptrecognition-api")
    .WaitForStart(ollama);*/

builder.Build().Run();
