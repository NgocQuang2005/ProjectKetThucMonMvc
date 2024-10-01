using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IProjectRepository
    {
        Task<IEnumerable<Project>> GetProjectAll();
        Task<Project> GetProjectById(int id);
        Task Add(Project project);
        Task Update(Project project);
        Task Delete(int id);
        Task<bool> ChangeActive(int id);

    }
}
