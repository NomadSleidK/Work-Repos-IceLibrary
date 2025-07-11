using Ascon.Pilot.SDK;
using MyIceLibrary.Command;
using MyIceLibrary.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MyIceLibrary.ViewModel.Pages
{
    public class TypePageVM : INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        #region Property
        private ObservableCollection<CurrentObjectInfo> _currentObjectTypeInfo;
        public ObservableCollection<CurrentObjectInfo> CurrentObjectTypeInfo
        {
            get => _currentObjectTypeInfo;
            private set
            {
                _currentObjectTypeInfo = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public TypePageVM() { }

        public ICommand LoadTypeInfoCommand => new RelayCommand<IDataObject>(LoadTypeInfo);

        private void LoadTypeInfo(IDataObject dataObject)
        {
            var typeInfo = new List<CurrentObjectInfo>();

            typeInfo.Add(new CurrentObjectInfo() { Name = "Id", Value = dataObject.Type.Id});
            typeInfo.Add(new CurrentObjectInfo() { Name = "Name", Value = dataObject.Type.Name });
            typeInfo.Add(new CurrentObjectInfo() { Name = "Title", Value = dataObject.Type.Title });
            typeInfo.Add(new CurrentObjectInfo() { Name = "Kind", Value = dataObject.Type.Kind });
            typeInfo.Add(new CurrentObjectInfo() { Name = "IsMountable", Value = dataObject.Type.IsMountable });
            typeInfo.Add(new CurrentObjectInfo() { Name = "IsProject", Value = dataObject.Type.IsProject });
            typeInfo.Add(new CurrentObjectInfo() { Name = "IsService", Value = dataObject.Type.IsService });

            CurrentObjectTypeInfo = new ObservableCollection<CurrentObjectInfo>(typeInfo);
        }
    }
}
