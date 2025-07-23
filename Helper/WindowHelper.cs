using Ascon.Pilot.Theme.Controls;
using System;
using System.Windows.Controls;

namespace MyIceLibrary.Helper
{
    public static class WindowHelper
    {
        public static DialogWindow CreateWindowWithUserControl<T>(object dataContext, bool showInTaskbar, string title) where T : UserControl, new()
        {
            DialogWindow window = new DialogWindow();

            try
            {
                window.Content = new T();
                window.DataContext = dataContext;
                window.ShowInTaskbar = showInTaskbar;
                window.Title = title;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error creating UserControl: {ex.Message}");
            }

            return window;
        }
    }
}
