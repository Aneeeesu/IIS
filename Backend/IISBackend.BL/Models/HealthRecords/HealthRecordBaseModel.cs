using IISBackend.BL.Models.Interfaces;
using IISBackend.Common.Enums;
using IISBackend.DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISBackend.BL.Models.HealthRecords;

public class HealthRecordBaseModel : IModel
{
    public Guid Id { get; init; }
    public required DateTime Time { get; set; }
    public required string Content { get; set; }
    public required HealthRecordType Type { get; set; }
}
