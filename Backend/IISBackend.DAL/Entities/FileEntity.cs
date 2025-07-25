﻿using AutoMapper;
using IISBackend.DAL.Entities.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace IISBackend.DAL.Entities;

public partial class FileEntity : IEntity,IUserAuthorized
{
    public Guid Id { get; set; }
    [Index(IsUnique = true)]
    public required string Url { get; set; }

    public required string Key { get; set; }

    public Guid? OwnerId { get; set; }
    [ForeignKey(nameof(OwnerId))]
    public UserEntity? Owner { get; set; }

    public DateTime UploadDate { get; set; } = DateTime.Now;
    public required string FileType { get; set; }

    public Guid GetOwnerID() => OwnerId ?? Guid.Empty;


    public class FileEntityMapperProfile : Profile
    {
        public FileEntityMapperProfile()
        {
            CreateMap<FileEntity, FileEntity>();
        }
    }
}