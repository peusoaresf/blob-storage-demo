using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Classes
{
    public class Arquivo
    {
        public bool IsDiretorio { get; set; }
        public long IdArquivo { get; set; }
        public long Tamanho { get; set; }
        public string Nome { get; set; }
        public string Url { get; set; }
        public Arquivo Parent { get; set; }
        public DateTime DataCriacao { get; set; }

        public Arquivo(string nome, bool isDiretorio, Arquivo parent)
        {
            this.Nome = nome;
            this.IsDiretorio = isDiretorio;
            this.Parent = parent;
            this.Url = parent != null
                            ? $"{parent.Url}{nome}" + (isDiretorio ? "/" : String.Empty)
                            : String.Empty;
        }
    }
}