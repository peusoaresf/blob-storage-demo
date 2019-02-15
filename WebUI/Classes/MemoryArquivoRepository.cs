using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace WebUI.Classes
{
    public class MemoryArquivoRepository : IArquivoRepository
    {
        private static long _identity = 1;

        private static Arquivo _root = new Arquivo("GED_local", true, null)
        {
            DataCriacao = new DateTime(2019, 2, 7),
            IdArquivo = _identity
        };

        private static Dictionary<long, Arquivo> _arquivos = new Dictionary<long, Arquivo>()
        {
            {
                _root.IdArquivo,
                _root
            }
        };

        public async Task AddAsync(Arquivo arquivo)
        {
            await SleepAsync();

            long id = ++_identity;
            var now = DateTime.Now;            

            arquivo.IdArquivo = id;
            arquivo.DataCriacao = now;

            _arquivos.Add(id, arquivo);
        }

        public async Task UpdateAsync(Arquivo arquivo)
        {
            await SleepAsync();

            _arquivos[arquivo.IdArquivo] = arquivo;
        }

        public async Task DeleteAsync(long id)
        {
            await SleepAsync();

            _arquivos.Remove(id);
        }

        public async Task<IEnumerable<Arquivo>> FindAllAsync()
        {
            await SleepAsync();

            return _arquivos.Values.OrderBy(x => !x.IsDiretorio);
        }

        public async Task<Arquivo> FindByIdAsync(long id)
        {
            await SleepAsync();

            return _arquivos[id];
        }

        public async Task<Arquivo> FindWhereParentIsNullAsync()
        {
            IEnumerable<Arquivo> arquivos = await FindAllAsync();

            foreach (var arquivo in arquivos)
            {
                if (arquivo.Parent == null)
                {
                    return arquivo;
                }
            }

            return null;
        }

        public async Task<IEnumerable<Arquivo>> FindWhereParentEqualsAsync(long id)
        {
            IEnumerable<Arquivo> arquivos = await FindAllAsync();

            var result = new List<Arquivo>();

            foreach (var arquivo in arquivos)
            {
                if (arquivo.Parent != null && arquivo.Parent.IdArquivo == id)
                {
                    result.Add(arquivo);
                }
            }

            return result.OrderBy(x => !x.IsDiretorio);
        }

        private async Task SleepAsync()
        {
            await Task.Run(() =>
            {
                Thread.Sleep(5);
            });
        }
    }
}