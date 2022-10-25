using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SftpClientDemo.Console;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
        services.AddSingleton(new SftpConfig
            {
                Host = "localhost",
                Port = 2222,
                UserName = "foo",
                Password = "pass"
            })
            .AddScoped<SftpService>())
    .Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();
var client = host.Services.GetRequiredService<SftpService>();

var files = client.ListAllFiles("upload");

foreach (var file in files)
{
    logger.LogInformation(file);
    var fileContent = client.GetFileContent(file);
    logger.LogInformation(fileContent);
}