﻿using Client_ADBD.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Client_ADBD.Models;

namespace Client_ADBD.Views
{
    /// <summary>
    /// Interaction logic for ModifyPostPage.xaml
    /// </summary>
    public partial class ModifyPostPage : Page
    {
        public ModifyPostPage(bool isAdmin = false)
        {
            InitializeComponent();
        }

        public ModifyPostPage(Post_ post, bool isAdmin = false)
        {
            InitializeComponent();
            VM_ModifyPostPage viewModel = new VM_ModifyPostPage(post, isAdmin);
            DataContext = viewModel;
        }
    }
}
