using Business;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DocumentInfoDAO : SingletonBase<DocumentInfoDAO>
    {
        public async Task<IEnumerable<DocumentInfo>> GetDocumentInfoAll()
        {
            var documentInfo = await _context.DocumentInfos.ToListAsync();
            return documentInfo;
        }
        public async Task<DocumentInfo> GetDocumentInfoById(int id)
        {
            var documentInfo = await _context.DocumentInfos.FirstOrDefaultAsync(df => df.IdDcIf == id);
            if (documentInfo == null) return null;

            return documentInfo;
        }
        public async Task Add(DocumentInfo documentInfo)
        {
            _context.DocumentInfos.Add(documentInfo);
            await _context.SaveChangesAsync();
        }
        public async Task Update(DocumentInfo documentInfo)
        {

            var existingItem = await GetDocumentInfoById(documentInfo.IdDcIf);
            if (existingItem != null)
            {
                // Cập nhật các thuộc tính cần thiết
                _context.Entry(existingItem).CurrentValues.SetValues(documentInfo);
                await _context.SaveChangesAsync();
            }

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
        public async Task<bool> ChangeActive(int id)
        {
            var documentInfo = await GetDocumentInfoById(id);
            documentInfo.Active = !documentInfo.Active;
            await _context.SaveChangesAsync();
            return documentInfo.Active;
        }
    }
}
