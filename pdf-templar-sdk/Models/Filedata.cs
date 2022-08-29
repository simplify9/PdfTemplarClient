using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTemplarSdk.Models
{
    public class FileData
    {
        public string MimeType { get; set; }
        public byte[] InlineData { get; set; }
        public string FileName { get; set; }
    }
}
