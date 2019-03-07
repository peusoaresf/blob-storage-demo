﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Classes
{
    public class FileManagerFactory
    {
        public static IFileManager Create()
        {
            return new AzureBlobStorageFileManager();
        }
    }
}