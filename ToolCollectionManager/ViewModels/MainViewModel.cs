using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using ToolCollectionManager.Models;
using ToolCollectionManager.Services;
using ToolCollectionManager.Views;

namespace ToolCollectionManager.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ISoftwareService _softwareService;

        [ObservableProperty]
        private string title = "软件工具箱";

        [ObservableProperty]
        private string searchText;

        [ObservableProperty]
        private Category selectedCategory;
        
        [ObservableProperty]
        private object dialogContent;

        [ObservableProperty]
        private bool isDialogOpen;

        public ObservableCollection<SoftwareItem> SoftwareItems { get; } = new ObservableCollection<SoftwareItem>();
        public ObservableCollection<Category> Categories { get; } = new ObservableCollection<Category>();

        public MainViewModel(ISoftwareService softwareService)
        {
            _softwareService = softwareService;
            LoadDataCommand.ExecuteAsync(null);
        }

        // Default constructor for design time data
        public MainViewModel() { }

        [RelayCommand]
        private async Task LoadDataAsync()
        {
            var categories = await _softwareService.GetAllCategoriesAsync();
            Categories.Clear();
            foreach (var cat in categories)
            {
                Categories.Add(cat);
            }

            await SearchAsync();
        }

        [RelayCommand]
        private async Task SearchAsync()
        {
            var items = await _softwareService.SearchSoftwareAsync(SearchText, SelectedCategory?.Id);
            SoftwareItems.Clear();
            foreach (var item in items)
            {
                SoftwareItems.Add(item);
            }
        }

        [ObservableProperty]
        private bool isLaunching;

        [RelayCommand]
        private async Task LaunchSoftwareAsync(SoftwareItem item)
        {
            if (IsLaunching) return;

            if (item != null && !string.IsNullOrEmpty(item.ExecutablePath))
            {
                try
                {
                    IsLaunching = true;
                    // Show launching status (optional: could add a snackbar message here)
                    DialogContent = new object(); // Just to trigger dialog open with a loading spinner or similar if we had a view
                    // But for now, we just rely on the flag preventing double clicks. 
                    // Actually, let's show a simple "Launching..." dialog or just rely on cursor/UI state if we had it.
                    // For simplicity, we just block input via flag.
                    
                    // Small delay to prevent double clicks and give visual feedback
                    await Task.Delay(500); 

                    await _softwareService.LaunchSoftwareAsync(item.ExecutablePath);
                }
                catch
                {
                    MessageBox.Show($"Failed to launch {item.Name}. Path: {item.ExecutablePath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsLaunching = false;
                }
            }
        }

        [RelayCommand]
        private void ViewDetails(SoftwareItem item)
        {
            if (item != null)
            {
                DialogContent = new SoftwareDetailViewModel(item, _softwareService, this);
                IsDialogOpen = true;
            }
        }

        [RelayCommand]
        private void OpenSettings()
        {
            DialogContent = new SettingsViewModel(_softwareService, this);
            IsDialogOpen = true;
        }

        partial void OnSearchTextChanged(string value)
        {
            SearchCommand.ExecuteAsync(null);
        }

        partial void OnSelectedCategoryChanged(Category value)
        {
            SearchCommand.ExecuteAsync(null);
        }
    }
}