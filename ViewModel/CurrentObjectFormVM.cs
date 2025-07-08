using Ascon.Pilot.SDK;
using MyIceLibrary.Command;
using MyIceLibrary.Model;
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
        #region Observable
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
        #endregion

        public ICommand OpenCurrentObjectFormCommand => new RelayCommand<IDataObject>(OpenCurrentObjectForm);
        public ICommand GoToParentCommand => new RelayCommand<object>(_ => GoToParent());
        public ICommand ChangeObjectNameLabelContentCommand => new RelayCommand<string>(ChangeObjectNameLabelContent);
        public ICommand ChangeParentNameLabelContentCommand => new RelayCommand<string>(ChangeParentNameLabelContent);
        public ICommand LoadAttributesCommand => new RelayCommand<object>(_ => LoadAttributes());
        public ICommand LoadMainInfoCommand => new RelayCommand<object>(_ => LoadMainInfo());

        private IObjectsRepository _objectsRepository;

        private IDataObject _dataObject;
        private IDataObject _parentDataObject;

        private CurrentObjectForm _currentForm;

        public CurrentObjectFormVM(IObjectsRepository objectsRepository)
        {
            _objectsRepository = objectsRepository;
        }

        private void OpenCurrentObjectForm(IDataObject dataObjects)
        {
            _currentForm = new CurrentObjectForm();

            _dataObject = dataObjects;

            LoadAttributesCommand.Execute(null);
            LoadMainInfoCommand.Execute(null);
            ChangeObjectNameLabelContentCommand.Execute(dataObjects.DisplayName);

            FindObjectById(_dataObject.ParentId);

            _currentForm.DataContext = this;
            _currentForm.Show();
        }

        private void LoadAttributes()
        {
            try
            {
                List<AttributeValue> attributes = new List<AttributeValue>();
                
                foreach (var item in _dataObject.Attributes)
                {
                    attributes.Add(new AttributeValue(item.Key, item.Value));
                }

                ObservableCollection<AttributeValue> observableCollection = new ObservableCollection<AttributeValue>(attributes);

                CurrentObjectAttributesValue = observableCollection;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void LoadMainInfo()
        {
            try
            {
                CurrentObjectMainInfoValues = DataGridHelper.GetMainInfoObservableCollection(new IDataObject[] { _dataObject });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        
        private void ChangeObjectNameLabelContent(string newName)
        {
            CurrentObjectName = newName;
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

        #region Find Object
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
        #endregion
    }
}