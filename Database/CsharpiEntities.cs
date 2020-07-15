using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace csharpi.Database
{
    public partial class CsharpiEntities : DbContext
    {
        public virtual DbSet<EightBallAnswer> EightBallAnswer { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = "csharpi.db" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);
            optionsBuilder.UseSqlite(connection);
        }        
    }
}