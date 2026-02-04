using WarehouseAPI.DTOs.ImportReceiptDTOs;

namespace WarehouseAPI.Services.ImportReceipt
{
    public interface IImportReceiptService
    {
        Task<List<ImportReceiptResponse>> GetAllImportReceiptAsync();
        Task<ImportReceiptResponse?> GetImportReceiptByIdAsync(int id);
        Task<ImportReceiptResponse> AddImportReceiptAsync(CreateImportReceiptRequest importReceipt);
        Task<bool> UpdateImportReceiptByIdAsync(int id, UpdateImportReceiptRequest importReceipt);
        Task<bool> DeleteImportReceiptByIdAsync(int id);
    }
}
