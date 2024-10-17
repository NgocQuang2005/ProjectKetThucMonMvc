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
        Task<int> GetTotalProjects();
        Task<IEnumerable<Project>> GetActiveProjects();      // Dự án đang hoạt động
        Task<IEnumerable<Project>> GetCompletedProjects();   // Dự án đã hoàn thành
    }
}
