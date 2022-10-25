using Microsoft.Extensions.Logging;
using Renci.SshNet;

namespace SftpClientDemo.Console;

public class SftpService
{
    private readonly ILogger<SftpService> _logger;
    private readonly SftpConfig _config;

    public SftpService(ILogger<SftpService> logger, SftpConfig sftpConfig)
    {
        _logger = logger;
        _config = sftpConfig;
    }

    public IEnumerable<string> ListAllFiles(string remoteDirectory = ".")
    {
        using var client = new SftpClient(_config.Host, _config.Port == 0 ? 22 : _config.Port, _config.UserName, _config.Password);
        try
        {
            client.Connect();
            var result = client.ListDirectory(remoteDirectory)
                .Where(x => x.IsDirectory is false)
                .Select(x => x.FullName);
            return result;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"Failed in listing files under [{remoteDirectory}]");
        }
        finally
        {
            client.Disconnect();
        }

        return new List<string>();
    }

    public void UploadFile(string localFilePath, string remoteFilePath)
    {
        using var client = new SftpClient(_config.Host, _config.Port == 0 ? 22 : _config.Port, _config.UserName, _config.Password);
        try
        {
            client.Connect();
            using var s = File.OpenRead(localFilePath);
            client.UploadFile(s, remoteFilePath);
            _logger.LogInformation($"Finished uploading file [{localFilePath}] to [{remoteFilePath}]");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"Failed in uploading file [{localFilePath}] to [{remoteFilePath}]");
        }
        finally
        {
            client.Disconnect();
        }
    }

    public string GetFileContent(string remoteFilePath)
    {
        var content = string.Empty;
        
        using var client = new SftpClient(_config.Host, _config.Port == 0 ? 22 : _config.Port, _config.UserName, _config.Password);
        try
        {
            client.Connect();
            if (client.Exists(remoteFilePath) is false) return content;
            
            Stream fileStream = new MemoryStream();
            client.OpenRead(remoteFilePath).CopyTo(fileStream);


            if (fileStream.Length > 0)
            {
                fileStream.Position = 0;
                using var sr = new StreamReader(fileStream);
                content = sr.ReadToEnd();
            }

            _logger.LogInformation("Finished downloading file as stream from [{remoteFilePath}]", remoteFilePath);
            return content;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"Failed in downloading file as stream from [{remoteFilePath}]", remoteFilePath);
        }
        finally
        {
            client.Disconnect();
        }

        return content;
    }

    public void DeleteFile(string remoteFilePath)
    {
        using var client = new SftpClient(_config.Host, _config.Port == 0 ? 22 : _config.Port, _config.UserName, _config.Password);
        try
        {
            client.Connect();
            client.DeleteFile(remoteFilePath);
            _logger.LogInformation($"File [{remoteFilePath}] deleted.");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"Failed in deleting file [{remoteFilePath}]");
        }
        finally
        {
            client.Disconnect();
        }
    }
}