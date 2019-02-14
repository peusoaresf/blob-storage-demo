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
    }
}