using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Classes
{
    public class ArquivoFactory
    {
        public static Arquivo Create()
        {
            return new Arquivo();
        }

        public static Arquivo Create(string nome, bool isDiretorio, Arquivo parent)
        {
            return new Arquivo(nome, isDiretorio, parent);
        }
    }
}