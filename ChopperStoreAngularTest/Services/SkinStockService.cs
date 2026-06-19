using Microsoft.EntityFrameworkCore;
using ChopperStoreAngularTest.Models;

namespace ChopperStoreAngularTest.Services
{
    public interface ISkinStockService
    {
        Task RemoveSkinAsync(int skinId);
        Task<List<int>> GetReservedSkinIdsAsync();
        Task<bool> AreAnyReservedAsync(IEnumerable<int> skinIds);
    }

    public class SkinStockService : ISkinStockService
    {
        private static readonly TimeSpan ReservationWindow = TimeSpan.FromMinutes(30);

        private readonly ChopperStoreContext _context;

        public SkinStockService(ChopperStoreContext context)
        {
            _context = context;
        }

        public async Task<List<int>> GetReservedSkinIdsAsync()
        {
            var cutoff = DateTime.UtcNow - ReservationWindow;
            return await _context.transactionItems
                .Where(ti => ti.SkinId != null
                    && ti.transaction.PaymentStatus == "pending"
                    && ti.transaction.transactionDate > cutoff)
                .Select(ti => ti.SkinId!.Value)
                .ToListAsync();
        }

        public async Task<bool> AreAnyReservedAsync(IEnumerable<int> skinIds)
        {
            var reserved = await GetReservedSkinIdsAsync();
            return skinIds.Any(reserved.Contains);
        }

        public async Task RemoveSkinAsync(int skinId)
        {
            var cartItems = await _context.items.Where(i => i.SkinId == skinId).ToListAsync();
            _context.items.RemoveRange(cartItems);

            var skin = await _context.skins.FirstOrDefaultAsync(s => s.Id == skinId);
            if (skin != null)
            {
                _context.skins.Remove(skin);
            }

            await _context.SaveChangesAsync();
        }
    }
}
