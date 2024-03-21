using System.ComponentModel.DataAnnotations;
using LinkGoblin.Data.Base;
using static RT.Comb.Provider;

namespace LinkGoblin.Data.Entities;

public class Collection : AuditableBase
{
    [Key]
    public Guid Key { get; set; } = PostgreSql.Create();
    [MaxLength(256), StringLength(256)]
    public required string Name { get; set; }
    [MaxLength(1024), StringLength(1024)]
    public string? Description { get; set; }
    
    /*
     * Max length set to 2796204 which is 2MB encoded to base64 using the formula
     * ciel((2 * 1024 * 1024) / 3) * 4
     */
    [MaxLength(2796204), StringLength(2796204)]
    public string? CoverImageBase64 { get; set; }
    
    public virtual IEnumerable<Bookmark>? Bookmarks { get; set; }
    public virtual IEnumerable<Tag>? Tags { get; set; }
}