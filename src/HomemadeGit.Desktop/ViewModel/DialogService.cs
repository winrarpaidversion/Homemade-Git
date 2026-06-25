using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Desktop.ViewModel
{
    public interface IDialogService
    {

        string SelectFolder();
    }
    public class DialogService : IDialogService
    {
        public string SelectFolder()
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Выберите папку для инициализации репозитория"
            };

            if (dialog.ShowDialog() == true)
            {
                return dialog.FolderName;
            }
            return null;
        }
    }
}
