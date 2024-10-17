using Business;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess
{
    public class ArtworkDAO : SingletonBase<ArtworkDAO>
    {
        
        public async Task<IEnumerable<Artwork>> GetArtworkAll()
        {
            return await _context.Artworks
                                 .Include(a => a.Account) 
                                 .ThenInclude(ac => ac.AccountDetail) 
                                 .Include(a => a.DocumentInfos)
                                 .Include(a => a.Reactions)
                                 .AsNoTracking() //  để tối ưu hóa đọc dữ liệu
                                 .ToListAsync();
        }

        // Lấy chi tiết tác phẩm theo ID, bao gồm cả DocumentInfos
        public async Task<Artwork> GetArtworkById(int id)
        {
            return await _context.Artworks
                                 .Include(a => a.DocumentInfos)
                                 .Include(a => a.Account)
                                 .ThenInclude(ac => ac.AccountDetail)
                                 .Include(a => a.Reactions)
                                 .AsNoTracking() // Tối ưu hóa cho truy vấn chỉ đọc
                                 .FirstOrDefaultAsync(a => a.IdArtwork == id);
        }
        public async Task<int> GetTotalArtwork()
        {
            return await _context.Artworks
                                   // Thêm để tối ưu hóa
                                 .CountAsync();
        }


        // Thêm mới một Artwork
        public async Task Add(Artwork artwork)
        {
            _context.Artworks.Add(artwork);
            await _context.SaveChangesAsync();
        }

        // Cập nhật thông tin Artwork
        public async Task Update(Artwork artwork)
        {
            var existingItem = await _context.Artworks.FindAsync(artwork.IdArtwork);

            if (existingItem == null)
            {
                throw new Exception("Artwork không tồn tại.");
            }

            _context.Entry(existingItem).State = EntityState.Detached;

            _context.Artworks.Attach(artwork);
            _context.Entry(artwork).State = EntityState.Modified;

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
            if (artwork == null)
            {
                throw new Exception("Artwork không tồn tại.");
            }

            artwork.Active = !artwork.Active;
            await _context.SaveChangesAsync();
            return artwork.Active;
        }
        public async Task<Artwork> GetArtworkByIdAsNoTracking(int id)
        {
            return await _context.Artworks
                                 .Include(a => a.DocumentInfos)
                                 .Include(a => a.Account)
                                 .ThenInclude(ac => ac.AccountDetail)
                                 .AsNoTracking() // Tối ưu hóa cho truy vấn không theo dõi
                                 .FirstOrDefaultAsync(a => a.IdArtwork == id);
        }

    }
}
