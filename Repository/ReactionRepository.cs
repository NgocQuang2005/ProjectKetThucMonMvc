using Business;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class ReactionRepository : IReactionRepository
    {
        public async Task Add(Reaction reaction)
        {
            await ReactionDAO.Instance.Add(reaction);
        }

        public async Task Delete(int id)
        {
            await ReactionDAO.Instance.Delete(id);
        }

        public async Task<IEnumerable<Reaction>> GetReactionAll()
        {
            return await ReactionDAO.Instance.GetReactionAll();
        }

        public async Task<Reaction> GetReactionById(int id)
        {
            return await ReactionDAO.Instance.GetReactionById(id);
        }
        public async Task Update(Reaction reaction)
        {
            await ReactionDAO.Instance.Update(reaction);
        }
    }
}
