using Asp_Labb3.Models;
using Microsoft.EntityFrameworkCore;

namespace Asp_Labb3.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}
		public DbSet<User> Users { get; set; }
		public DbSet<Interest> Interests { get; set; }
		public DbSet<UserInterest> UserInterests { get; set; }
		public DbSet<WebLink> WebLinks { get; set; }
	}
}
