using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace IISBackend.BL.Models;

public abstract record ModelBase : IModel
{
    public Guid Id { get; init; }
}
