using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GalaSoft.MvvmLight.Views;
using HomemadeGit.Core.DTOs.Commits;
using HomemadeGit.Core.DTOs.Repositories;
using HomemadeGit.Core.Models;
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
        [ObservableProperty]
        private string _TitleCommit;
        [ObservableProperty]
        private string _descriptionCommit;
        private string _repositoryRoot = string.Empty;
        private IDialogService _dialogService;
        private readonly RepositoryClientService _repositoryClientService;
        private readonly CommitClientService _commitClientService;
<<<<<<< HEAD
        private readonly BranchClientService _branchClientService;
        private Dictionary<int, string> _repositoryLocalPaths = new();
=======

        [ObservableProperty]
        private CommitListItemResponse? _selectedCommit;
>>>>>>> ea119a82fbc1c01a107ffd57216c0314c955b17b

        // DTO для отображения в списке
        public record RepositoryListItem(int Id, string Name, string? Description, bool IsPublic);
        public record FileSystemItem(string Name, string FullPath, bool IsDirectory);
        public record TextLine(string Number, string Text);

        // ===== СВОЙСТВА ДЛЯ ПРИВЯЗКИ В XAML =====

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
        private bool _isPublic = true;  // ВАЖНО: для привязки к CheckBox

        [ObservableProperty]
        private string _statusMessage = "Готов";  // ВАЖНО: для строки статуса

        [ObservableProperty]
        private ObservableCollection<RepositoryListItem> _repositories = new();  // ВАЖНО: называется Repositories

        [ObservableProperty]
        private ObservableCollection<CommitListItemResponse> _historycommits = new();

        [ObservableProperty]
        private ObservableCollection<CommitResponse> _commits = new();

        [ObservableProperty]
        private ObservableCollection<FileSystemItem> _pathToFile = new();  // ВАЖНО: называется PathToFile

        [ObservableProperty]
        private ObservableCollection<TextLine> _codeLines = new();  // ВАЖНО: называется CodeLines

<<<<<<< HEAD
        public GitViewModel(int userId, IDialogService dialogService, RepositoryClientService repositoryService, CommitClientService commitClientService, BranchClientService branchClientService)
=======
        private string TitleRepository => _selectedRepository.Name;
        public GitViewModel(int userId, IDialogService dialogService, RepositoryClientService repositoryService, CommitClientService commitClientService)
>>>>>>> ea119a82fbc1c01a107ffd57216c0314c955b17b
        {
            UserId = userId;
            _dialogService = dialogService;
            _repositoryClientService = repositoryService;
            _commitClientService = commitClientService;
<<<<<<< HEAD
            _branchClientService = branchClientService;
=======


>>>>>>> ea119a82fbc1c01a107ffd57216c0314c955b17b
            // Загружаем репозитории при старте
            Task.Run(LoadRepositoriesAsync);
        }

        // ===== КОМАНДЫ =====

        private async Task LoadCommitAsync()
        {
            if (SelectedRepository == null)
            {
                StatusMessage = "Выберите репозиторий для загрузки коммитов.";
                return;
            }
            try
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    StatusMessage = "Загрузка коммитов...";
                });

                var commits = await _commitClientService.GetRepositoryCommitsAsync(UserId, SelectedRepository.Id);

                // Безопасное обновление коллекции в UI-потоке
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Historycommits.Clear();
                    if (commits != null)
                    {
                        foreach (var commit in commits)
                        {
                            Historycommits.Add(commit);
                        }
                        StatusMessage = "Коммиты загружены.";
                    }
                    else
                    {
                        StatusMessage = "Коммиты не найдены.";
                    }
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    StatusMessage = $"Ошибка при загрузке коммитов: {ex.Message}";
                });
            }
        }

        [RelayCommand]
        public async Task LoadCommitDetailsAsync()
        {
            if (SelectedCommit == null || SelectedRepository == null) return;

            try
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    StatusMessage = $"Загрузка деталей коммита {SelectedCommit.Title}...";
                });

                var commitDetails = await _commitClientService.GetCommitByIdAsync(UserId, SelectedCommit.Id);

                if (commitDetails != null)
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        // 1. Показываем информацию о самом коммите на центральной панели
                        Commits.Clear();
                        Commits.Add(commitDetails);

                        // 2. Распаковываем пути файлов и добавляем в панель файлов
                        PathToFile.Clear();
                        if (commitDetails.Files != null)
                        {
                            foreach (var file in commitDetails.Files)
                            {
                                PathToFile.Add(new FileSystemItem(
                                    Name: System.IO.Path.GetFileName(file.Path),
                                    FullPath: file.Path,
                                    IsDirectory: false));
                            }
                        }

                        StatusMessage = $"Коммит {SelectedCommit.Title} загружен.";
                    });
                }
                else
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        StatusMessage = "Не удалось загрузить детали коммита.";
                    });
                }
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    StatusMessage = $"Ошибка при загрузке коммита: {ex.Message}";
                });
                Debug.WriteLine($"LoadCommitDetailsAsync Error: {ex.Message}");
            }
        }

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
                    await LoadCommitAsync();
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
        private async Task NewCommitAsync()
        {
            if (SelectedRepository == null)
            {
                StatusMessage = "Выберите репозиторий.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Path) || !Directory.Exists(Path))
            {
                StatusMessage = "Выберите корректную папку для коммита.";
                return;
            }

            if (string.IsNullOrWhiteSpace(TitleCommit))
            {
                StatusMessage = "Введите заголовок коммита.";
                return;
            }

            try
            {
                StatusMessage = "Создание коммита...";

                var createdCommit = await _commitClientService.CreateCommitFormFolderAsync(
                    UserId,
                    SelectedRepository.Id,
                    Path,
                    TitleCommit,
                    DescriptionCommit
                );

                if (createdCommit != null)
                {
                    Commits.Add(createdCommit);
                    StatusMessage = $"Коммит '{createdCommit.Title}' успешно создан.";

                    TitleCommit = string.Empty;
                    DescriptionCommit = string.Empty;
                    await LoadCommitAsync();
                }
                else
                {
                    StatusMessage = "Не удалось создать коммит.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка при создании коммита: {ex.Message}";
                Debug.WriteLine($"NewCommit Error: {ex.Message}");
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
                string[] textExtensions = { ".txt", ".cs", ".xaml", ".xml", ".json", ".js", ".html", ".css", ".md", ".sql", ".yml", ".yaml" };
                string ext = System.IO.Path.GetExtension(value.FullPath).ToLower();

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

        partial void OnSelectedRepositoryChanged(RepositoryListItem? value)
        {
            if (value != null)
            {
                LoadRepositoryDetailsCommand.Execute(null);
            }
        }

        partial void OnSelectedCommitChanged(CommitListItemResponse? value)
        {
            if (value != null)
            {
                // ИСПРАВЛЕНО: Вызываем сгенерированную команду LoadCommitDetailsCommand
                LoadCommitDetailsCommand.Execute(null);
            }
        }

        // Свойство для XAML
        public int SelectedRepositoryId => SelectedRepository?.Id ?? 0;
    }
}