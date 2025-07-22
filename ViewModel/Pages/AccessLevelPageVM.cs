using Ascon.Pilot.SDK;
using MyIceLibrary.Command;
using MyIceLibrary.Helper;
using MyIceLibrary.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MyIceLibrary.ViewModel.Pages
{
    public class AccessLevelPageVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<AccessLevelInfo> _attributesValue;
        public ObservableCollection<AccessLevelInfo> CurrentObjectAttributesValue
        {
            get => _attributesValue;
            private set
            {
                _attributesValue = value;
                OnPropertyChanged();
            }
        }

        private readonly AccessLoader _objectAccessHelper;

        public AccessLevelPageVM(IObjectsRepository objectsRepository)
        {
            _objectAccessHelper = new AccessLoader(objectsRepository);
        }
        public ICommand LoadAccessLevelCommand => new RelayCommand<Guid>(LoadAccessLevel);

        private async void LoadAccessLevel(Guid currentObjectGuid)
        {
            var resultAccess = await _objectAccessHelper.GetObjectAccess(currentObjectGuid);
            
            var result = new List<AccessLevelInfo>();
           
            foreach (var res in resultAccess)
            {
                result.Add(_objectAccessHelper.ConvertAccessRecordToAccessLevelInfo(res));
            }

            CurrentObjectAttributesValue = new ObservableCollection<AccessLevelInfo>(result);
        }      
    }
}