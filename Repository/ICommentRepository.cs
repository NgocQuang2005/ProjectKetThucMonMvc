using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetCommentAll();
        Task<Comment> GetCommentById(int id);
        Task Add(Comment comment);
        Task Update(Comment comment);
        Task Delete(int id);
    }
}
