using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace voiceLibrary.Utils
{
    public interface IFileUploadService
    {
        string UploadFile(HttpPostedFileBase fileInput, string fileName);
    }
}