using System.ComponentModel;
using System.Runtime.CompilerServices;
using IISBackend.BL.Models.Interfaces;

namespace IISBackend.BL.Models;

public abstract record ModelBase : IModel
{
    public Guid Id { get; init; }
}
