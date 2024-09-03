using Business;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class CommentDAO : SingletonBase<CommentDAO>
    {
        public async Task<IEnumerable<Comment>> GetCommentAll()
        {
            var comment = await _context.Comments.ToListAsync();
            return comment;
        }
        public async Task<Comment> GetCommentById(int id)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.IdComment == id);
            if (comment == null) return null;

            return comment;
        }
        public async Task Add(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }
        public async Task Update(Comment comment)
        {

            var existingItem = await GetCommentById(comment.IdComment);
            if (existingItem != null)
            {
                // Cập nhật các thuộc tính cần thiết
                _context.Entry(existingItem).CurrentValues.SetValues(comment);
                await _context.SaveChangesAsync();
            }

        }
        public async Task Delete(int id)
        {
            var comment = await GetCommentById(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }
    }
}
