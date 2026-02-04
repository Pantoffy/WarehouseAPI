using WarehouseAPI.DTOs.ExportReceiptDTOs;
using WarehouseAPI.Repository;

namespace WarehouseAPI.Services.ExportReceipt
{
    public class ExportReceiptService : IExportReceiptService
    {
        private IUOW UOW;

        public ExportReceiptService(IUOW uow)
        {
            UOW = uow;
        }

        public async Task<ExportReceiptResponse> AddExportReceiptAsync(CreateExportReceiptRequest exportReceipt)
        {
            return await UOW.ExportReceiptRepository.AddExportReceiptAsync(exportReceipt);
        }

        public async Task<bool> DeleteExportReceiptByIdAsync(int id)
        {
            return await UOW.ExportReceiptRepository.DeleteExportReceiptByIdAsync(id);
        }

        public async Task<List<ExportReceiptResponse>> GetAllExportReceiptAsync()
        {
            return await UOW.ExportReceiptRepository.GetAllExportReceiptAsync();
        }

        public async Task<ExportReceiptResponse?> GetExportReceiptByIdAsync(int id)
        {
            return await UOW.ExportReceiptRepository.GetExportReceiptByIdAsync(id);
        }

        public async Task<bool> UpdateExportReceiptByIdAsync(int id, UpdateExportReceiptRequest exportReceipt)
        {
            return await UOW.ExportReceiptRepository.UpdateExportReceiptByIdAsync(id, exportReceipt);
        }
    }
}
