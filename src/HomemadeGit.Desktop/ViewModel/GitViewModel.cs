using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection.Metadata;
using System.Text;
using System.Windows.Shapes;

namespace HomemadeGit.Desktop.ViewModel
{

    public partial class GitViewModel : ViewModelBase
    {
        private string _repositoryRoot;
        [ObservableProperty]
        private string _title;
        [ObservableProperty]
        private string _description;
        [ObservableProperty]
        private string _path;
        [ObservableProperty]
        private FileSystemItem _selectedItem;
        private DialogService DialogService;
        public record FileSystemItem(string Name, string FullPath, bool IsDirectory);
        [ObservableProperty]
        private ObservableCollection<FileSystemItem> pathtofile = new();

        public record textLine(string Number, string Text);

        [ObservableProperty]
        public ObservableCollection<textLine> _codeLine = new ObservableCollection<textLine>();

        public int UserId;

        public GitViewModel(int userId)
        {
            UserId = userId;
            DialogService = new DialogService();
        }
        [RelayCommand]
        private void PushCommit()
        {
            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Description)) return;

        }
        [RelayCommand]
        private void NewRepository()
        {
            Pathtofile.Clear();
            Path = DialogService.SelectFolder();
            if (Path == null) return;

            LoadFolderContent(Path);
        }
        private void LoadFolderContent(string targetPath)
        {
            try
            {
             
                pathtofile.Clear();

                if (targetPath != _repositoryRoot)
                {
                    string parentDir = Directory.GetParent(targetPath)?.FullName;
                    if (parentDir != null)
                    {
                        pathtofile.Add(new FileSystemItem("...", parentDir, true));
                    }
                }

     
                string[] subdirect = Directory.GetDirectories(targetPath);
                foreach (string sub in subdirect)
                {
                    pathtofile.Add(new FileSystemItem(System.IO.Path.GetFileName(sub), sub, true));
                }

           
                string[] subfile = Directory.GetFiles(targetPath);
                foreach (string file in subfile)
                {
            
                    pathtofile.Add(new FileSystemItem(System.IO.Path.GetFileName(file), file, false));
                }

          
                Path = targetPath;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка доступа к папке: {ex.Message}");
            }

        }
        [RelayCommand]
        private void LoadAndIndexText(string filePath)
        {
            string[] line = File.ReadAllLines(filePath);
            CodeLine.Clear();
            for (int i = 0; i < line.Length; i++)
            {
                string num = (i + 1).ToString();
                string text = line[i];
                CodeLine.Add(new textLine(num, text));
            }


        }
        partial void OnSelectedItemChanged(FileSystemItem value)
        {
            if (value == null) return;

            if (!value.IsDirectory && File.Exists(value.FullPath))
            {
                LoadAndIndexText(value.FullPath);
            }
            else if (value.IsDirectory) { LoadFolderContent(value.FullPath); }
        }
    }

}


