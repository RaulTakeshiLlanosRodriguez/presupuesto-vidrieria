using DinkToPdf.Contracts;
using DinkToPdf;

namespace ApiPresupuesto.Service
{
    public class PdfGeneratorService
    {
        private readonly IConverter _converter;

        public PdfGeneratorService(IConverter converter)
        {
            _converter = converter;
        }

        public byte[] GeneratePdf(string htmlContent)
        {
            var pdfDocument = new HtmlToPdfDocument
            {
                GlobalSettings = new GlobalSettings
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait,
                    Margins = new MarginSettings
                    {
                        Top = 10,
                        Bottom = 10,
                        Left = 10,
                        Right = 10
                    }
                }
            };

            // Agrega el objeto a la lista existente en lugar de asignar una nueva lista
            pdfDocument.Objects.Add(new ObjectSettings
            {
                HtmlContent = htmlContent,
                WebSettings = { DefaultEncoding = "utf-8" }
            });

            return _converter.Convert(pdfDocument);
        }

    }
}
