using Business;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess
{
    public class ArtworkDAO : SingletonBase<ArtworkDAO>
    {
        public async Task<IEnumerable<Artwork>> GetArtworkAll()
        {
            return await _context.Artworks
                                 .Include(a => a.Account) // Tải thông tin Account
                                 .ThenInclude(ac => ac.AccountDetail) // Tải chi tiết của Account
                                 .ToListAsync();
        }


        public async Task<Artwork> GetArtworkById(int id)
        {
            return await _context.Artworks.FirstOrDefaultAsync(a => a.IdArtwork == id);
        }

        public async Task Add(Artwork artwork)
        {
            _context.Artworks.Add(artwork);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Artwork artwork)
        {
            var existingItem = await GetArtworkById(artwork.IdArtwork);
            if (existingItem == null)
            {
                throw new Exception("Artwork không tồn tại.");
            }

            _context.Entry(existingItem).CurrentValues.SetValues(artwork);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var artwork = await GetArtworkById(id);
            if (artwork != null)
            {
                _context.Artworks.Remove(artwork);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> ChangeActive(int id)
        {
            var artwork = await GetArtworkById(id);
            artwork.Active = !artwork.Active;
            await _context.SaveChangesAsync();
            return artwork.Active;
        }
    }
}
