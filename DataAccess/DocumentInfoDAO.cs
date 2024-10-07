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
            var documentInfo = await GetDocumentInfoById(id);
            if (documentInfo != null)
            {
                _context.DocumentInfos.Remove(documentInfo);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<DocumentInfo>> GetDocumentInfoAll()
        {
            return await _context.DocumentInfos.ToListAsync();
        }

        public async Task<DocumentInfo?> GetDocumentInfoById(int id)
        {
            return await _context.DocumentInfos.FirstOrDefaultAsync(df => df.IdDcIf == id);
        }

        public async Task Update(DocumentInfo documentInfo)
        {
            var existingItem = await GetDocumentInfoById(documentInfo.IdDcIf);
            if (existingItem != null)
            {
                _context.Entry(existingItem).CurrentValues.SetValues(documentInfo);
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
    }
}
