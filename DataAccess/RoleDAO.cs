using Business;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class RoleDAO : SingletonBase<RoleDAO>
    {
        public async Task<IEnumerable<Role>> GetRoleAll()
        {
            var roles = await _context.Roles.ToListAsync();
            return roles;
        }
        public async Task<Role> GetRoleById(int id)
        {
            var roles = await _context.Roles.FirstOrDefaultAsync(r => r.IdRole == id);
            if (roles == null) return null;

            return roles;
        }
        public async Task Add(Role roles)
        {
            _context.Roles.Add(roles);
            await _context.SaveChangesAsync();
        }
        public async Task Update(Role roles)
        {

            var existingItem = await GetRoleById(roles.IdRole);
            if (existingItem != null)
            {
                // Cập nhật các thuộc tính cần thiết
                _context.Entry(existingItem).CurrentValues.SetValues(roles);
                await _context.SaveChangesAsync();
            }

        }
        public async Task Delete(int id)
        {
            var roles = await GetRoleById(id);
            if (roles != null)
            {
                _context.Roles.Remove(roles);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> ChangeActive(int id)
        {
            var role = await GetRoleById(id);
            role.Active = !role.Active;
            await _context.SaveChangesAsync();
            return role.Active;
        }
    }
}
