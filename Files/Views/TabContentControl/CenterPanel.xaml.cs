using Files.Models;
using Files.Resources.Cursors;
using Files.ViewModels.TabContentViewModels;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Files.Views.TabContentControl
{
    /// <summary>
    /// Interaction logic for CenterPanel.xaml
    /// </summary>
    public partial class CenterPanel : UserControl
    {
        private Cursor moveCursor = new Cursor(new MemoryStream(ResourceCursors.Move)); 
        private Cursor copyCursor = new Cursor(new MemoryStream(ResourceCursors.Copy));
        //private Cursor linkCursor = new Cursor(new MemoryStream(ResourceCursors.Link));
        private Cursor noneCursor = new Cursor(new MemoryStream(ResourceCursors.None));

        private bool dragItemIsSelected;
        private Point mouseStart;
        private Point mouseCurent;

        private delegate void FileOperation(string[] files, string target);
        private FileOperation CopyFiles;
        private FileOperation MoveFiles;

        public CenterPanel()
        {
            InitializeComponent();
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            FileList.SelectedItems.Clear();
            ((CenterPanelViewModel)FileList.DataContext).UnselectItem();
        }

        private void FileList_Loaded(object sender, RoutedEventArgs e)
        {
            CopyFiles = ((CenterPanelViewModel)DataContext).CopyFiles;
            MoveFiles = ((CenterPanelViewModel)DataContext).MoveFiles;
        }

        private void FileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((CenterPanelViewModel)FileList.DataContext).SelectedItems = FileList.SelectedItems.Cast<FileDetails>().ToList();
        }

        private void FileListViewItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                dragItemIsSelected = ((FileDetails)((ListViewItem)sender).DataContext).IsSelected;
                mouseStart = e.GetPosition(this);
            }
            else if (e.MiddleButton == MouseButtonState.Pressed)
            {
                ((CenterPanelViewModel)FileList.DataContext).TabContentViewModel.MainViewModel.TabButtonsViewModel.AddTab(
                    new string[] { ((FileDetails)((ListViewItem)sender).DataContext).FullName }, false);
            }
        }

        private void FileListViewItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!dragItemIsSelected) return;

                mouseCurent = e.GetPosition(this);

                if (Math.Abs(mouseCurent.X - mouseStart.X) > SystemParameters.MinimumHorizontalDragDistance || 
                    Math.Abs(mouseCurent.Y - mouseStart.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    if (FileList.SelectedItems.Count == 0) return;

                    string[] data = new string[FileList.SelectedItems.Count];

                    for (int i = 0; i < data.Length; i++)
                        data[i] = ((FileDetails)FileList.SelectedItems[i]).FullName;

                    ((CenterPanelViewModel)DataContext).TabContentViewModel.MainViewModel.UpdateAllowDropTabs(data, false);

                    var result = DragDrop.DoDragDrop((ListViewItem)sender, new DataObject(DataFormats.FileDrop, data),
                        DragDropEffects.Move | DragDropEffects.Copy | DragDropEffects.Link);

                    ((CenterPanelViewModel)DataContext).TabContentViewModel.MainViewModel.UpdateAllowDropTabs(data, true);
                }
            }
        }

        private void FileList_Drop(object sender, DragEventArgs e)
        {
            string target = ((CenterPanelViewModel)((ListView)sender).DataContext).CurrentPath;

            FileDrop(target, e);
        }

        private void FileListViewItem_Drop(object sender, DragEventArgs e)
        {
            string target = ((FileDetails)((ListViewItem)sender).DataContext).FullName;

            FileDrop(target, e);

            e.Handled = true;
        }

        private void FileDrop(string target, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            //if (((int)e.KeyStates & 32) == 32 && (e.AllowedEffects & DragDropEffects.Link) == DragDropEffects.Link)
            //{
            //    // ALT KeyState for link.
            //    e.Effects = DragDropEffects.Link;
            //}
            if (((int)e.KeyStates & 4) == 4 && (e.AllowedEffects & DragDropEffects.Move) == DragDropEffects.Move)
            {
                // SHIFT KeyState for move.
                e.Effects = DragDropEffects.Move;

                MoveFiles(files, target);
            }
            else if (((int)e.KeyStates & 8) == 8 && (e.AllowedEffects & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                // CTRL KeyState for copy.
                e.Effects = DragDropEffects.Copy;

                CopyFiles(files, target);
            }
            else if ((e.AllowedEffects & DragDropEffects.Move) == DragDropEffects.Move)
            {
                // By default, the drop action should be move, if allowed.
                e.Effects = DragDropEffects.Move;

                MoveFiles(files, target);
            }
            else e.Effects = DragDropEffects.None;
        }

        private void FileListViewItem_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = false;

            switch (e.Effects)
            {
                case DragDropEffects.Move:
                    Mouse.SetCursor(moveCursor);
                    break;
                case DragDropEffects.Copy:
                    Mouse.SetCursor(copyCursor);
                    break;
                //case DragDropEffects.Link:
                //    Mouse.SetCursor(linkCursor);
                //    break;
                case DragDropEffects.None:
                    Mouse.SetCursor(noneCursor);
                    break;

                default:
                    e.UseDefaultCursors = true;
                    break;
            }

            e.Handled = true;
        }

        private void SearchPatternTextBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox.Visibility == Visibility.Visible)
            {
                textBox.Focus();
                Keyboard.Focus(textBox);
                textBox.SelectAll();
            }
        }

        private void SearchPatternTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textBox = sender as TextBox;
                ((CenterPanelViewModel)textBox.DataContext).SearchFiles(textBox.Text);
                Keyboard.ClearFocus();
            }
        }
    }
}
