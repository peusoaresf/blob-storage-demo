using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Classes
{
    public class ArquivoRepository
    {
        private static Arquivo root = new Arquivo()
        {
            IdArquivo = 1,
            Nome = "GED_local",
            Url = "/",
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

        public static void Add(Arquivo arquivo)
        {
            var now = DateTime.Now;
            long id = now.Ticks;

            arquivo.IdArquivo = id;
            arquivo.DataCriacao = now;

            _arquivos.Add(id, arquivo);
        }

        public static void Delete(long id)
        {
            _arquivos.Remove(id);
        }

        public static IEnumerable<Arquivo> FindAll()
        {
            return _arquivos.Values.OrderBy(x => !x.IsDiretorio);
        }

        public static Arquivo FindById(long id)
        {
            return _arquivos[id];
        }

        public static Arquivo FindWhereParentIsNull()
        {
            foreach (var arquivo in FindAll())
            {
                if (arquivo.Parent == null)
                {
                    return arquivo;
                }
            }

            return null;
        }

        public static IEnumerable<Arquivo> FindWhereParentEquals(long id)
        {
            var result = new List<Arquivo>();

            foreach (var arquivo in FindAll())
            {
                if (arquivo.Parent != null && arquivo.Parent.IdArquivo == id)
                {
                    result.Add(arquivo);
                }
            }

            return result.OrderBy(x => !x.IsDiretorio);
        }        
    }
}