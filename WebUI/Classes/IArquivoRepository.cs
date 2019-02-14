using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebUI.Classes
{
    public interface IArquivoRepository
    {
        Task Add(Arquivo arquivo);
        Task Delete(long id);
        Task<IEnumerable<Arquivo>> FindAll();
        Task<Arquivo> FindById(long id);
        Task<IEnumerable<Arquivo>> FindWhereParentEquals(long id);
        Task<Arquivo> FindWhereParentIsNull();
        Task Update(Arquivo arquivo);
    }
}