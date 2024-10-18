using Business;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class ProjectRepository : IProjectRepository
    {
        public async Task Add(Project project)
        {
            await ProjectDAO.Instance.Add(project);
        }

        public async Task Delete(int id)
        {
            await ProjectDAO.Instance.Delete(id);
        }

        public async Task<IEnumerable<Project>> GetProjectAll()
        {
            return await ProjectDAO.Instance.GetProjectAll();
        }

        public async Task<Project> GetProjectById(int id)
        {
            return await ProjectDAO.Instance.GetProjectById(id);
        }
        public async Task Update(Project project)
        {
            await ProjectDAO.Instance.Update(project);
        }
        public async Task<bool> ChangeActive(int id)
        {
            return await ProjectDAO.Instance.ChangeActive(id);
        }
        public async Task<int> GetTotalProjects()
        {
            return await ProjectDAO.Instance.GetTotalProjects();
        }
        // Lấy tất cả dự án đang hoạt động
        public async Task<IEnumerable<Project>> GetActiveProjects()
        {
            return await ProjectDAO.Instance.GetActiveProjects();
        }

        // Lấy tất cả dự án đã hoàn thành
        public async Task<IEnumerable<Project>> GetCompletedProjects()
        {
            return await ProjectDAO.Instance.GetCompletedProjects();
        }
        public void Detach(Project project)
        {
            ProjectDAO.Instance.Detach(project);
        }

    }
}
