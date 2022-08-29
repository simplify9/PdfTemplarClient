using System;
using System.Collections.Generic;
using System.Text;

namespace SW.PdfTemplar.Client
{
    public class PdfTemplarException: Exception
    {
        public PdfTemplarError Error { get; }
        public string ErrorMessage { get; }

        public PdfTemplarException(PdfTemplarError error, 
            string message, string template = "", string data = ""): 
            base($"PDF Template Error Code: {error}\nError: {message}\nTemplate: {template} \nData: {data}  ")
        {
            Error = error;
            ErrorMessage = message;
        }
    }
}
