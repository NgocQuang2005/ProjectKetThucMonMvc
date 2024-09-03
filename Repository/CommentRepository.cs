using Business;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CommentRepository : ICommentRepository
    {
        public async Task Add(Comment comment)
        {
            await CommentDAO.Instance.Add(comment);
        }

        public async Task Delete(int id)
        {
            await CommentDAO.Instance.Delete(id);
        }

        public async Task<IEnumerable<Comment>> GetCommentAll()
        {
            return await CommentDAO.Instance.GetCommentAll();
        }

        public async Task<Comment> GetCommentById(int id)
        {
            return await CommentDAO.Instance.GetCommentById(id);
        }
        public async Task Update(Comment comment)
        {
            await CommentDAO.Instance.Update(comment);
        }
    }
}
