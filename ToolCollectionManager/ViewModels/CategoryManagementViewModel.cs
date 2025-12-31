using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using System.Windows;
using ToolCollectionManager.Models;
using ToolCollectionManager.Services;

namespace ToolCollectionManager.ViewModels
{
    public partial class CategoryManagementViewModel : ObservableObject
    {
        private readonly ISoftwareService _softwareService;
        private readonly MainViewModel _mainViewModel;

        [ObservableProperty]
        private string newCategoryName = string.Empty;

        [ObservableProperty]
        private string newCategoryColor = "#0078D4"; // Default color

        public CategoryManagementViewModel(ISoftwareService softwareService, MainViewModel mainViewModel)
        {
            _softwareService = softwareService;
            _mainViewModel = mainViewModel;
        }

        public System.Collections.ObjectModel.ObservableCollection<Category> Categories => _mainViewModel.Categories;

        [RelayCommand]
        private async Task AddCategoryAsync()
        {
            if (string.IsNullOrWhiteSpace(NewCategoryName))
            {
                MessageBox.Show("Category name cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var category = new Category { Name = NewCategoryName.Trim(), Color = NewCategoryColor };
                await _softwareService.AddCategoryAsync(category);

                NewCategoryName = string.Empty;
                NewCategoryColor = "#0078D4"; // Reset to default

                MessageBox.Show("Category added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Refresh categories in main view
                await _mainViewModel.LoadDataCommand.ExecuteAsync(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to add category: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task DeleteCategoryAsync(Category category)
        {
            if (category == null) return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete the category '{category.Name}'?\n\nThis action cannot be undone and will fail if any software is assigned to this category.",
                "Confirm Deletion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _softwareService.DeleteCategoryAsync(category.Id);
                    MessageBox.Show("Category deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Refresh categories in main view
                    await _mainViewModel.LoadDataCommand.ExecuteAsync(null);
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message, "Cannot Delete", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to delete category: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}