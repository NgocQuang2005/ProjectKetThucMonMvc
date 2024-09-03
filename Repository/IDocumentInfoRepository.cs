using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IDocumentInfoRepository
    {
        Task<IEnumerable<DocumentInfo>> GetDocumentInfoAll();
        Task<DocumentInfo> GetDocumentInfoById(int id);
        Task Add(DocumentInfo documentInfo);
        Task Update(DocumentInfo documentInfo);
        Task Delete(int id);
        Task<bool> ChangeActive(int id);

    }
}
