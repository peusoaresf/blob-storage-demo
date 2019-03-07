using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Classes
{
    public class ArquivoRepositoryFactory
    {
        public static IArquivoRepository Create()
        {
            return new SqlServerArquivoRepository();
        }
    }
}