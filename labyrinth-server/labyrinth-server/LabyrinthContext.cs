using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;

namespace labyrinth_server
{
    public class LabyrinthContext : DbContext
    {
        public DbSet<ScoreBoard> ScoreBoard { get; set; }

        //https://stackoverflow.com/questions/38338475/no-database-provider-has-been-configured-for-this-dbcontext-on-signinmanager-p
        public LabyrinthContext(DbContextOptions<LabyrinthContext> options) : base(options)
        {
            
        }
    }
}
