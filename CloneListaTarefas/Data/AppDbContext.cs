using CloneListaTarefas.Model;
using Microsoft.EntityFrameworkCore;

namespace CloneListaTarefas.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Tarefa> lista { get; set; }
    }
}
