using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Commons.Model.ServiceCustomHttpClient
{
    public class FormFileUpload
    {
        public string? FileName { get; set; }
        public byte[]? DataByte { get; set; }


        public FormFileUpload(string? FileName)
        {
            this.FileName = FileName;
        }

    }


    public class FileUploadRespone
    {
        public string? FileName { get; set; }
        public byte[]? DataByte { get; set; }


        public FileUploadRespone(string? FileName)
        {
            this.FileName = FileName;
        }
        public FileUploadRespone()
        {
        }

    }
}
