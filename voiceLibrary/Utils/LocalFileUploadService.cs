using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace voiceLibrary.Utils
{
    public class LocalFileUploadStrategy : IFileUploadService
    {
        public string UploadFile(HttpPostedFileBase fileInput, string fileName)
        {
            string path = HttpContext.Current.Server.MapPath("~/Content/Media/");
            var filePath = Path.Combine(path, fileName);
            fileInput.SaveAs(filePath);

            return "Content/Media/" + fileName;
        }
    }
}