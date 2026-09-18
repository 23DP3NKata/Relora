namespace Relora.Identity.Domain;

/// <summary>
/// Represents the user class.
/// </summary>
public sealed class User
{
    /// <summary>
    /// Gets or sets the id used by this type.
    /// </summary>
    public Guid Id { get; private set; }
    /// <summary>
    /// Gets or sets the name used by this type.
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// Gets or sets the email used by this type.
    /// </summary>
    public string Email { get; private set; }
    /// <summary>
    /// Gets or sets the password hash used by this type.
    /// </summary>
    public string PasswordHash { get; private set; }
    /// <summary>
    /// Gets or sets the user name used by this type.
    /// </summary>
    public string UserName { get; private set; }
    /// <summary>
    /// Gets or sets the is admin used by this type.
    /// </summary>
    public bool IsAdmin { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="User"/> class.
    /// </summary>
    private User() { } // EF

    private User(Guid id,
        string name,
        string email,
        string passwordHash,
        string userName,
        bool isAdmin)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("Password hash cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(userName))
        {
            throw new ArgumentException("Username cannot be empty");
        }

        Id = id;
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        UserName = userName;
        IsAdmin = isAdmin;
    }

    public void UpdateProfile(string name, string userName)
    {
        Name = name;
        UserName = userName;
    }

    public void ChangePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
    }

    public static User Create(
        string name,
        string email,
        string passwordHash,
        string userName)
    {
        return new User(
            Guid.NewGuid(),
            name,
            email,
            passwordHash,
            userName,
            false);
    }
}
