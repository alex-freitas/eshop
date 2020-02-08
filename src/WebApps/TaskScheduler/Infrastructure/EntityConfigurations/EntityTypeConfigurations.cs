using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskScheduler.Entities;

namespace TaskScheduler.Infrastructure.EntityConfigurations
{
    public class DatabaseInfoConfiguration : IEntityTypeConfiguration<DatabaseInfo>
    {
        public void Configure(EntityTypeBuilder<DatabaseInfo> builder)
        {
            builder.ToTable("DatabaseInfo");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            
            builder
                .Property(p => p.Name)
                .HasColumnName("Name")
                .IsRequired()
                .HasMaxLength(100);

            builder
                .Property(p => p.Description)
                .HasColumnName("Description")
                .IsRequired()
                .HasMaxLength(1000);

            builder
                .Property(p => p.ConnectionString)
                .HasColumnName("ConnectionString")
                .IsRequired()
                .HasMaxLength(500);

            builder
                .Property(p => p.DatabaseTypeId)
                .HasColumnName("DatabaseTypeId")
                .IsRequired();

            builder
                .HasOne(x => x.DatabaseType)
                .WithMany()
                .HasForeignKey(x => x.DatabaseTypeId);
        }
    }

    public class DatabaseTypeConfiguration : IEntityTypeConfiguration<DatabaseType>
    {
        public void Configure(EntityTypeBuilder<DatabaseType> builder)
        {
            builder.ToTable("DatabaseType");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder
                .Property(p => p.ProviderName)
                .HasColumnName("ProviderName")
                .IsRequired()
                .HasMaxLength(100);
        }
    }
    
    public class JobConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder.ToTable("Job");

            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Id).ValueGeneratedOnAdd();                

            builder.Property(p => p.Name)
                .HasColumnName("Name")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Description)
                .HasColumnName("Description")
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(p => p.Cron)
                .HasColumnName("Cron")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Enabled)
                .HasColumnName("Enabled")
                .IsRequired();

            builder.Property(p => p.JobType)
                .HasColumnName("JobType")
                .IsRequired()
                .HasMaxLength(100);
        }
    }

    public class SqlCommandJobConfiguration : IEntityTypeConfiguration<SqlCommandJob>
    {
        public void Configure(EntityTypeBuilder<SqlCommandJob> builder)
        {
            builder.ToTable("SqlCommandJob");

            builder.Property(p => p.Command)
                .HasColumnName("Command")
                .IsRequired();

            builder.Property(p => p.DatabaseInfoId)
                .HasColumnName("DatabaseInfoId")
                .IsRequired();

            builder.HasOne(p => p.DatabaseInfo)
                .WithMany()
                .HasForeignKey(x => x.DatabaseInfoId);
        }
    }

    public class WindowsCommandJobConfiguration : IEntityTypeConfiguration<WindowsCommandJob>
    {
        public void Configure(EntityTypeBuilder<WindowsCommandJob> builder)
        {
            builder.ToTable("WindowsCommandJob");

            builder.Property(p => p.Command)
                .HasColumnName("Command")
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(p => p.Arguments)
                .HasColumnName("Arguments")                
                .HasMaxLength(500);
        }
    }

    public class SqlMailReportJobConfiguration : IEntityTypeConfiguration<SqlMailReportJob>
    {
        public void Configure(EntityTypeBuilder<SqlMailReportJob> builder)
        {
            builder.ToTable("SqlMailReportJob");

            builder.Property(p => p.MailSubject)
                .HasColumnName("MailSubject")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(p => p.MailMessage)
                .HasColumnName("MailMessage")
                .HasMaxLength(1000);

            builder.Property(p => p.To)
                .HasColumnName("To")
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(p => p.Cc)
                .HasColumnName("Cc")
                .HasMaxLength(2000);

            builder.Property(p => p.Cco)
                .HasColumnName("Cco")
                .HasMaxLength(2000);

            builder.Property(p => p.MinRowsToSend)
                .HasColumnName("MinRowsToSend")
                .IsRequired();

            builder.Property(p => p.ResultFormat)
                .HasColumnName("ResultFormat")
                .HasColumnType("INT")
                .IsRequired();

            builder.Property(p => p.Query)
                .HasColumnName("Query")
                .IsRequired();

            builder.Property(p => p.DatabaseInfoId)
                .HasColumnName("DatabaseInfoId")
                .IsRequired();

            builder.HasOne(p => p.DatabaseInfo)
                .WithMany()
                .HasForeignKey(x => x.DatabaseInfoId);
        }
    }

    public class HttpJobConfiguration : IEntityTypeConfiguration<HttpJob>
    {
        public void Configure(EntityTypeBuilder<HttpJob> builder)
        {
            builder.ToTable("HttpJob");

            builder.Property(p => p.Url)
                .HasColumnName("Url")
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(p => p.Method)
                .HasColumnName("Method")
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.ContentType)
                .HasColumnName("ContentType")
                .HasMaxLength(100);

            builder.Property(p => p.Body)
                .HasColumnName("Body");
        }
    }
}
