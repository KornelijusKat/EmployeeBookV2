namespace EmployeeBook.ImageService
{
    public interface IImageService
    {
        IFormFile ConvertToIFormFile(byte[] imageBytes, string fileName);
        byte[] GetByteArray(IFormFile imageRequest);
    }
}
