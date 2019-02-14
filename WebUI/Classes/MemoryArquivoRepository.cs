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

        private static Arquivo root = new Arquivo()
        {
            IdArquivo = _identity,
            Nome = "GED_local",
            Url = "",
            DataCriacao = new DateTime(2019, 2, 7),
            IsDiretorio = true,
            Parent = null
        };

        private static Dictionary<long, Arquivo> _arquivos = new Dictionary<long, Arquivo>()
        {
            {
                root.IdArquivo,
                root
            }/*,
            {
                2,
                new Arquivo()
                {
                    IdArquivo = 2,
                    Nome = "antigo painel de conexão.txt",
                    Url = "/antigo painel de conexão.txt",
                    DataCriacao = new DateTime(2019, 2, 8),
                    IsDiretorio = false,
                    Parent = root
                }
            }*/
        };

        public async Task Add(Arquivo arquivo)
        {
            await Sleep();

            long id = ++_identity;
            var now = DateTime.Now;            

            arquivo.IdArquivo = id;
            arquivo.DataCriacao = now;

            _arquivos.Add(id, arquivo);
        }

        public async Task Update(Arquivo arquivo)
        {
            await Sleep();

            _arquivos[arquivo.IdArquivo] = arquivo;
        }

        public async Task Delete(long id)
        {
            await Sleep();

            _arquivos.Remove(id);
        }

        public async Task<IEnumerable<Arquivo>> FindAll()
        {
            await Sleep();

            return _arquivos.Values.OrderBy(x => !x.IsDiretorio);
        }

        public async Task<Arquivo> FindById(long id)
        {
            await Sleep();

            return _arquivos[id];
        }

        public async Task<Arquivo> FindWhereParentIsNull()
        {
            IEnumerable<Arquivo> arquivos = await FindAll();

            foreach (var arquivo in arquivos)
            {
                if (arquivo.Parent == null)
                {
                    return arquivo;
                }
            }

            return null;
        }

        public async Task<IEnumerable<Arquivo>> FindWhereParentEquals(long id)
        {
            IEnumerable<Arquivo> arquivos = await FindAll();

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

        private async Task Sleep()
        {
            await Task.Run(() =>
            {
                Thread.Sleep(10);
            });
        }
    }
}