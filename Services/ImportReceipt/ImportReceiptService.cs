using WarehouseAPI.DTOs.ImportReceiptDTOs;
using WarehouseAPI.Repository;

namespace WarehouseAPI.Services.ImportReceipt
{
    public class ImportReceiptService : IImportReceiptService
    {
        private IUOW UOW;

        public ImportReceiptService(IUOW uow)
        {
            UOW = uow;
        }

        public async Task<ImportReceiptResponse> AddImportReceiptAsync(CreateImportReceiptRequest importReceipt)
        {
            return await UOW.ImportReceiptRepository.AddImportReceiptAsync(importReceipt);
        }

        public async Task<bool> DeleteImportReceiptByIdAsync(int id)
        {
            return await UOW.ImportReceiptRepository.DeleteImportReceiptByIdAsync(id);
        }

        public async Task<List<ImportReceiptResponse>> GetAllImportReceiptAsync()
        {
            return await UOW.ImportReceiptRepository.GetAllImportReceiptAsync();
        }

        public async Task<ImportReceiptResponse?> GetImportReceiptByIdAsync(int id)
        {
            return await UOW.ImportReceiptRepository.GetImportReceiptByIdAsync(id);
        }

        public async Task<bool> UpdateImportReceiptByIdAsync(int id, UpdateImportReceiptRequest importReceipt)
        {
            return await UOW.ImportReceiptRepository.UpdateImportReceiptByIdAsync(id, importReceipt);
        }
    }
}
