using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DarkStoreCommonLib.Db.Entities;

public class BaseEntity
{
    [Key]
    public Guid Id { get; set; }

    [Column(TypeName = "timestamp(0) without time zone")]
    public DateTime CreatedAt { get; set; }

    [Column(TypeName = "timestamp(0) without time zone")]
    public DateTime UpdatedAt { get; set; }
}