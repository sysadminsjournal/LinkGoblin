using System.ComponentModel.DataAnnotations;
using LinkGoblin.Data.Base;
using static RT.Comb.Provider;

namespace LinkGoblin.Data.Entities;

public class Collection : AuditableBase
{
    [Key]
    public Guid Key { get; set; } = PostgreSql.Create();
}