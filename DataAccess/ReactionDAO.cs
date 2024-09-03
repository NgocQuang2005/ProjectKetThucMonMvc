using Business;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class ReactionDAO : SingletonBase<ReactionDAO>
    {
        public async Task<IEnumerable<Reaction>> GetReactionAll()
        {
            var reactions = await _context.Reactions.ToListAsync();
            return reactions;
        }
        public async Task<Reaction> GetReactionById(int id)
        {
            var reactions = await _context.Reactions.FirstOrDefaultAsync(re => re.IdReaction == id);
            if (reactions == null) return null;

            return reactions;
        }
        public async Task Add(Reaction reactions)
        {
            _context.Reactions.Add(reactions);
            await _context.SaveChangesAsync();
        }
        public async Task Update(Reaction reactions)
        {

            var existingItem = await GetReactionById(reactions.IdReaction);
            if (existingItem != null)
            {
                // Cập nhật các thuộc tính cần thiết
                _context.Entry(existingItem).CurrentValues.SetValues(reactions);
                await _context.SaveChangesAsync();
            }

        }
        public async Task Delete(int id)
        {
            var reactions = await GetReactionById(id);
            if (reactions != null)
            {
                _context.Reactions.Remove(reactions);
                await _context.SaveChangesAsync();
            }
        }
    }
}
