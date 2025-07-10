using Ascon.Pilot.Theme.Controls;
using MyIceLibrary.View;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MyIceLibrary.Helper
{
    public static class WindowHelper
    {
        public static DialogWindow CreateWindowWithUserControl<T>() where T : UserControl, new()
        {
            DialogWindow window = new DialogWindow();
            
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
