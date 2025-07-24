using Ascon.Pilot.SDK;
using MyIceLibrary.Command;
using MyIceLibrary.Model;
using System;
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
        private ObservableCollection<MainInfoValue> _currentObjectMainInfo;
        public ObservableCollection<MainInfoValue> CurrentObjectMainInfo
        {
            get => _currentObjectMainInfo;
            private set
            {
                _currentObjectMainInfo = value;
                OnPropertyChanged();
            }
        }
        #endregion

        private readonly IObjectsRepository _objectsRepository;

        public MainInfoPageVM(IObjectsRepository objectsRepository)
        {
            _objectsRepository = objectsRepository;
        }

        public ICommand LoadMainInfoCommand => new RelayCommand<IDataObject>(LoadMainInfo);

        private void LoadMainInfo(IDataObject dataObject)
        {
            try
            {           
                var content = new MainInfoValue[]
                {
                    new MainInfoValue()
                    {
                        DisplayName = dataObject.DisplayName,
                        ID = dataObject.Id,
                        Created = dataObject.Created.ToLocalTime().ToString("dd-MM-yyyy HH:mm:ss"),
                        Creator = _objectsRepository.GetPerson(dataObject.Creator.Id).DisplayName,
                        Type = dataObject.Type.Name,
                    }
                };

                CurrentObjectMainInfo = new ObservableCollection<MainInfoValue>(content);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}