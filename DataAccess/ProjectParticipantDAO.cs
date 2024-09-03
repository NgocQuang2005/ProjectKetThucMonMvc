using Business;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class ProjectParticipantDAO : SingletonBase<ProjectParticipantDAO>
    {
        public async Task<IEnumerable<ProjectParticipant>> GetProjectParticipantAll()
        {
            var projectParticipant = await _context.ProjectParticipants.ToListAsync();
            return projectParticipant;
        }
        public async Task<ProjectParticipant> GetProjectParticipantById(int id)
        {
            var projectParticipant = await _context.ProjectParticipants.FirstOrDefaultAsync(pp => pp.IdProjectParticipant == id);
            if (projectParticipant == null) return null;

            return projectParticipant;
        }
        public async Task Add(ProjectParticipant projectParticipant)
        {
            _context.ProjectParticipants.Add(projectParticipant);
            await _context.SaveChangesAsync();
        }
        public async Task Update(ProjectParticipant projectParticipant)
        {

            var existingItem = await GetProjectParticipantById(projectParticipant.IdProjectParticipant);
            if (existingItem != null)
            {
                // Cập nhật các thuộc tính cần thiết
                _context.Entry(existingItem).CurrentValues.SetValues(projectParticipant);
                await _context.SaveChangesAsync();
            }

        }
        public async Task Delete(int id)
        {
            var projectParticipant = await GetProjectParticipantById(id);
            if (projectParticipant != null)
            {
                _context.ProjectParticipants.Remove(projectParticipant);
                await _context.SaveChangesAsync();
            }
        }
    }
}
