using Business;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class ArtworkDAO : SingletonBase<ArtworkDAO>
    {
        public async Task<IEnumerable<Artwork>> GetArtworkAll()
        {
            var artwork = await _context.Artworks.ToListAsync();
            return artwork;
        }
        public async Task<Artwork> GetArtworkById(int id)
        {
            var artwork = await _context.Artworks.FirstOrDefaultAsync(a => a.IdArtwork == id);
            if (artwork == null) return null;

            return artwork;
        }
        public async Task Add(Artwork artwork)
        {
            _context.Artworks.Add(artwork);
            await _context.SaveChangesAsync();
        }
        public async Task Update(Artwork artwork)
        {

            var existingItem = await GetArtworkById(artwork.IdArtwork);
            if (existingItem != null)
            {
                // Cập nhật các thuộc tính cần thiết
                _context.Entry(existingItem).CurrentValues.SetValues(artwork);
                await _context.SaveChangesAsync();
            }

        }
        public async Task Delete(int id)
        {
            var Artwork = await GetArtworkById(id);
            if (Artwork != null)
            {
                _context.Artworks.Remove(Artwork);
                await _context.SaveChangesAsync();
            }
        }
    }
}
