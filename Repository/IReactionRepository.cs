using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IReactionRepository
    {
        Task<IEnumerable<Reaction>> GetReactionAll();
        Task<Reaction> GetReactionById(int id);
        Task Add(Reaction reaction);
        Task Update(Reaction reaction);
        Task Delete(int id);
    }
}
