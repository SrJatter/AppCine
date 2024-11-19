using AppCine;
using AppCine.dto;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace SideBar_Nav.Pages
{
    public partial class Upload : Page
    {

        public Upload()
        {
            InitializeComponent();
        }

        private void ShowFileUploadScreen(object sender, RoutedEventArgs e)
        {
            var window = new Upload_File();

            window.ShowDialog();
        }
        private void ShowWidgetUploadScreen(object sender, RoutedEventArgs e)
        {
            var window = new Upload_Widgets();

            window.ShowDialog();
        }
    }
}
