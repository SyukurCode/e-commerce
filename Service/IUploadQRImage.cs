namespace E_Commers_Adelia.Service
{
    public interface IUploadQRImage
    {
        Task<bool> isQRExist(string userId);
        Task<bool> UploadQrCodeAsync(IFormFile QRFile, string userId);
    }
}
