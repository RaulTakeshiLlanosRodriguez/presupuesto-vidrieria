using CloudinaryDotNet.Actions;
using CloudinaryDotNet;

namespace ApiPresupuesto.Service
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            var account = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]
            );

            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadPdfAsync(byte[] pdfData, string fileName)
        {
            using var stream = new MemoryStream(pdfData);

            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(fileName, stream),
                Folder = "pdfs",
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.ToString();
        }
    }
}
