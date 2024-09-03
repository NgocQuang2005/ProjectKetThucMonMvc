using Business;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class FollowDAO : SingletonBase<FollowDAO>
    {
        public async Task<IEnumerable<Follow>> GetFollowAll()
        {
            var follows = await _context.Follows.ToListAsync();
            return follows;
        }
        public async Task<Follow> GetFollowById(int id)
        {
            var follows = await _context.Follows.FirstOrDefaultAsync(f => f.IdFollow == id);
            if (follows == null) return null;

            return follows;
        }
        public async Task Add(Follow follows)
        {
            _context.Follows.Add(follows);
            await _context.SaveChangesAsync();
        }
        public async Task Update(Follow follows)
        {

            var existingItem = await GetFollowById(follows.IdFollow);
            if (existingItem != null)
            {
                // Cập nhật các thuộc tính cần thiết
                _context.Entry(existingItem).CurrentValues.SetValues(follows);
                await _context.SaveChangesAsync();
            }

        }
        public async Task Delete(int id)
        {
            var follows = await GetFollowById(id);
            if (follows != null)
            {
                _context.Follows.Remove(follows);
                await _context.SaveChangesAsync();
            }
        }
    }
}
