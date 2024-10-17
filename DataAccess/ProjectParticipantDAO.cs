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
            var projectParticipants = await _context.ProjectParticipants
                .Include(pp => pp.Account)  // Nạp dữ liệu liên quan từ bảng Account
                .Include(pp => pp.Project)  // Nạp dữ liệu liên quan từ bảng Project
                .ToListAsync();
            return projectParticipants;
        }

        public async Task<ProjectParticipant> GetProjectParticipantById(int id)
        {
            var projectParticipant = await _context.ProjectParticipants
                .Include(pp => pp.Project)   // Nạp dữ liệu từ bảng Project
                .Include(pp => pp.Account)   // Nạp dữ liệu từ bảng Account
                .FirstOrDefaultAsync(pp => pp.IdProjectParticipant == id);

            return projectParticipant;
        }

        public async Task Add(ProjectParticipant projectParticipant)
        {
            try
            {
                _context.ProjectParticipants.Add(projectParticipant);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi lưu dữ liệu: " + ex.Message);
                throw;
            }
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
        public async Task<bool> ChangeActive(int id)
        {
            var projectParticipant = await GetProjectParticipantById(id);
            projectParticipant.Active = !projectParticipant.Active;
            await _context.SaveChangesAsync();
            return projectParticipant.Active;
        }
        public async Task DeleteByProjectId(int projectId)
        {
            var participants = await _context.ProjectParticipants
                                             .Where(pp => pp.IdProject == projectId)
                                             .ToListAsync();
            _context.ProjectParticipants.RemoveRange(participants);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<ProjectParticipant>> GetProjectParticipantsByProjectId(int projectId)
        {
            // Lấy danh sách người tham gia dự án dựa trên IdProject
            return await _context.ProjectParticipants
                .Where(pp => pp.IdProject == projectId)
                .Include(pp => pp.Account)  // Nạp dữ liệu liên quan từ bảng Account nếu cần
                .Include(pp => pp.Project)  // Nạp dữ liệu liên quan từ bảng Project nếu cần
                .ToListAsync();
        }
        public async Task<ProjectParticipant> GetProjectParticipantByProjectAndAccount(int projectId, int accountId)
        {
            return await _context.ProjectParticipants
                                 .FirstOrDefaultAsync(pp => pp.IdProject == projectId && pp.IdAc == accountId);
        }
        public async Task<int> GetParticipantCountByProjectId(int projectId)
        {
            var participantCount = await _context.ProjectParticipants
                .CountAsync(pp => pp.IdProject == projectId);  // Đếm số lượng người tham gia dự án theo IdProject
            return participantCount;
        }
    }
}
