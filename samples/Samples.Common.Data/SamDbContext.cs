using Common.VNextFramework.EntityFramework;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Samples.Common.Data.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Samples.Common.Data
{
    public class SamDbContext : BCChinaDbContext<SamDbContext>
    {
        private readonly IServiceProvider _serviceProvider;
        public SamDbContext(DbContextOptions<SamDbContext> options, IServiceProvider serviceProvider) : base(options, serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        #region AddTimestamps

        private void AddTimestamps()
        {
            var dateTimeNow = DateTime.UtcNow;
            var userName = _serviceProvider.GetService<IHttpContextAccessor>()?.HttpContext?.User.Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            var trackingEntities = ChangeTracker.Entries().Where(x =>
                x.Entity is BaseAuditEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var trackingEntity in trackingEntities)
            {
                var entity = (BaseAuditEntity)trackingEntity.Entity;

                if (trackingEntity.State == EntityState.Added)
                {
                    entity.CreatedOn = entity.CreatedOn == null ? dateTimeNow : entity.CreatedOn;
                    entity.CreatedBy = userName;
                    entity.IsDeleted = false;
                }
                else
                {
                    entity.LastModifiedOn = dateTimeNow;
                    entity.LastModifiedBy = userName;
                }
            }
        }

        #endregion AddTimestamps

        #region OnModelCreating

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Registration>().HasQueryFilter(x => x.IsDeleted == false);

            //modelBuilder.Entity<BcilLocationHierarchyOrs>(e =>
            //{
            //    e.Property(x => x.ApprovedProducts).HasJsonConversion();
            //    e.Property(x => x.LocationAddresses).HasJsonConversion();
            //});

            //// use ef core computed-columns for FullName
            //modelBuilder.Entity<Examiner>().Property(p => p.FullName)
            //    .HasComputedColumnSql("[FirstName] + ' ' + [LastName]", true);

            //var encryptionConfig = _serviceProvider.GetRequiredService<IOptions<EfEncryptionConfig>>();
            //if (!string.IsNullOrEmpty(encryptionConfig.Value.Key))
            //{
            //    var provider = new AesProvider(Encoding.UTF8.GetBytes(encryptionConfig.Value.Key),
            //        Encoding.UTF8.GetBytes(encryptionConfig.Value.Iv));
            //    modelBuilder.UseEncryption(provider);
            //}

            //modelBuilder.Entity<Incident>()
            //    .Property(prop => prop.No)
            //    .UseIdentityColumn(10000001);

            foreach (var mutableEntityType in modelBuilder.Model.GetEntityTypes())
                if (mutableEntityType.ClrType.IsSubclassOf(typeof(GuidEntity)))
                    mutableEntityType.SetTableName(mutableEntityType.ClrType.Name);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
                relationship.DeleteBehavior = DeleteBehavior.Restrict;

            foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetProperties()))
            {
                if (property.ClrType == typeof(string))
                    if (property.GetMaxLength() == null)
                        property.SetMaxLength(256);
                // 枚举生成字符串
                if (property.ClrType.IsEnum || Nullable.GetUnderlyingType(property.ClrType)?.IsEnum == true)
                    property.SetColumnType("nvarchar(256)");
            }

            base.OnModelCreating(modelBuilder);
        }

        #endregion OnModelCreating

        #region Entitys
        public virtual DbSet<User> Users { get; set; }
        #endregion
    }
}
