using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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