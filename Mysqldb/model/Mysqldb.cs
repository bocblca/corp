using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using rcbblc.netauto;
using System.Reflection;

[assembly: AssemblyKeyFileAttribute("rcbautokey.keys")]
namespace Mysqldb
{
    public class Wxusers : IdentityDbContext<User, myRole, long>
    {
        //private static ILoggerFactory loggerFactory=LoggerFactory.Create(b=>b.AddConsole());
        private IConfiguration configuration;
        public Wxusers(IConfiguration configuration) {
        
          this.configuration = configuration;
        
        }

        public Wxusers(DbContextOptions<Wxusers> options) : base(options){ }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
           //var serverVersion = new MySqlServerVersion(new Version(8, 0, 27));
           // Console.WriteLine(configuration.GetConnectionString("mysql"));
         
            //optionsBuilder.UseMySql(configuration.GetConnectionString("mysql"), serverVersion);
           
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("postpresql"));
           
           // optionsBuilder.UseBatchEF_MySQLPomelo();
           
            //optionsBuilder.UseLoggerFactory(loggerFactory);

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }

        public DbSet<asset_state> asset_States { get; set; }
        public DbSet<Asset> assets { get; set; }
        public DbSet<Gps> gps { get; set; }
        public DbSet<Member> members { get; set; }
        public DbSet<Workdepart> wrokdeparts { get; set; }
        public DbSet<hangjob> hangjobs { get; set; }
        public DbSet<Loans> Loans { get; set; }
        public DbSet<Contract> contracts { get; set; }
        public DbSet<Loanuser> loanusers { get; set; }
        public DbSet<LoanProcess> loanProcesses { get; set; }
        public DbSet<Wxsession> wxsessions { get; set; }
        public DbSet<Booking> bookings { get; set; }
        public DbSet<Rcbstraff> rcbstraffs { get; set; }
        public DbSet<Account> accounts { get; set; }
        public DbSet<Cloudprinter> cloudprinters { get; set; }
        public DbSet<Printers> printers { get; set; }
        public DbSet<Straff> straffs { get; set; }

        public DbSet<employee> employees { get; set; }
        public DbSet<wxunionid> wxunionids { get; set; }
        public DbSet<Order_TBL> order_TBLs { get; set; }
        public DbSet<Trans_TBL> trans_TBLs { get; set; }
        public DbSet<Bank> banks { get; set; }
        public DbSet<Express> express { get; set; }
        public DbSet<gold_TBL> gold_TBLs { get; set; }
        public DbSet<bankcoord> bankcoords { get; set; }
        public DbSet<customers> customers { get; set; }
        public DbSet<Credittoken> credittokens { get; set; }

        public DbSet<Subankdata> subankdatas { get; set; }
        public DbSet<Aitoken> aitokens { get; set; }
    }
    class AutoInitcs : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddScoped<Wxusers>();
            
        }
    }

}