using System.ComponentModel.DataAnnotations;
using LinkGoblin.Data.Base;
using Microsoft.EntityFrameworkCore;
using static RT.Comb.Provider;

namespace LinkGoblin.Data.Entities;

[Index(nameof(Name), IsUnique = true)]
public class Tag : AuditableBase
{
    [Key]
    public Guid Key { get; set; } = PostgreSql.Create();
    public required string Name { get; set; }
    [StringLength(1024), MaxLength(1024)]
    public string? Description { get; set; }
    
    public virtual IEnumerable<Collection>? Collections { get; set; }
    public virtual IEnumerable<Bookmark>? Bookmarks { get; set; }
}