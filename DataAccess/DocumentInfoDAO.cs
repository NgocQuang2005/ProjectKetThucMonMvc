using Business;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DocumentInfoDAO : SingletonBase<DocumentInfoDAO>
    {
        private readonly ApplicationDbContext _context; // Đảm bảo bạn đã khai báo đúng DbContext

        public DocumentInfoDAO()
        {
            _context = new ApplicationDbContext(); // Thay YourDbContext bằng tên thực tế
        }

        // Phương thức lấy tất cả DocumentInfo
        public async Task<IEnumerable<DocumentInfo>> GetDocumentInfoAll()
        {
            return await _context.DocumentInfos
                .Include(d => d.Account) // Bao gồm dữ liệu từ bảng liên quan
                .Include(d => d.IdArtworkNavigation)
                .Include(d => d.IdProjectNavigation)
                .Include(d => d.IdEventNavigation)
                .ToListAsync();
        }

        // Phương thức lấy DocumentInfo theo ID
        public async Task<DocumentInfo> GetDocumentInfoById(int id)
        {
            return await _context.DocumentInfos
                .Include(d => d.Account) // Bao gồm dữ liệu từ bảng liên quan
                .Include(d => d.IdArtworkNavigation)
                .Include(d => d.IdProjectNavigation)
                .Include(d => d.IdEventNavigation)
                .FirstOrDefaultAsync(df => df.IdDcIf == id);
        }

        // Phương thức thêm DocumentInfo
        public async Task Add(DocumentInfo documentInfo)
        {
            await _context.DocumentInfos.AddAsync(documentInfo);
            await _context.SaveChangesAsync();
        }

        // Phương thức cập nhật DocumentInfo
        public async Task Update(DocumentInfo documentInfo)
        {
            var existingItem = await GetDocumentInfoById(documentInfo.IdDcIf);
            if (existingItem != null)
            {
                _context.Entry(existingItem).CurrentValues.SetValues(documentInfo);
                await _context.SaveChangesAsync();
            }
        }

        // Phương thức xóa DocumentInfo
        public async Task Delete(int id)
        {
            var documentInfo = await GetDocumentInfoById(id);
            if (documentInfo != null)
            {
                _context.DocumentInfos.Remove(documentInfo);
                await _context.SaveChangesAsync();
            }
        }

        // Phương thức thay đổi trạng thái hoạt động của DocumentInfo
        public async Task<bool> ChangeActive(int id)
        {
            var documentInfo = await GetDocumentInfoById(id);
            if (documentInfo != null)
            {
                documentInfo.Active = !documentInfo.Active;
                await _context.SaveChangesAsync();
                return documentInfo.Active;
            }
            return false; // Nếu không tìm thấy documentInfo, trả về false
        }
        // Thêm phương thức này để lấy DocumentInfo theo IdAccount
        public async Task<DocumentInfo> GetDocumentInfoByAccountId(int accountId)
        {
            using (var context = new ApplicationDbContext())
            {
                return await context.DocumentInfos.FirstOrDefaultAsync(di => di.IdAc == accountId);
            }
        }


        // Thêm các phương thức lấy dữ liệu từ các bảng liên quan nếu cần
        public async Task<IEnumerable<AccountDetail>> GetAccountDetailAll()
        {
            return await _context.AccountDetails.ToListAsync();
        }

        public async Task<IEnumerable<Artwork>> GetArtworkAll()
        {
            return await _context.Artworks.ToListAsync();
        }

        public async Task<IEnumerable<Project>> GetProjectAll()
        {
            return await _context.Projects.ToListAsync();
        }
        
        public async Task<IEnumerable<Event>> GetEventAll()
        {
            return await _context.Events.ToListAsync();
        }
        public async Task<IEnumerable<DocumentInfo>> GetDocumentInfosByArtworkId(int artworkId)
        {
            return await _context.DocumentInfos
                                 .Where(d => d.IdArtwork == artworkId)
                                 .ToListAsync();
        }
        public async Task<IEnumerable<DocumentInfo>> GetDocumentInfoByEventId(int eventId)
        {
            return await _context.DocumentInfos.Where(di => di.IdEvent == eventId).ToListAsync();
        }
        public async Task<IEnumerable<DocumentInfo>> GetDocumentInfoByProjectId(int eventId)
        {
            return await _context.DocumentInfos.Where(di => di.IdProject == eventId).ToListAsync();
        }
    }
}
