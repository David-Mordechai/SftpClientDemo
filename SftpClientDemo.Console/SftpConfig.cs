namespace SftpClientDemo.Console;

public class SftpConfig
{
    public string Host { get; set; } = default!;
    public int Port { get; set; }
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
}