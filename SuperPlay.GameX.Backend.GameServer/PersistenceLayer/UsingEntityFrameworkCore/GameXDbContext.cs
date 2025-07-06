using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SuperPlay.GameX.Backend.GameServer.DomainLayer.Data;
using SuperPlay.GameX.Shared.DomainLayer.Data;

namespace SuperPlay.GameX.Backend.GameServer.PersistenceLayer.UsingEntityFrameworkCore
{
    internal class GameXDbContext : DbContext
    {
        internal GameXDbContext(DbContextOptions<GameXDbContext> options) : base(options) { }

        public DbSet<Player> Players { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var playerIdConverter = new ValueConverter<PlayerId, int>(
                id => id.Value,
                value => new PlayerId(value)
            );


            var deviceIdConverter = new ValueConverter<DeviceId, string>(
                id => id.Value,
                value => new DeviceId(value)
            );

            var resourceValueConverter = new ValueConverter<ResourceValue, int>(
                id => id.Value,
                value => new ResourceValue(value)
            );

            var resourceValueComparer = new ValueComparer<ResourceValue>(
                (a, b) => a.Value == b.Value,
                id => id.Value.GetHashCode(),
                id => new ResourceValue(id.Value)
            );

            modelBuilder.Entity<Player>(b =>
            {
                b.HasKey(e => e.PlayerId);

                b.Property(e => e.PlayerId)
                    .HasConversion(playerIdConverter)
                    .ValueGeneratedOnAdd();

                b.Property(e => e.DeviceId)
                    .HasConversion(deviceIdConverter);

                b.Property(e => e.Coins)
                    .HasConversion(resourceValueConverter)
                    .Metadata.SetValueComparer(resourceValueComparer);

                b.Property(e => e.Rolls)
                    .HasConversion(resourceValueConverter)
                    .Metadata.SetValueComparer(resourceValueComparer);

                b.Property(p => p.RowVersion)
                    .IsRowVersion();

                b.HasIndex(e => e.DeviceId);
            });
        }

    }

}
