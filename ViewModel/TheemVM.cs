using MyIceLibrary.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MyIceLibrary.ViewModel
{
    public class ThemeViewModel : INotifyPropertyChanged
    {
        private System.Windows.ResourceDictionary _currentTheme;
        public System.Windows.ResourceDictionary CurrentTheme
        {
            get => _currentTheme;
            set
            {
                _currentTheme = value;
                OnPropertyChanged();
            }
        }

        // Коллекция доступных тем
        public ObservableCollection<System.Windows.ResourceDictionary> AvailableThemes;

        public ICommand ChangeThemeCommand => new RelayCommand<System.Windows.ResourceDictionary>(theme => { CurrentTheme = theme; });

        public ThemeViewModel()
        {
            //AvailableThemes = new ObservableCollection<ResourceDictionary>();
            AvailableThemes.Add(new System.Windows.ResourceDictionary { Source = new Uri("Style/JediStyle.xaml", UriKind.Relative) });
            AvailableThemes.Add(new System.Windows.ResourceDictionary { Source = new Uri("Style/SithStyle.xaml", UriKind.Relative) });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
