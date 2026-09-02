using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relora.Items.Application.Models;

public sealed class LotSellerDto
{
    /// <summary>
    /// Gets or sets the id used by this type.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name used by this type.
    /// </summary>
    public string Name { get; set; }
    public string Username { get; set; }
}
