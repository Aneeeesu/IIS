using IISBackend.BL.Models.Interfaces;
using IISBackend.DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISBackend.BL.Models.Requests;

public class VerificationRequestBaseModel : IModel
{
    public Guid Id { get; init; }
}
