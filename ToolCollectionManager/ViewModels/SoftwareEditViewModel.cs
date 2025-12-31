using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using ToolCollectionManager.Models;
using ToolCollectionManager.Services;

namespace ToolCollectionManager.ViewModels
{
    public partial class SoftwareEditViewModel : ObservableObject
    {
        private readonly ISoftwareService _softwareService;
        private readonly MainViewModel _mainViewModel;

        [ObservableProperty]
        private SoftwareItem software;

        [ObservableProperty]
        private string title;

        public ObservableCollection<Category> Categories { get; } = new ObservableCollection<Category>();

        [ObservableProperty]
        private Category selectedCategory;

        public SoftwareEditViewModel(SoftwareItem software, ISoftwareService softwareService, MainViewModel mainViewModel)
        {
            _softwareService = softwareService;
            _mainViewModel = mainViewModel;
            Software = software;
            Title = software.Id == 0 ? "Add Software" : "Edit Software";
            
            LoadCategories();
        }

        public SoftwareEditViewModel() { }

        private async void LoadCategories()
        {
            var categories = await _softwareService.GetAllCategoriesAsync();
            Categories.Clear();
            foreach (var cat in categories)
            {
                Categories.Add(cat);
                if (Software.CategoryId == cat.Id)
                {
                    SelectedCategory = cat;
                }
            }
        }

        [RelayCommand]
        private void SelectExecutable()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*",
                Title = "Select Executable"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                Software.ExecutablePath = openFileDialog.FileName;
                
                // Auto-fill name if empty
                if (string.IsNullOrWhiteSpace(Software.Name))
                {
                    Software.Name = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                }

                // Extract Icon
                try
                {
                    ExtractAndSaveIcon(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    // Fail silently or log
                    System.Diagnostics.Debug.WriteLine($"Failed to extract icon: {ex.Message}");
                }
                
                // Notify changes
                OnPropertyChanged(nameof(Software));
            }
        }

        private void ExtractAndSaveIcon(string exePath)
        {
            if (!File.Exists(exePath)) return;

            // Extract associated icon
            using (var icon = Icon.ExtractAssociatedIcon(exePath))
            {
                if (icon != null)
                {
                    // Create icons directory in AppData
                    string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    string iconsFolder = Path.Combine(appData, "ToolCollectionManager", "Icons");
                    if (!Directory.Exists(iconsFolder))
                    {
                        Directory.CreateDirectory(iconsFolder);
                    }

                    // Generate unique filename
                    string iconFileName = $"{Guid.NewGuid()}.png";
                    string iconPath = Path.Combine(iconsFolder, iconFileName);

                    // Convert to bitmap and save as PNG
                    using (var bitmap = icon.ToBitmap())
                    {
                        bitmap.Save(iconPath, System.Drawing.Imaging.ImageFormat.Png);
                    }

                    // Update software model
                    Software.IconPath = iconPath;
                }
            }
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(Software.Name) || string.IsNullOrWhiteSpace(Software.ExecutablePath))
            {
                MessageBox.Show("Name and Path are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedCategory != null)
            {
                Software.CategoryId = SelectedCategory.Id;
            }
            else if (Software.CategoryId == 0 && Categories.Count > 0)
            {
                 // Default to first category if none selected
                 Software.CategoryId = Categories[0].Id;
            }
            
            // Ensure Category is not null to prevent FK constraint failure
            if (Software.Category == null && Software.CategoryId > 0)
            {
                 // We don't need to set the navigation property for the update/insert, just the ID
                 // But EF Core might need it to be null to avoid tracking issues if we attached it incorrectly
                 Software.Category = null; 
            }

            if (Software.Id == 0)
            {
                await _softwareService.AddSoftwareAsync(Software);
            }
            else
            {
                await _softwareService.UpdateSoftwareAsync(Software);
            }

            _mainViewModel.IsDialogOpen = false;
            await _mainViewModel.LoadDataCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        private void Cancel()
        {
            _mainViewModel.IsDialogOpen = false;
        }
    }
}