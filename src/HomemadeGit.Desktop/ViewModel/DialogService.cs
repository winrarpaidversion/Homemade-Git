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
            var dialog = new OpenFolderDialog();
          

            if (dialog.ShowDialog() == true)
            {
                dialog.ShowHiddenItems = true;
                return dialog.FolderName;

            }
            return null;

        }
        public string SelectFile()
        {
            var dialog = new OpenFileDialog();


            if (dialog.ShowDialog() == true)
            {
                dialog.ShowHiddenItems = true;
                return dialog.FileName;

            }
            return null;
        }
    }
}
