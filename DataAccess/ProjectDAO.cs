using Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
            var projects = await _context.Projects.Include(p => p.Account) // Include Account
                                       .ThenInclude(a => a.AccountDetail).AsNoTracking()
                                       .ToListAsync();
            return projects;
        }
        public async Task<Project> GetProjectById(int id)
        {
            var projects = await _context.Projects
                .Include(p => p.DocumentInfos)
                .Include(p => p.Account) // Include Account
                .ThenInclude(a => a.AccountDetail) // Include AccountDetail
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.IdProject == id);
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

            _context.Entry(projects).State = EntityState.Modified;
            await _context.SaveChangesAsync();

        }
        public async Task Delete(int id)
        {
            var projects = await GetProjectById(id);
            if (projects != null)
            {
                _context.Entry(projects).State = EntityState.Detached;

                if (projects.Account?.AccountDetail != null)
                {
                    _context.Entry(projects.Account.AccountDetail).State = EntityState.Detached;
                }

                foreach (var doc in projects.DocumentInfos)
                {
                    _context.Entry(doc).State = EntityState.Detached;
                }

                _context.Projects.Remove(projects);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> ChangeActive(int id)
        {
            var projects = await GetProjectById(id);
            if (projects != null)
            {
                projects.Active = !projects.Active;
                _context.Entry(projects).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return projects.Active;
            }
            return false;
        }
    }
}
