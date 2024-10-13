using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IFollowRepository
    {
        Task<IEnumerable<Follow>> GetFollowAll();
        Task<Follow> GetFollowById(int id);
        Task Add(Follow follow);
        Task Update(Follow follow);
        Task Delete(int id);
        Task<bool> ChangeActive(int id);

        // Thêm phương thức để lấy tất cả các đối tượng Follow của một người dùng
        Task<Follow> GetFollowByUsers(int followerId, int followingId);
    }

}
