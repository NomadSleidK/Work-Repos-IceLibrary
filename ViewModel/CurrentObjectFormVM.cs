using Ascon.Pilot.SDK;
using MyIceLibrary.Command;
using MyIceLibrary.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MyIceLibrary.ViewModel
{
    internal class CurrentObjectFormVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<AttributeValue> _attributesValue;
        public ObservableCollection<AttributeValue> CurrentObjectAttributesValue
        {
            get => _attributesValue;
            set
            {
                _attributesValue = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<MainInfoValue> _mainInfoValues;
        public ObservableCollection<MainInfoValue> CurrentObjectMainInfoValues
        {
            get => _mainInfoValues;
            set
            {
                _mainInfoValues = value;
                OnPropertyChanged();
            }
        }

        private string _currentObjectName;
        public string CurrentObjectName
        {
            get => _currentObjectName;
            set
            {
                _currentObjectName = value;
                OnPropertyChanged();
            }
        }

        private string _parentObjectName;
        public string ParentObjectName
        {
            get => _parentObjectName;
            set
            {
                _parentObjectName = value;
                OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand OpenCurrentObjectFormCommand => new RelayCommand<IDataObject>(OpenCurrentObjectForm);
        public ICommand GoToParentCommand => new RelayCommand<object>(_ => GoToParent());
        public ICommand ChangeObjectNameLabelContentCommand => new RelayCommand<string>(UpdateLabel);
        public ICommand ChangeParentNameLabelContentCommand => new RelayCommand<string>(ChangeParentNameLabelContent);
        public ICommand LoadAttributesCommand => new RelayCommand<object>(_ => LoadAttributes());
        public ICommand LoadMainInfoCommand => new RelayCommand<object>(_ => LoadMainInfo());

        private IObjectsRepository _objectsRepository;

        private IDataObject _dataObject;
        private IDataObject _parentDataObject;


        public CurrentObjectFormVM(IObjectsRepository objectsRepository)
        {
            _objectsRepository = objectsRepository;
        }

        private void OpenCurrentObjectForm(IDataObject dataObjects)
        {
            CurrentObjectForm currentObjectForm = new CurrentObjectForm();

            _dataObject = dataObjects;

            LoadAttributesCommand.Execute(null);
            LoadMainInfoCommand.Execute(null);
            ChangeObjectNameLabelContentCommand.Execute(dataObjects.DisplayName);

            FindObjectById(_dataObject.ParentId);

            currentObjectForm.DataContext = this;
            currentObjectForm.Show();
        }

        private void LoadAttributes()
        {
            try
            {
                List<AttributeValue> atr = new List<AttributeValue>();
                
                foreach (var item in _dataObject.Attributes)
                {
                    atr.Add(new AttributeValue(item.Key, item.Value));
                }

                ObservableCollection<AttributeValue> observableCollection = new ObservableCollection<AttributeValue>(atr);

                CurrentObjectAttributesValue = observableCollection;
            }
            catch
            {

            }
        }

        private void LoadMainInfo()
        {
            try
            {
                var content = new List<MainInfoValue>();

                content.Add(new MainInfoValue
                {
                    DisplayName = _dataObject?.DisplayName,
                    ID = _dataObject?.Id,
                    Created = _dataObject?.Created,
                    Creator = _dataObject.Creator?.DisplayName,
                    Type = _dataObject.Type?.Title,
                });

                ObservableCollection<MainInfoValue> observableCollection = new ObservableCollection<MainInfoValue>(content);

                CurrentObjectMainInfoValues = observableCollection;
            }
            catch
            {

            }
        }
        
        private void UpdateLabel(string newName)
        {
            CurrentObjectName = newName;
        }

        private void FindObjectById(Guid id)
        {
            Guid[] guids = new Guid[] { id };

            var dataObjects = _objectsRepository.SubscribeObjects(guids);
            ObserverFindedObjects observer = new ObserverFindedObjects(OnObjectsFind);
            dataObjects.Subscribe(observer);
        }

        private void OnObjectsFind(IDataObject[] obj)
        {
            _parentDataObject = obj[0];
            ChangeParentNameLabelContentCommand.Execute(obj[0].DisplayName);
        }

        private void ChangeParentNameLabelContent(string newName)
        {
            ParentObjectName = newName;
        }

        private void GoToParent()
        {
            CurrentObjectFormVM currentObjectFormVM = new CurrentObjectFormVM(_objectsRepository);
            currentObjectFormVM.OpenCurrentObjectFormCommand.Execute(_parentDataObject);
        }
    }
}
