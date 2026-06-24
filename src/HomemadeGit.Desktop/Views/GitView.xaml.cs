using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HomemadeGit.Desktop.Views
{
    /// <summary>
    /// Логика взаимодействия для SelectingView.xaml
    /// </summary>
    public partial class GitView : UserControl
    {
        public GitView()
        {
            InitializeComponent();
            //DataContext = (Application.Current as App).Services.GetService<GitViewModel>();
        }
    }
}
