using Microsoft.EntityFrameworkCore;


namespace mysqlapi.Models
{
    public class StudentDetailContext: DbContext
    {
        public StudentDetailContext(DbContextOptions<StudentDetailContext> options) : base(options)
        {

        }

        public  DbSet<StudentDetail> StudentDetails { get; set; }

        public  DbSet<User> Users { get; set; }
    }
}
