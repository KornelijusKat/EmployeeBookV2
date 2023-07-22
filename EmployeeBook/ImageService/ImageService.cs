using System.Drawing;

namespace EmployeeBook.ImageService
{
    public class ImageService :IImageService
    {
        public Image ImageResize(IFormFile imageRequest)
        {
            if(imageRequest == null)
            {
                return null;
            }
            using var memoryStream = new MemoryStream();
            imageRequest.CopyTo(memoryStream);
            using (Image image = Image.FromStream(memoryStream, true, true))
            {
                Size newSize = new Size(200, 200);
                Image neImage = new Bitmap(newSize.Width, newSize.Height);

                using (Graphics gr = Graphics.FromImage((Bitmap)neImage))
                {
                    gr.DrawImage(image, new Rectangle(Point.Empty, newSize));
                }
                return neImage;
            }
        }
        public byte[] ImageToByteArray(Image image)
        {
            ImageConverter converter = new ImageConverter();
            byte[] MyImageArray = (byte[])converter.ConvertTo(image, typeof(byte[]));
            return MyImageArray;
        }
        public byte[] GetByteArray(IFormFile imageRequest)
        {
            var redrawn = ImageResize(imageRequest);
            return ImageToByteArray(redrawn);
        }
        public Image ByteArrayToImage(byte[] imageBytes)
        {
            using (Image image = Image.FromStream(new MemoryStream(imageBytes)))
            {
                return image;
            }
        }
        public IFormFile ConvertToIFormFile(byte[] imageBytes, string fileName)
        {
            if (imageBytes == null || fileName == null)
            {
                return null;
            }
            using (var memoryStream = new MemoryStream(imageBytes))
            {
                var formFile = new FormFile(memoryStream, 0, memoryStream.Length, null, fileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/jpeg" 
                };

                return formFile;
            }
        }
    }
}
