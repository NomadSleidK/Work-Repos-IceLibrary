using Ascon.Pilot.SDK;
using MyIceLibrary.Command;
using MyIceLibrary.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MyIceLibrary.ViewModel.Pages
{
    public class MainInfoPageVM : INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Property
        private ObservableCollection<CurrentObjectInfo> _currentObjectMainInfo;
        public ObservableCollection<CurrentObjectInfo> CurrentObjectMainInfo
        {
            get => _currentObjectMainInfo;
            private set
            {
                _currentObjectMainInfo = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public MainInfoPageVM() { }

        public ICommand LoadMainInfoCommand => new RelayCommand<IDataObject>(LoadMainInfo);

        private void LoadMainInfo(IDataObject dataObject)
        {
            try
            {
                var content = new List<CurrentObjectInfo>();

                content.Add(new CurrentObjectInfo { Name = "DisplayName", Value = dataObject?.DisplayName });
                content.Add(new CurrentObjectInfo { Name = "ID", Value = dataObject?.Id });
                content.Add(new CurrentObjectInfo { Name = "Created", Value = dataObject?.Created });
                content.Add(new CurrentObjectInfo { Name = "Creator", Value = dataObject.Creator?.DisplayName });
                content.Add(new CurrentObjectInfo { Name = "Type", Value = dataObject.Type?.Title });

                CurrentObjectMainInfo = new ObservableCollection<CurrentObjectInfo>(content);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}