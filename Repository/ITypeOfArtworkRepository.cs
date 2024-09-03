using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface ITypeOfArtworkRepository
    {
        Task<IEnumerable<TypeOfArtwork>> GetTypeOfArtworkAll();
        Task<TypeOfArtwork> GetTypeOfArtworkById(int id);
        Task Add(TypeOfArtwork typeOfArtwork);
        Task Update(TypeOfArtwork typeOfArtwork);
        Task Delete(int id);
        Task<bool> ChangeActive(int id);

    }
}
