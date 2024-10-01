using Business;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class ProjectDAO : SingletonBase<ProjectDAO>
    {
        public async Task<IEnumerable<Project>> GetProjectAll()
        {
            var projects = await _context.Projects.ToListAsync();
            return projects;
        }
        public async Task<Project> GetProjectById(int id)
        {
            var projects = await _context.Projects.FirstOrDefaultAsync(p => p.IdProject == id);
            if (projects == null) return null;

            return projects;
        }
        public async Task Add(Project projects)
        {
            _context.Projects.Add(projects);
            await _context.SaveChangesAsync();
        }
        public async Task Update(Project projects)
        {

            var existingItem = await GetProjectById(projects.IdProject);
            if (existingItem != null)
            {
                // Cập nhật các thuộc tính cần thiết
                _context.Entry(existingItem).CurrentValues.SetValues(projects);
                await _context.SaveChangesAsync();
            }

        }
        public async Task Delete(int id)
        {
            var projects = await GetProjectById(id);
            if (projects != null)
            {
                _context.Projects.Remove(projects);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> ChangeActive(int id)
        {
            var projects = await GetProjectById(id);
            projects.Active = !projects.Active;
            await _context.SaveChangesAsync();
            return projects.Active;
        }
    }
}
