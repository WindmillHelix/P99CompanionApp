﻿using System;
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
using System.Windows.Shapes;
using WindmillHelix.Companion99.Services;
using WindmillHelix.Companion99.Services.Models;

namespace WindmillHelix.Companion99.App
{
    /// <summary>
    /// Interaction logic for EditNoteWindow.xaml
    /// </summary>
    public partial class EditNoteWindow : Window
    {
        private readonly INoteService _noteService;

        public EditNoteWindow()
        {
            InitializeComponent();
            _noteService = DependencyInjector.Resolve<INoteService>();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            NoteTextBox.Text = WhoResult.Note;
        }

        public WhoResult WhoResult { get; set; }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            _noteService.SetNote(WhoResult.ServerName, WhoResult.Name, NoteTextBox.Text);
            WhoResult.Note = NoteTextBox.Text;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
