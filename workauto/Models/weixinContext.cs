using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mysqldb;


namespace workapi


{


    public class Wxusers: IdentityDbContext<User, myRole, long>
    {

        //private static ILoggerFactory loggerFactory=LoggerFactory.Create(b=>b.AddConsole());


        public Wxusers()

        {
            
        }
       
        public Wxusers(DbContextOptions<Wxusers> options) : base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
        { 
            base.OnConfiguring(optionsBuilder);
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 27));
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                optionsBuilder.UseMySql("Server=192.168.100.223;port=3306;database=wxapp;uid=radondb;pwd=Blc_050303;character set=utf8;AllowLoadLocalInfile=true", serverVersion);
            }else
           
            {

                optionsBuilder.UseMySql("Server=192.168.100.223;port=3306;database=wxapp;uid=radondb;pwd=Blc_050303;character set=utf8;AllowLoadLocalInfile=true;allowPublicKeyRetrieval=true", serverVersion);
            }
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "RCB") {
                optionsBuilder.UseMySql("Server=mysql;port=3306;database=wxapp;uid=radondb;pwd=Blc_050303;character set=utf8;AllowLoadLocalInfile=true", serverVersion);
            }
                //if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "RCB")
                //{
                //    optionsBuilder.UseMySql("server=mysql;port=3306;database=wxapp;uid=radondb;pwd=Blc_050303;character set=utf8;AllowLoadLocalInfile=true", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0-mysql"));
                //}
                optionsBuilder.UseBatchEF_MySQLPomelo();
            //optionsBuilder.UseLoggerFactory(loggerFactory);

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
         base.OnModelCreating (modelBuilder);
           
           
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly); 
        }

        public DbSet<asset_state> asset_States { get; set; }
        public DbSet<Asset> assets { get; set; }
        public DbSet<corp.Gps> gps { get; set; }
        public DbSet<Member> members { get; set; }
        public DbSet<Workdepart> wrokdeparts { get; set; }
        public DbSet<hangjob> hangjobs { get; set; }
        public DbSet<Loans> Loans { get; set; }
        public DbSet<Contract> contracts { get; set; }
        public DbSet<Loanuser> loanusers { get; set; }
        public DbSet<LoanProcess>loanProcesses { get; set; }
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
        public DbSet<Trans_TBL> trans_TBLs { get; set;}
        public DbSet<Bank> banks { get; set; }
        public DbSet <Express> express { get; set; }
        public DbSet <gold_TBL> gold_TBLs { get; set;}
        public DbSet <bankcoord>bankcoords { get; set; }
        public DbSet <customers>customers { get; set; }
        public DbSet <Credittoken>credittokens { get; set; }

        public DbSet <Subankdata> subankdatas { get; set; }
        public DbSet <Aitoken> aitokens { get; set; }
    }
}
