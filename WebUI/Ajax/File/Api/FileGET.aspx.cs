using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebUI.Classes;

namespace WebUI.Ajax.File.Api
{
    public partial class FileGET : System.Web.UI.Page
    {
        protected async void Page_Load(object sender, EventArgs e)
        {
            long? idParent = 
                !String.IsNullOrWhiteSpace(Request.Params["fkParent"]) 
                    ? Convert.ToInt64(Request.Params["fkParent"]) 
                    : (long?) null;

            IArquivoRepository arquivoRepository = ArquivoRepositoryFactory.Create();

            IEnumerable<Arquivo> arquivos = await arquivoRepository.FindWhereParentEqualsAsync(idParent);

            Response.Clear();
            Response.Write(new JavaScriptSerializer().Serialize(new
            {
                result = arquivos
            }));
            Response.End();
        }
    }
}