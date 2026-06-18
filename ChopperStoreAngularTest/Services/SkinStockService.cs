using Microsoft.EntityFrameworkCore;
using ChopperStoreAngularTest.Models;

namespace ChopperStoreAngularTest.Services
{
    public interface ISkinStockService
    {
        Task RemoveSkinAsync(int skinId);
    }

    public class SkinStockService : ISkinStockService
    {
        private readonly ChopperStoreContext _context;

        public SkinStockService(ChopperStoreContext context)
        {
            _context = context;
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
