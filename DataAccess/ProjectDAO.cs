using Business;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess
{
    public class ProjectDAO : SingletonBase<ProjectDAO>
    {
        // Lấy tất cả dự án, bao gồm cả Account và AccountDetail
        public async Task<IEnumerable<Project>> GetProjectAll()
        {
            var projects = await _context.Projects
                                         .Include(p => p.Account) // Include Account
                                         .ThenInclude(a => a.AccountDetail)
                                         .AsNoTracking() // Không theo dõi để tránh xung đột
                                         .ToListAsync();
            return projects;
        }

        // Lấy dự án theo ID, bao gồm DocumentInfos và thông tin tài khoản
        public async Task<Project> GetProjectById(int id)
        {
            var project = await _context.Projects
                                        .Include(p => p.DocumentInfos) // Bao gồm DocumentInfos
                                        .Include(p => p.Account) // Bao gồm Account
                                        .ThenInclude(a => a.AccountDetail) // Bao gồm AccountDetail
                                        .AsNoTracking() // Không theo dõi để tránh xung đột
                                        .FirstOrDefaultAsync(p => p.IdProject == id);
            return project;
        }

        // Thêm mới dự án vào cơ sở dữ liệu
        public async Task Add(Project project)
        {
            _context.Projects.Add(project); // Thêm project mới
            await _context.SaveChangesAsync(); // Lưu thay đổi vào DB
        }

        // Cập nhật dự án, chỉ thay đổi EntityState nếu cần thiết
        public async Task Update(Project project)
        {
            // Kiểm tra nếu project đã tồn tại và đã được theo dõi
            var existingProject = await GetProjectById(project.IdProject);
            if (existingProject != null)
            {
                _context.Entry(project).State = EntityState.Modified; // Đánh dấu là đã sửa đổi
                await _context.SaveChangesAsync(); // Lưu thay đổi vào DB
            }
        }

        // Xóa dự án theo ID, đồng thời xóa các liên kết liên quan
        public async Task Delete(int id)
        {
            var project = await GetProjectById(id); // Lấy project theo ID
            if (project != null)
            {
                // Xóa các DocumentInfos liên quan nếu có
                _context.DocumentInfos.RemoveRange(project.DocumentInfos);

                _context.Projects.Remove(project); // Xóa project
                await _context.SaveChangesAsync(); // Lưu thay đổi vào DB
            }
        }

        // Thay đổi trạng thái kích hoạt của dự án
        public async Task<bool> ChangeActive(int id)
        {
            var project = await GetProjectById(id); // Lấy project theo ID
            if (project != null)
            {
                project.Active = !project.Active; // Đảo ngược trạng thái Active
                _context.Entry(project).State = EntityState.Modified; // Đánh dấu là đã sửa đổi
                await _context.SaveChangesAsync(); // Lưu thay đổi vào DB
                return project.Active;
            }
            return false;
        }

        // Lấy tất cả các dự án đang hoạt động (chưa kết thúc)
        public async Task<IEnumerable<Project>> GetActiveProjects()
        {
            var activeProjects = await _context.Projects
                                               .Where(p => p.Active && p.EndDate > DateTime.Now)  // Dự án đang hoạt động và chưa kết thúc
                                               .Include(p => p.Account)
                                               .ThenInclude(a => a.AccountDetail)
                                               .AsNoTracking() // Không theo dõi để tránh xung đột
                                               .ToListAsync();
            return activeProjects;
        }

        // Lấy tất cả dự án đã hoàn thành (đã kết thúc)
        public async Task<IEnumerable<Project>> GetCompletedProjects()
        {
            var completedProjects = await _context.Projects
                                                  .Where(p => p.EndDate <= DateTime.Now)  // Dự án đã kết thúc
                                                  .Include(p => p.Account)
                                                  .ThenInclude(a => a.AccountDetail)
                                                  .AsNoTracking() // Không theo dõi để tránh xung đột
                                                  .ToListAsync();
            return completedProjects;
        }

        // Lấy tổng số lượng dự án
        public async Task<int> GetTotalProjects()
        {
            return await _context.Projects.AsNoTracking().CountAsync();
        }
    }
}
