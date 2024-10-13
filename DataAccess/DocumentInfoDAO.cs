using Business;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DocumentInfoDAO : SingletonBase<DocumentInfoDAO>
    {
        private readonly ApplicationDbContext _context;

        public DocumentInfoDAO()
        {
            _context = new ApplicationDbContext();
        }

        public async Task Add(DocumentInfo documentInfo)
        {
            await _context.DocumentInfos.AddAsync(documentInfo);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var documentInfo = await _context.DocumentInfos.FindAsync(id);
            if (documentInfo != null)
            {
                // Xóa trực tiếp mà không cần Detach
                _context.DocumentInfos.Remove(documentInfo);
                await _context.SaveChangesAsync();
            }
        }




        public async Task<IEnumerable<DocumentInfo>> GetDocumentInfoAll()
        {
            return await _context.DocumentInfos
                    .Include(d => d.Account)
                    .Include(d => d.IdEventNavigation)
                    .Include(d => d.IdArtworkNavigation)
                    .Include(d => d.IdProjectNavigation)
                 .ToListAsync();
        }

        public async Task<DocumentInfo?> GetDocumentInfoById(int id)
        {
            return await _context.DocumentInfos
                .Include(d => d.Account)
                .Include(d => d.IdEventNavigation)
                .Include(d => d.IdArtworkNavigation)
                .Include(d => d.IdProjectNavigation)
                .AsNoTracking()  // Duy trì AsNoTracking cho truy vấn chỉ đọc
                .FirstOrDefaultAsync(df => df.IdDcIf == id);
        }


        public async Task Update(DocumentInfo documentInfo)
        {
            var existingItem = await _context.DocumentInfos
                .AsNoTracking() // Đảm bảo không theo dõi thực thể cũ
                .FirstOrDefaultAsync(df => df.IdDcIf == documentInfo.IdDcIf);

            if (existingItem != null)
            {
                // Đặt thực thể vào trạng thái 'Modified'
                _context.Entry(documentInfo).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }


        public async Task<bool> ChangeActive(int id)
        {
            var documentInfo = await GetDocumentInfoById(id);
            if (documentInfo != null)
            {
                documentInfo.Active = !documentInfo.Active;
                await _context.SaveChangesAsync();
                return documentInfo.Active;
            }
            return false;
        }

        // Thêm phương thức GetDocumentInfoByAccountId
        public async Task<DocumentInfo?> GetDocumentInfoByAccountId(int accountId)
        {
            return await _context.DocumentInfos.FirstOrDefaultAsync(di => di.IdAc == accountId);
        }

        public async Task<IEnumerable<DocumentInfo>> GetDocumentInfosByArtworkId(int artworkId)
        {
            return await _context.DocumentInfos.Where(d => d.IdArtwork == artworkId).ToListAsync();
        }

        public async Task<IEnumerable<DocumentInfo>> GetDocumentInfoByEventId(int eventId)
        {
            return await _context.DocumentInfos.Where(di => di.IdEvent == eventId).ToListAsync();
        }

        public async Task<IEnumerable<DocumentInfo>> GetDocumentInfoByProjectId(int projectId)
        {
            return await _context.DocumentInfos.Where(di => di.IdProject == projectId).ToListAsync();
        }
        public async Task<DocumentInfo?> GetDocumentInfoByIdAsNoTracking(int id)
        {
            return await _context.DocumentInfos
                .AsNoTracking()  // Sử dụng AsNoTracking để không theo dõi thực thể
                .FirstOrDefaultAsync(df => df.IdDcIf == id);
        }
    }
}
