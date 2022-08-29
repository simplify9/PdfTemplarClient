using System;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using PdfTemplarSdk.Models;
using Newtonsoft.Json.Serialization;

namespace SW.PdfTemplar.Client

{
    public enum NamingStrategy
    {
        SnakeCase,
        CamelCase,
        CapitalCase
    }
    public class PdfTemplarClientSettings
    {
        public string ServiceUri { get; set; } = "https://pdftemplar.sf9.io/";
        public NamingStrategy NamingStrategy { get; set; } = NamingStrategy.CapitalCase;
    }

    public class PdfTemplar
    {
        private PdfTemplarClientSettings settings;
        private IHttpClientFactory factory;

        public PdfTemplar(PdfTemplarClientSettings settings, IHttpClientFactory factory)
        {
            this.settings = settings;
            this.factory = factory;
        }

        public async Task<FileData> GetDocument(string template, object model)
        {
            Uri uri = new Uri(this.settings.ServiceUri + "/generate");
            var data = new { template = template, dataObj = model };

            Newtonsoft.Json.Serialization.NamingStrategy namingStrategy = new DefaultNamingStrategy();
            switch (this.settings.NamingStrategy)
            {
                case NamingStrategy.SnakeCase:
                    namingStrategy = new SnakeCaseNamingStrategy();
                    break;
                case NamingStrategy.CamelCase:
                    namingStrategy = new CamelCaseNamingStrategy();
                    break;
                default:
                    break;
            }

            var settings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = namingStrategy
                }
            };


            var content = JsonConvert.SerializeObject(data, settings);

            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Post,
                
                Content = new StringContent(content, Encoding.UTF8, "application/json"),
            };

            HttpResponseMessage results = await factory.CreateClient().SendAsync(request);

            if (!results.IsSuccessStatusCode)
            {
                string err = await results.Content.ReadAsStringAsync();
                if(results.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    throw new PdfTemplarException(PdfTemplarError.TemplateInvalid, err, template, content);
                }
                if(results.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    throw new PdfTemplarException(PdfTemplarError.ServerError, err, template, content);
                }
                if(results.StatusCode == System.Net.HttpStatusCode.RequestTimeout)
                {
                    throw new PdfTemplarException(PdfTemplarError.Timeout, err, template, content);
                }
                if(results.StatusCode == System.Net.HttpStatusCode.BadGateway)
                {
                    throw new PdfTemplarException(PdfTemplarError.GatewayError, "Site will reboot momentarily");
                }
            }

            byte[] response = await results.Content.ReadAsByteArrayAsync();

            //PDFTemplarData document = JsonConvert.DeserializeObject<PDFTemplarData>(response);
            return new FileData()
            {
                FileName = "generatedPDF.pdf",
                InlineData = response,
                MimeType = "application/pdf"
            };
        }
        
    }
}
