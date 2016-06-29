﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Threading;
using System.Text;

namespace Gsoft.SilverlightFileUpload
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    //[WebService(Namespace = "http://tempuri.org/")]
    //[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class FileUpload : IHttpHandler
    {
        private HttpContext ctx;
        public void ProcessRequest(HttpContext context)
        {
            ctx = context;
            string uploadPath = context.Server.MapPath("~/" + context.Request.QueryString["FolderUpload"]);
            FileUploadProcess fileUpload = new FileUploadProcess();
            fileUpload.FileUploadCompleted += new FileUploadCompletedEvent(fileUpload_FileUploadCompleted);
            fileUpload.ProcessRequest(context, uploadPath);
        }

        void fileUpload_FileUploadCompleted(object sender, FileUploadCompletedEventArgs args)
        {
            string id = ctx.Request.QueryString["id"];
            //FileInfo fi = new FileInfo(args.FilePath);
            //string targetFile = Path.Combine(fi.Directory.FullName, args.FileName);
            //if (File.Exists(targetFile))
            //    File.Delete(targetFile);
            //fi.MoveTo(targetFile);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
