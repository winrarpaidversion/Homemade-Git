using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GalaSoft.MvvmLight.Views;
using HomemadeGit.Core.DTOs.Repositories;
using HomemadeGit.Desktop.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace HomemadeGit.Desktop.ViewModel
{
    public partial class GitViewModel : ViewModelBase
    {
        private int UserId;
        private string _repositoryRoot = string.Empty;
        private IDialogService _dialogService;
        private readonly RepositoryClientService _repositoryClientService;

        public int SelectedRepositoryId => SelectedRepository?.Id ?? 0;

    
        public record RepositoryListItem(int Id, string Name, string? Description, bool IsPublic);
        public record FileSystemItem(string Name, string FullPath, bool IsDirectory);
        public record TextLine(string Number, string Text);



        [ObservableProperty]
        private string _searchQuery = string.Empty;

        [ObservableProperty]
        private string _title = string.Empty;

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private string _path = string.Empty;

        [ObservableProperty]
        private FileSystemItem? _selectedItem;

        [ObservableProperty]
        private RepositoryListItem? _selectedRepository;

        [ObservableProperty]
        private RepositoryResponse? _currentRepository;

        [ObservableProperty]
        private bool _isPublic = true;  

        [ObservableProperty]
        private string _statusMessage = "Готов";  

        [ObservableProperty]
        private ObservableCollection<RepositoryListItem> _repositories = new(); 

        [ObservableProperty]
        private ObservableCollection<FileSystemItem> _pathToFile = new();  

        [ObservableProperty]
        private ObservableCollection<TextLine> _codeLines = new(); 

        public GitViewModel(int userId, IDialogService dialogService, RepositoryClientService repositoryService)
        {
            UserId = userId;
            _dialogService = dialogService;
            _repositoryClientService = repositoryService;

       
            Task.Run(LoadRepositoriesAsync);
        }



        [RelayCommand]
        private async Task LoadRepositoriesAsync()
        {
            try
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    StatusMessage = "Загрузка репозиториев...";
                });

                var result = await _repositoryClientService.GetRepositoriesAsync(UserId, SearchQuery);

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Repositories.Clear();
                    if (result != null)
                    {
                        foreach (var item in result)
                        {
                            Repositories.Add(new RepositoryListItem(
                                item.Id,
                                item.Name,
                                item.Description ?? string.Empty,
                                item.isPublic
                            ));
                        }
                        StatusMessage = $"Загружено {Repositories.Count} репозиториев";
                    }
                    else
                    {
                        StatusMessage = "Не удалось загрузить репозитории";
                    }
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    StatusMessage = $"Ошибка: {ex.Message}";
                });
                Debug.WriteLine($"LoadRepositories Error: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task SearchRepositoriesAsync()
        {
            await LoadRepositoriesAsync();
        }

        [RelayCommand]
        private async Task LoadRepositoryDetailsAsync()  
        {
            if (SelectedRepository == null)
            {
                StatusMessage = "Выберите репозиторий";
                return;
            }

            try
            {
                StatusMessage = $"Загрузка {SelectedRepository.Name}...";

                var repo = await _repositoryClientService.GetRepositoryAsync(SelectedRepository.Id, UserId);

                if (repo != null)
                {
                    CurrentRepository = repo;
                    Title = repo.Name;
                    Description = repo.Description ?? string.Empty;
                    IsPublic = repo.isPublic;
                    StatusMessage = $"Загружен: {repo.Name}";
                }
                else
                {
                    StatusMessage = "Не удалось загрузить детали репозитория";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
                Debug.WriteLine($"LoadRepositoryDetails Error: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task CreateRepositoryAsync()  
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                StatusMessage = "Введите название репозитория";
                return;
            }

            try
            {
                StatusMessage = "Создание репозитория...";

                var result = await _repositoryClientService.CreateRepositoryAsync(
                    UserId,
                    Title.Trim(),
                    string.IsNullOrWhiteSpace(Description) ? null : Description.Trim(),
                    IsPublic
                );

                if (result != null)
                {
                    StatusMessage = $"Репозиторий '{result.Name}' создан!";
                    Title = string.Empty;
                    Description = string.Empty;

                    await LoadRepositoriesAsync();
                }
                else
                {
                    StatusMessage = "Не удалось создать репозиторий";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
                Debug.WriteLine($"CreateRepository Error: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task DeleteRepositoryAsync()
        {
            if (SelectedRepository == null)
            {
                StatusMessage = "Выберите репозиторий для удаления";
                return;
            }

            try
            {
                StatusMessage = $"Удаление {SelectedRepository.Name}...";

                var success = await _repositoryClientService.DeleteRepositoryAsync(SelectedRepository.Id, UserId);

                if (success)
                {
                    StatusMessage = $"Репозиторий '{SelectedRepository.Name}' удален";
                    CurrentRepository = null;
                    Title = string.Empty;
                    Description = string.Empty;

                    await LoadRepositoriesAsync();
                }
                else
                {
                    StatusMessage = "Не удалось удалить репозиторий";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
                Debug.WriteLine($"DeleteRepository Error: {ex.Message}");
            }
        }

        [RelayCommand]
        private void BrowseFolder()  
        {
            string? selectedPath = _dialogService.SelectFolder();
            if (!string.IsNullOrEmpty(selectedPath))
            {
                _repositoryRoot = selectedPath;
                Path = selectedPath;
                LoadFolderContent(selectedPath);
                StatusMessage = $"Выбрана папка: {selectedPath}";
            }
        }

        private void LoadFolderContent(string targetPath)
        {
            try
            {
                PathToFile.Clear();

                if (!string.IsNullOrEmpty(_repositoryRoot) && targetPath != _repositoryRoot)
                {
                    string? parentDir = Directory.GetParent(targetPath)?.FullName;
                    if (parentDir != null)
                    {
                        PathToFile.Add(new FileSystemItem("...", parentDir, true));
                    }
                }

                foreach (string sub in Directory.GetDirectories(targetPath))
                {
                    PathToFile.Add(new FileSystemItem(System.IO.Path.GetFileName(sub), sub, true));
                }

                foreach (string file in Directory.GetFiles(targetPath))
                {
                    PathToFile.Add(new FileSystemItem(System.IO.Path.GetFileName(file), file, false));
                }

                Path = targetPath;
                StatusMessage = $"Загружено {PathToFile.Count} элементов";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка доступа: {ex.Message}";
                Debug.WriteLine($"LoadFolderContent Error: {ex.Message}");
            }
        }

        [RelayCommand]
        private void LoadTextFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    StatusMessage = "Файл не найден";
                    return;
                }

                string[] lines = File.ReadAllLines(filePath);
                CodeLines.Clear();

                for (int i = 0; i < lines.Length; i++)
                {
                    CodeLines.Add(new TextLine((i + 1).ToString(), lines[i]));
                }

                StatusMessage = $"Загружено {lines.Length} строк из {System.IO.Path.GetFileName(filePath)}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка чтения файла: {ex.Message}";
                Debug.WriteLine($"LoadTextFile Error: {ex.Message}");
            }
        }
      
        partial void OnSelectedItemChanged(FileSystemItem? value)
        {
            if (value == null) return;

            if (value.IsDirectory)
            {
                LoadFolderContent(value.FullPath);
            }
            else if (File.Exists(value.FullPath))
            {
                string[] textExtensions = {
                    ".txt", ".md", ".markdown", ".mdown", ".mkd", ".mkdown",
                    ".cs", ".xaml", ".xml", ".json", ".js", ".html", ".htm",
                    ".css", ".sql", ".yml", ".yaml", ".h", ".c", ".cpp",
                    ".hpp", ".java", ".py", ".rb", ".go", ".rs", ".php",
                    ".config", ".props", ".targets", ".sln", ".csproj",
                    ".gitignore", ".gitattributes", ".editorconfig",
                    ".sh", ".bash", ".ps1", ".bat", ".cmd",
                    ".conf", ".cfg", ".ini", ".env", ".properties",
                    ".csv", ".tsv", ".log", ".readme", ".license",
                    ".psd1", ".psm1", ".pssc", ".cdxml",
                    ".jsonc", ".ts", ".tsx", ".jsx", ".vue",
                    ".sass", ".scss", ".less", ".styl",
                    ".dockerfile", ".makefile", ".cmake",
                    ".gradle", ".groovy", ".lua", ".pl", ".pm",
                    ".r", ".rmd", ".rnw", ".swift", ".kt", ".kts",
                    ".fs", ".fsx", ".fsi", ".fsscript",
                    ".vbs", ".vba", ".vb", ".vbp",
                    ".asm", ".s", ".inc" 
                };
                string ext =System.IO.Path.GetExtension(value.FullPath).ToLower();

                if (textExtensions.Contains(ext))
                {
                    LoadTextFile(value.FullPath);
                }
                else
                {
                    StatusMessage = $"Файл {System.IO.Path.GetFileName(value.FullPath)} не является текстовым";
                }
            }
        }

   
    }
}