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
            var reactions = await _context.Reactions
                .Include(re => re.Artwork)
                .Include(re => re.Account)
                .ToListAsync();
            return reactions;
        }

        // Get a reaction by Id with related Artwork and Account entities
        public async Task<Reaction> GetReactionById(int id)
        {
            var reaction = await _context.Reactions
                .Include(re => re.Artwork)
                .Include(re => re.Account)
                .FirstOrDefaultAsync(re => re.IdReaction == id);
            return reaction;
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
