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
        public async Task Update(Follow follow)
        {
            try
            {
                var existingItem = await _context.Follows.FindAsync(follow.IdFollow); // Lấy trực tiếp từ DB

                if (existingItem != null)
                {
                    // Cập nhật trạng thái Active
                    existingItem.Active = follow.Active;
                    existingItem.LastUpdateWhen = DateTime.Now;

                    // Ghi log để kiểm tra trạng thái trước khi cập nhật
                    Console.WriteLine($"Trạng thái trước cập nhật: {existingItem.Active}");

                    _context.Follows.Update(existingItem); // Cập nhật đối tượng
                    await _context.SaveChangesAsync(); // Lưu thay đổi

                    // Ghi log để kiểm tra trạng thái sau khi cập nhật
                    Console.WriteLine($"Trạng thái sau cập nhật: {existingItem.Active}");
                }
                else
                {
                    Console.WriteLine("Không tìm thấy bản ghi Follow để cập nhật.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật Follow: {ex.Message}");
                throw;
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
