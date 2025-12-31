using System.Collections.Generic;
using System.Threading.Tasks;
using ToolCollectionManager.Models;

namespace ToolCollectionManager.Services
{
    public interface ISoftwareService
    {
        Task<List<SoftwareItem>> GetAllSoftwareAsync();
        Task<List<Category>> GetAllCategoriesAsync();
        Task AddCategoryAsync(Category category);
        Task DeleteCategoryAsync(int id);
        Task AddSoftwareAsync(SoftwareItem software);
        Task UpdateSoftwareAsync(SoftwareItem software);
        Task DeleteSoftwareAsync(int id);
        Task<List<SoftwareItem>> SearchSoftwareAsync(string query, int? categoryId);
        Task LaunchSoftwareAsync(string executablePath);
    }
}
