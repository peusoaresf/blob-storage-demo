using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebUI.Classes;

namespace WebUI.Ajax.File
{
    public partial class FileDownload : System.Web.UI.Page
    {
        protected async void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Response.Clear();
                
                string json = await HandleFileDownload();

                Response.Write(json);
                Response.End();
            }
        }

        private async Task<string> HandleFileDownload()
        {
            Arquivo arquivo = PegarArquivoRequest();

            var fileManager = new FileManager();

            return await fileManager.Download(arquivo);
        }

        private Arquivo PegarArquivoRequest()
        {
            long idArquivo = Convert.ToInt64(Request.Params["idArquivo"]);

            return ArquivoRepository.FindById(idArquivo);
        }        
    }
}