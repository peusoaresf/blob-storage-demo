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
    }
}