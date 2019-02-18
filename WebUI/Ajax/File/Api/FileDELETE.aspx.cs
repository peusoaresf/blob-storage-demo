using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebUI.Classes;

namespace WebUI.Ajax.File.Api
{
    public partial class FileDELETE : System.Web.UI.Page
    {
        protected async void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();

            long idArquivo = Convert.ToInt64(Request.Params["id"]);

            try
            {
                await ArquivoRules.DeleteAsync(idArquivo);

                Response.Write(new JavaScriptSerializer().Serialize(new
                {
                    result = "sucesso"
                }));
            } 
            catch (ArgumentException argumentException)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.Write(new JavaScriptSerializer().Serialize(new
                {
                    error = new { message = argumentException.Message }
                }));
            }
           
            Response.End();
        }
    }
}