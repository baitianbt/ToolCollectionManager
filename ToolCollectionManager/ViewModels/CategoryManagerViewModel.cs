using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using ToolCollectionManager.Models;
using ToolCollectionManager.Services;

namespace ToolCollectionManager.ViewModels
{
    public partial class CategoryManagerViewModel : ObservableObject
    {
        private readonly ISoftwareService _softwareService;
        private readonly MainViewModel _mainViewModel;

        public ObservableCollection<Category> Categories { get; } = new ObservableCollection<Category>();

        [ObservableProperty]
        private string newCategoryName;

        public CategoryManagerViewModel(ISoftwareService softwareService, MainViewModel mainViewModel)
        {
            _softwareService = softwareService;
            _mainViewModel = mainViewModel;
            LoadCategories();
        }

        public CategoryManagerViewModel() { }

        private async void LoadCategories()
        {
            var categories = await _softwareService.GetAllCategoriesAsync();
            Categories.Clear();
            foreach (var cat in categories)
            {
                Categories.Add(cat);
            }
        }

        [RelayCommand]
        private async Task AddCategoryAsync()
        {
            if (string.IsNullOrWhiteSpace(NewCategoryName))
            {
                return;
            }

            var category = new Category { Name = NewCategoryName.Trim(), Color = "#0078D4" }; // Default color
            await _softwareService.AddCategoryAsync(category);
            
            NewCategoryName = string.Empty;
            LoadCategories();
        }

        [RelayCommand]
        private async Task DeleteCategoryAsync(Category category)
        {
            if (category == null) return;

            var result = MessageBox.Show($"Are you sure you want to delete category '{category.Name}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _softwareService.DeleteCategoryAsync(category.Id);
                    LoadCategories();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private async Task CloseAsync()
        {
            _mainViewModel.IsDialogOpen = false;
            // Refresh main view data to reflect changes
            await _mainViewModel.LoadDataCommand.ExecuteAsync(null);
        }
    }
}