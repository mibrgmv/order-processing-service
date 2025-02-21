namespace OrderProcessingService.Infrastructure.Persistence;

public class PersistenceOptions
{
    public string Host { get; set; } = string.Empty;

    public int Port { get; set; }

    public string Database { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string SslMode { get; set; } = string.Empty;

    public string ConnectionString => $"Host={Host};" +
                                      $"Port={Port};" +
                                      $"Database={Database};" +
                                      $"Username={Username};" +
                                      $"Password={Password};" +
                                      $"Ssl Mode={SslMode};";
}