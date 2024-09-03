using Business;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class TypeOfArtworkDAO : SingletonBase<TypeOfArtworkDAO>
    {
        public async Task<IEnumerable<TypeOfArtwork>> GetTypeOfArtworkAll()
        {
            var typeOfArtworks = await _context.TypeOfArtworks.ToListAsync();
            return typeOfArtworks;
        }
        public async Task<TypeOfArtwork> GetTypeOfArtworkById(int id)
        {
            var typeOfArtworks = await _context.TypeOfArtworks.FirstOrDefaultAsync(r => r.IdTypeOfArtwork == id);
            if (typeOfArtworks == null) return null;

            return typeOfArtworks;
        }
        public async Task Add(TypeOfArtwork typeOfArtworks)
        {
            _context.TypeOfArtworks.Add(typeOfArtworks);
            await _context.SaveChangesAsync();
        }
        public async Task Update(TypeOfArtwork typeOfArtworks)
        {

            var existingItem = await GetTypeOfArtworkById(typeOfArtworks.IdTypeOfArtwork);
            if (existingItem != null)
            {
                // Cập nhật các thuộc tính cần thiết
                _context.Entry(existingItem).CurrentValues.SetValues(typeOfArtworks);
                await _context.SaveChangesAsync();
            }

        }
        public async Task Delete(int id)
        {
            var typeOfArtworks = await GetTypeOfArtworkById(id);
            if (typeOfArtworks != null)
            {
                _context.TypeOfArtworks.Remove(typeOfArtworks);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> ChangeActive(int id)
        {
            var typeOfArtworks = await GetTypeOfArtworkById(id);
            typeOfArtworks.Active = !typeOfArtworks.Active;
            await _context.SaveChangesAsync();
            return typeOfArtworks.Active;
        }
    }
}
