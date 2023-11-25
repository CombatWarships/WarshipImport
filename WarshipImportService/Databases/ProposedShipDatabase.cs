using Microsoft.EntityFrameworkCore;
using Serilog;
using WarshipImport.Data;
using WarshipImport.Interfaces;

namespace WarshipImport.Databases
{
	public class ProposedShipDatabase : DbContext, IProposedShipsDatabase
    {
        private readonly IConfiguration _configuration;

        public ProposedShipDatabase(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<ProposedShip> ProposedShips { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = _configuration.GetConnectionString("DBConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                Log.Error($"Database connection string is NULL");
                return;
            }
            Log.Information("Connecting to SQL");
            optionsBuilder.UseSqlServer(connectionString);
        }

        public async Task<ProposedShip> CreateOrUpdate(ProposedShip ship, string userId)
        {
            if (ship == null)
                throw new ArgumentNullException(nameof(ship));
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException($"'{nameof(userId)}' cannot be null or empty.", nameof(userId));

            ship.UserID = userId;

            if (ship.ID != null && ship.ID != Guid.Empty)
            {
                var existingShip = ProposedShips.Where(s => s.ID == ship.ID).AsNoTracking().FirstOrDefault();
                if (existingShip == null)
                    throw new ArgumentException($"Cannot update ship with ID {ship.ID}, it does not exist.");

                if (existingShip.UserID != ship.UserID)
                    throw new ArgumentException($"Cannot delete ship with ID {ship.ID}, it does not belong to user {existingShip.UserID}.");

                var result = ProposedShips.Update(ship);
                await SaveChangesAsync();
                return result.Entity;
            }
            else
            {
                ship.ID = Guid.NewGuid();
                var result = ProposedShips.Add(ship);
                await SaveChangesAsync();
                return result.Entity;
            }
        }

        public List<ProposedShip> GetUserShips(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            List<ProposedShip> usersShips = ProposedShips.Where(s => s.UserID == userId).ToList();

            return usersShips;
        }

        public ProposedShip? GetShip(Guid shipId)
        {
            if (shipId == Guid.Empty)
                throw new ArgumentNullException(nameof(shipId));

            return ProposedShips.FirstOrDefault(s => s.ID == shipId);
        }

        public async Task<bool> Delete(Guid shipId, string userId)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            if (shipId == Guid.Empty)
                throw new ArgumentException($"Cannot delete ship with ID {shipId}.");

            var existingShip = ProposedShips.Where(s => s.ID == shipId).FirstOrDefault();
            if (existingShip == null)
                throw new ArgumentException($"Cannot delete ship with ID {shipId}, it does not exist.");

            if (existingShip.UserID != userId)
                throw new ArgumentException($"Cannot delete ship with ID {shipId}, it does not belong to user {userId}.");

            var result = ProposedShips.Remove(existingShip);
            await SaveChangesAsync();

            return result.State == EntityState.Deleted;
        }
    }
}