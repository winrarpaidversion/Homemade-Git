using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection.Metadata;
using System.Text;

namespace HomemadeGit.Desktop.ViewModel
{

    public partial class GitViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _title;
        [ObservableProperty]
        private string _description;
        [ObservableProperty]
        private string _path;
        private DialogService DialogService;
      
        public record textLine(string Number, string Text);

        [ObservableProperty]
        public ObservableCollection<textLine> _codeLine = new ObservableCollection<textLine>();

        public GitViewModel()
        {
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
            Path = DialogService.SelectFile();
            if(Path== null) return;
            LoadAndIndexText(Path);


        }
        [RelayCommand]
        private void LoadAndIndexText(string filePath)
        {
            string[] line = File.ReadAllLines(filePath);
            CodeLine.Clear();
            for (int i = 0; i< line.Length; i++)
            {
               string num = (i+1).ToString();
                string text = line[i];
                CodeLine.Add(new textLine(num, text));
            }
       
        }
    }

}


