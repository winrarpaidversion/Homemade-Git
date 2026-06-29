using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GalaSoft.MvvmLight.Views;
using HomemadeGit.Core.DTOs;
using HomemadeGit.Core.DTOs.Branches;
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
        private readonly BranchClientService _branchClientService;

        [ObservableProperty]
        private CommitListItemResponse? _selectedCommit;

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

        [ObservableProperty]
        private BranchResponse? _selectedBranch;

        [ObservableProperty]
        private ObservableCollection<BranchResponse> _branches = new();

        [ObservableProperty]
        private string _newBranchName = string.Empty;


        private string TitleRepository => _selectedRepository.Name;
        public GitViewModel(int userId, IDialogService dialogService, RepositoryClientService repositoryService, CommitClientService commitClientService, BranchClientService branchClientService)
        {
            UserId = userId;
            _dialogService = dialogService;
            _repositoryClientService = repositoryService;
            _commitClientService = commitClientService;
            _branchClientService = branchClientService;

            // Загружаем репозитории при старте
            Task.Run(LoadRepositoriesAsync);
        }

        // ===== КОМАНДЫ =====

        [RelayCommand]
        private async Task LoadBranchesAsync()
        {
            if (SelectedRepository == null)
            {
                StatusMessage = "Выберите репозиторий для загрузки веток.";
                return;
            }

            try
            {
                StatusMessage = "Загрузка веток...";

                var result = await _branchClientService.GetBranchesAsync(
                    UserId,
                    SelectedRepository.Id);

                Branches.Clear();

                foreach (var branch in result)
                {
                    Branches.Add(branch);
                }

                SelectedBranch =
                    Branches.FirstOrDefault(b => b.IsDefault)
                    ?? Branches.FirstOrDefault();

                if (SelectedBranch != null)
                {
                    StatusMessage = $"Выбрана ветка: {SelectedBranch.Name}";
                }
                else
                {
                    StatusMessage = "Ветки не найдены. Проверь, создаётся ли main при создании репозитория.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка загрузки веток: {ex.Message}";
                Debug.WriteLine($"LoadBranches Error: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task CreateBranchAsync()
        {
            if (SelectedRepository == null)
            {
                StatusMessage = "Выберите репозиторий.";
                return;
            }

            var branchName = NewBranchName?.Trim();

            if (string.IsNullOrWhiteSpace(branchName))
            {
                StatusMessage = "Введите название ветки.";
                return;
            }

            if (Branches.Any(b =>
            string.Equals(b.Name, branchName, StringComparison.OrdinalIgnoreCase)))
            {
                StatusMessage = $"Ветка '{branchName}' уже существует.";
                return;
            }

            try
            {
                StatusMessage = "Создание ветки...";

                var createdBranch = await _branchClientService.CreateBranchAsync(
                    UserId,
                    SelectedRepository.Id,
                    branchName,
                    SelectedBranch?.Id);

                if (createdBranch == null)
                {
                    StatusMessage = "Не удалось создать ветку.";
                    return;
                }

                NewBranchName = string.Empty;

                await LoadBranchesAsync();

                SelectedBranch = Branches.FirstOrDefault(b => b.Id == createdBranch.Id)
                                 ?? SelectedBranch;

                StatusMessage = $"Ветка '{createdBranch.Name}' создана.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка создания ветки: {ex.Message}";
                Debug.WriteLine($"CreateBranch Error: {ex.Message}");
            }
        }

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

                    await LoadBranchesAsync();
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

            if (SelectedBranch == null)
            {
                StatusMessage = "Выберите ветку.";
                return;
            }

            if (string.IsNullOrWhiteSpace(_repositoryRoot) || !Directory.Exists(_repositoryRoot))
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
                    _repositoryRoot,
                    SelectedBranch.Id,
                    TitleCommit,
                    DescriptionCommit
                );

                if (createdCommit != null)
                {
                    Commits.Add(createdCommit);
                    StatusMessage = $"Коммит '{createdCommit.Title}' успешно создан.";

                    TitleCommit = string.Empty;
                    DescriptionCommit = string.Empty;

                    await LoadBranchesAsync();
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

        [RelayCommand]
        private async Task CloneRepositoryAsync()
        {
            if (SelectedRepository == null)
            {
                StatusMessage = "Выберите репозиторий для клонирования";
                return;
            }

            var targetFolder = _dialogService.SelectFolder();

            if (string.IsNullOrWhiteSpace(targetFolder))
            {
                StatusMessage = "Выберите папку для клонирования.";
                return;
            }

            try
            {
                StatusMessage = "Клонирование репозитория...";

                var snapshot = await _commitClientService.CloneRepositoryAsync(UserId, SelectedRepository.Id, SelectedBranch.Id);

                if(snapshot == null)
                {
                    StatusMessage = "Не удалось получить snapshot репозитория";
                    return;
                }

                ApplySnapshotToFolder(snapshot, targetFolder);

                StatusMessage = $"Репозиторий '{snapshot.RepositoryName}' склонирован в {targetFolder}";

                await LoadCommitAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка клонирования: {ex.Message}";
                Debug.WriteLine($"CloneRepository Error: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task ResetToCommitAsync()
        {
            if (SelectedRepository == null)
            {
                StatusMessage = "Выберите репозиторий.";
                return;
            }

            if (SelectedCommit == null)
            {
                StatusMessage = "Выберите коммит для отката.";
                return;
            }

            if (SelectedBranch == null)
            {
                StatusMessage = "Выберите ветку для отката.";
                return;
            }

            if (string.IsNullOrWhiteSpace(_repositoryRoot) || !Directory.Exists(_repositoryRoot))
            {
                StatusMessage = "Выберите локальную папку репозитория.";
                return;
            }

            try
            {
                StatusMessage = $"Откат ветки до коммита '{SelectedCommit.Title}'...";

                await _commitClientService.ResetBranchToCommitAsync(UserId, SelectedRepository.Id, SelectedBranch.Id, SelectedCommit.Id);

                var snapshot = await _commitClientService.GetCommitSnapshotAsync(UserId, SelectedCommit.Id);

                if(snapshot == null)
                {
                    StatusMessage = "Откат выполнен, но snapshot не получен";
                    await LoadCommitAsync();
                    return;
                }

                ApplySnapshotToFolder(snapshot, _repositoryRoot);

                StatusMessage = $"Откат выполнен до коммита '{SelectedCommit.Title}'";

                await LoadCommitAsync();
            }
            catch(Exception ex)
            {
                StatusMessage = $"Ошибка отката: {ex.Message}";
                Debug.WriteLine($"ResetToCommit Error: {ex.Message}");
            }
        }

        private void ApplySnapshotToFolder(RepositorySnapshotResponse snapshot, string targetFolder)
        {
            if (!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);

            foreach(var file in Directory.GetFiles(targetFolder, "*", SearchOption.AllDirectories))
            {
                File.Delete(file);
            }

            foreach(var directory in Directory.GetDirectories(targetFolder, "*", SearchOption.AllDirectories))
            {
                if (Directory.Exists(directory) && !Directory.EnumerateFileSystemEntries(directory).Any())
                    Directory.Delete(directory);     
            }
             
            foreach(var file in snapshot.Files)
            {
                var safeRelativePath = file.Path.Replace('\\', '/').TrimStart('/');

                var fullPath = System.IO.Path.Combine(targetFolder, safeRelativePath.Replace('/', System.IO.Path.DirectorySeparatorChar));

                string? directory = System.IO.Path.GetDirectoryName(fullPath);

                if(!string.IsNullOrWhiteSpace(directory))
                    Directory.CreateDirectory(directory);

                File.WriteAllBytes(fullPath, file.Data);
            }

            _repositoryRoot = targetFolder;
            Path = targetFolder;
            LoadFolderContent(targetFolder);
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
            ClearRepositoryViewState();

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

        private void ClearRepositoryViewState()
        {
            Historycommits.Clear();
            Commits.Clear();

            SelectedCommit = null;

            Branches.Clear();
            SelectedBranch = null;

            CodeLines.Clear();
            PathToFile.Clear();

            CurrentRepository = null;

            _repositoryRoot = string.Empty;
            Path = string.Empty;
        }

        // Свойство для XAML
        public int SelectedRepositoryId => SelectedRepository?.Id ?? 0;
    }
}