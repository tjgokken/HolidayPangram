var builder = DistributedApplication.CreateBuilder(args);

var ollama = builder.AddOllama("ollama")
    .WithDataVolume();

var llama = ollama.AddModel("llama", "llama3");

builder.AddProject<Projects.HolidayPangram_Web>("pangram-web")
    .WithReference(llama)
    .WaitFor(llama);

await builder.Build().RunAsync();