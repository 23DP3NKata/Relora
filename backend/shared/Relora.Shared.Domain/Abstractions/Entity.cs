using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relora.Shared.Domain.Abstractions;

/// <summary>
/// Represents the entity class.
/// </summary>
public class Entity<TId>
{
    /// <summary>
    /// Gets or sets the id used by this type.
    /// </summary>
    public TId Id { get; protected set; } = default!;

    /// <summary>
    /// Initializes a new instance of the <see cref="Entity"/> class.
    /// </summary>
    protected Entity() { }
    /// <summary>
    /// Initializes a new instance of the <see cref="Entity"/> class.
    /// </summary>
    /// <param name="id">Entity identifier.</param>
    public Entity(TId id)
    {
        if (id == null || id.Equals(default))
        {
            throw new ArgumentException("Entity ID cannot be an empty GUID.", nameof(id));
        }

        Id = id;
    }

    /// <summary>
    /// Performs the equals operation.
    /// </summary>
    /// <param name="obj">Obj.</param>
    /// <returns>True if the operation succeeds; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Id.Equals(other.Id);
    }

    /// <summary>
    /// Gets hash code.
    /// </summary>
    /// <returns>The operation result.</returns>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
