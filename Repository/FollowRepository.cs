using Business;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class FollowRepository : IFollowRepository
    {
        public async Task Add(Follow follow)
        {
            await FollowDAO.Instance.Add(follow);
        }

        public async Task Delete(int id)
        {
            await FollowDAO.Instance.Delete(id);
        }

        public async Task<IEnumerable<Follow>> GetFollowAll()
        {
            return await FollowDAO.Instance.GetFollowAll();
        }


        public async Task<Follow> GetFollowById(int id)
        {
            return await FollowDAO.Instance.GetFollowById(id);
        }
        public async Task Update(Follow follow)
        {
            await FollowDAO.Instance.Update(follow);
        }
        public async Task<bool> ChangeActive(int id)
        {   
            return await FollowDAO.Instance.ChangeActive(id);
        }
        public async Task<Follow> GetFollowByUsers(int followerId, int followingId)
        {
            var follows = await FollowDAO.Instance.GetFollowAll(); // Lấy kết quả từ Task
            return follows.Where(f => f.IdFollower == followerId && f.IdFollowing == followingId).FirstOrDefault();

        }

    }
}
