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
            var follows = await _context.Follows
                .Select(f => new Follow
                {
                    IdFollow = f.IdFollow,
                    IdFollower = f.IdFollower,
                    IdFollowing = f.IdFollowing,
                    // Lấy thông tin từ bảng Account
                    Follower = _context.Accounts.FirstOrDefault(a => a.IdAccount == f.IdFollower),
                    Following = _context.Accounts.FirstOrDefault(a => a.IdAccount == f.IdFollowing),
                    Active = f.Active,
                    CreatedWhen = f.CreatedWhen,
                    LastUpdateWhen = f.LastUpdateWhen
                }).ToListAsync();

            return follows;
        }

        public async Task<Follow> GetFollowById(int id)
        {
            var follow = await _context.Follows
                .Where(f => f.IdFollow == id)
                .Select(f => new Follow
                {
                    IdFollow = f.IdFollow,
                    IdFollower = f.IdFollower,
                    IdFollowing = f.IdFollowing,
                    // Lấy thông tin từ bảng Account
                    Follower = _context.Accounts.FirstOrDefault(a => a.IdAccount == f.IdFollower),
                    Following = _context.Accounts.FirstOrDefault(a => a.IdAccount == f.IdFollowing),
                    Active = f.Active,
                    CreatedWhen = f.CreatedWhen,
                    LastUpdateWhen = f.LastUpdateWhen
                })
                .FirstOrDefaultAsync();

            return follow;
        }

        public async Task Add(Follow follows)
        {
            try { 
            _context.Follows.Add(follows);
            await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
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
        public async Task<bool> ChangeActive(int id)
        {
            var follows = await _context.Follows.FirstOrDefaultAsync(f => f.IdFollow == id);
            if (follows != null)
            {
                // Thay đổi trạng thái của thuộc tính Active
                follows.Active = !follows.Active;

                // Cập nhật đối tượng Follow
                _context.Follows.Update(follows);
                await _context.SaveChangesAsync();

                return follows.Active;
            }

            return false; // Trả về false nếu không tìm thấy đối tượng Follow
        }

    }
}
