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
    public class CreatorPageVM : INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        #region Property
        private ObservableCollection<CurrentObjectInfo> _currentObjectCreatorInfo;
        public ObservableCollection<CurrentObjectInfo> CurrentObjectCreatorInfo
        {
            get => _currentObjectCreatorInfo;
            private set
            {
                _currentObjectCreatorInfo = value;
                OnPropertyChanged();
            }
        }
        #endregion

        private readonly ObjectLoader _objectLoader;

        public CreatorPageVM(IObjectsRepository objectsRepository)
        {
            _objectLoader = new ObjectLoader(objectsRepository);
        }

        public ICommand LoadCreatorInfoCommand => new RelayCommand<Guid>(LoadCreatorInfo);

        private async void LoadCreatorInfo(Guid objectGuid)
        {
            var dataObject = await _objectLoader.Load(objectGuid);
            var typeInfo = new List<CurrentObjectInfo>();

            typeInfo.Add(new CurrentObjectInfo() { Name = "Id", Value = dataObject.Creator.Id });
            typeInfo.Add(new CurrentObjectInfo() { Name = "ActualName", Value = dataObject.Creator.ActualName });
            typeInfo.Add(new CurrentObjectInfo() { Name = "DisplayName", Value = dataObject.Creator.DisplayName });
            typeInfo.Add(new CurrentObjectInfo() { Name = "IsAdmin", Value = dataObject.Creator.IsAdmin });
            typeInfo.Add(new CurrentObjectInfo() { Name = "Login", Value = dataObject.Creator.Login });
            typeInfo.Add(new CurrentObjectInfo() { Name = "MainPosition", Value = dataObject.Creator.MainPosition });
            typeInfo.Add(new CurrentObjectInfo() { Name = "Sid", Value = dataObject.Creator.Sid });

            CurrentObjectCreatorInfo = new ObservableCollection<CurrentObjectInfo>(typeInfo);
        }
    }
}
