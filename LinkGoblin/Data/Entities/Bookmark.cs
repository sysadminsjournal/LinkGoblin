using System.ComponentModel.DataAnnotations;
using LinkGoblin.Data.Base;

namespace LinkGoblin.Data.Entities;

using static RT.Comb.Provider;

public class Bookmark : AuditableBase
{
    [Key]
    public Guid Key { get; set; } = PostgreSql.Create();
    
    public virtual required Collection Collection { get; set; }
    
    /*
     * Max length set to 2048 as sitemaps and search engines break at anything higher
     * While in theory URLs can be longer, it's extremely unlikely.
     * If for some reason it's needed, update these values to a new sane setting.
     */
    [MaxLength(2048), StringLength(2048)]
    public required string Url { get; set; }
    
    public virtual IEnumerable<Tag>? Tags { get; set; }
}