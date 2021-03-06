﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebUI.Classes;

namespace WebUI.Ajax.File
{
    public partial class FileChunkUpload : System.Web.UI.Page
    {
        protected async void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();

            string token = Request.Params["token"];
            HttpPostedFile httpPostedFile = Request.Files["chunk"];

            IFileManager fileManager = FileManagerFactory.Create();

            long fileSize = await fileManager.UploadChunkAsync(httpPostedFile.InputStream, token);

            Response.Write(fileSize);
            Response.End();
        }
    }
}