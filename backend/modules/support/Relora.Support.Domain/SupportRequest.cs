namespace Relora.Support.Domain;

public sealed class SupportRequest
{
    public SupportRequest(Guid id, string email, string category, string subject, string message, DateTime createdAt)
    {
        Id = id;
        Email = email;
        Category = category;
        Subject = subject;
        Message = message;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string Category { get; private set; }
    public string Subject { get; private set; }
    public string Message { get; private set; }
    public DateTime CreatedAt { get; private set; }
}
