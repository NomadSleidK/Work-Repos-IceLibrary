using System;
using System.Windows;
using System.Windows.Controls;

namespace MyIceLibrary.Helper
{
    public static class WindowHelper
    {
        public static Window CreateWindowWithUserControl<T>() where T : UserControl, new()
        {
            Window window = new Window();

            try
            {
                window.Content = new T();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating UserControl: {ex.Message}");
            }

            return window;
        }
    }
}
