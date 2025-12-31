using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ToolCollectionManager.Models;
using ToolCollectionManager.Services;

namespace ToolCollectionManager.ViewModels
{
    public partial class SoftwareDetailViewModel : ObservableObject
    {
        private readonly ISoftwareService _softwareService;
        private readonly MainViewModel _mainViewModel;

        [ObservableProperty]
        private SoftwareItem software;

        public ObservableCollection<Review> Reviews { get; } = new ObservableCollection<Review>();

        public SoftwareDetailViewModel(SoftwareItem softwareItem, ISoftwareService softwareService, MainViewModel mainViewModel)
        {
            Software = softwareItem;
            _softwareService = softwareService;
            _mainViewModel = mainViewModel;

            if (softwareItem.Reviews != null)
            {
                foreach (var review in softwareItem.Reviews)
                {
                    Reviews.Add(review);
                }
            }
        }

        public SoftwareDetailViewModel() { }

        [RelayCommand]
        private void Edit()
        {
            _mainViewModel.DialogContent = new SoftwareEditViewModel(Software, _softwareService, _mainViewModel);
        }

        [RelayCommand]
        private void Close()
        {
            _mainViewModel.IsDialogOpen = false;
        }
    }
}
