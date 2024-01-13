using Microsoft.EntityFrameworkCore;

namespace api.Data;

public class ApplicationContext : DbContext
{

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
}
