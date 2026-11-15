namespace Relora.Support.Domain;

public sealed class SupportRequest
{
    public SupportRequest(Guid id, Guid? userId, string email, string category, string subject, string message, DateTime createdAt)
    {
        Id = id;
        UserId = userId;
        Email = email;
        Category = category;
        Subject = subject;
        Message = message;
        Status = SupportRequestStatus.New;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    // viesim te ir null
    public Guid? UserId { get; private set; }
    public string Email { get; private set; }
    public string Category { get; private set; }
    public string Subject { get; private set; }
    public string Message { get; private set; }
    public SupportRequestStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public void ChangeStatus(SupportRequestStatus status)
    {
        Status = status;
    }
}
