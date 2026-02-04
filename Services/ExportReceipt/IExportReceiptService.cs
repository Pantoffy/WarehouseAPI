using WarehouseAPI.DTOs.ExportReceiptDTOs;

namespace WarehouseAPI.Services.ExportReceipt
{
    public interface IExportReceiptService
    {
        Task<List<ExportReceiptResponse>> GetAllExportReceiptAsync();
        Task<ExportReceiptResponse?> GetExportReceiptByIdAsync(int id);
        Task<ExportReceiptResponse> AddExportReceiptAsync(CreateExportReceiptRequest exportReceipt);
        Task<bool> UpdateExportReceiptByIdAsync(int id, UpdateExportReceiptRequest exportReceipt);
        Task<bool> DeleteExportReceiptByIdAsync(int id);
    }
}
