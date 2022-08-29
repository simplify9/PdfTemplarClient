using System;
using System.Collections.Generic;
using System.Text;

namespace SW.PdfTemplar.Client
{
    public enum PdfTemplarError
    {
        TemplateInvalid = 1, 
        ServerError = 2,
        Timeout = 3,
        GatewayError = 4,
    }
}
