using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using ToolCollectionManager.Models;
using ToolCollectionManager.Services;

namespace ToolCollectionManager.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly ISoftwareService _softwareService;
        private readonly MainViewModel _mainViewModel;

        // Sub-ViewModels for tabs
        public CategoryManagementViewModel CategoryManagement { get; }
        public SoftwareEditViewModel AddSoftware { get; }

        public SettingsViewModel(ISoftwareService softwareService, MainViewModel mainViewModel)
        {
            _softwareService = softwareService;
            _mainViewModel = mainViewModel;

            // Initialize sub-viewmodels
            CategoryManagement = new CategoryManagementViewModel(softwareService, mainViewModel);
            AddSoftware = new SoftwareEditViewModel(new SoftwareItem(), softwareService, mainViewModel);

            // Initialize startup status directly to backing field to avoid triggering OnChanged
            isStartupEnabled = CheckStartup();
        }

        // Startup Logic
        [ObservableProperty]
        private bool isStartupEnabled;

        partial void OnIsStartupEnabledChanged(bool value)
        {
            SetStartup(value);
        }

        private bool CheckStartup()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    if (key == null) return false;
                    return key.GetValue("ToolCollectionManager") != null;
                }
            }
            catch
            {
                return false;
            }
        }

        private void SetStartup(bool enable)
        {
            try
            {
                string appName = "ToolCollectionManager";
                string appPath = Process.GetCurrentProcess().MainModule.FileName;

                // Handle .dll vs .exe in .NET Core (usually FileName is the .exe)
                if (appPath.EndsWith(".dll"))
                {
                    appPath = appPath.Replace(".dll", ".exe");
                }

                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    if (key == null) return;

                    if (enable)
                    {
                        key.SetValue(appName, $"\"{appPath}\"");
                    }
                    else
                    {
                        key.DeleteValue(appName, false);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update startup settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                // Revert toggle if failed
                IsStartupEnabled = !enable;
            }
        }
    }
}