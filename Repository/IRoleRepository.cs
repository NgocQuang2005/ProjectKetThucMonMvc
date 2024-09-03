using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetRoleAll();
        Task<Role> GetRoleById(int id);
        Task Add(Role roles);
        Task Update(Role roles);
        Task Delete(int id);
        Task<bool> ChangeActive(int id);
    }
}
