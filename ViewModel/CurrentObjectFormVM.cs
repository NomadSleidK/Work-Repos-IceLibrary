using Ascon.Pilot.SDK;
using MyIceLibrary.Command;
using MyIceLibrary.Helper;
using MyIceLibrary.Model;
using MyIceLibrary.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using static MyIceLibrary.ObserverFindObjectById;

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

        public bool IsExpanded => true;

        private ObservableCollection<TreeItem> _treeItems;
        public ObservableCollection<TreeItem> TreeItems
        {
            get => _treeItems;
            set
            {
                _treeItems = value;
                OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand OpenCommand => new RelayCommand<IDataObject>(OpenForm);
        public ICommand GoToParentCommand => new RelayCommand<object>(_ => GoToParent());
        public ICommand ChangeObjectNameLabelContentCommand => new RelayCommand<string>(ChangeObjectNameLabelContent);
        public ICommand ChangeParentNameLabelContentCommand => new RelayCommand<string>(ChangeParentNameLabelContent);
        public ICommand LoadAttributesCommand => new RelayCommand<object>(_ => LoadAttributes());
        public ICommand LoadMainInfoCommand => new RelayCommand<object>(_ => LoadMainInfo());

        private IObjectsRepository _objectsRepository;
        private IDataObject _dataObject;
        private IDataObject _parentDataObject;

        private System.Windows.Window _currentWindow;

        public CurrentObjectFormVM(IObjectsRepository objectsRepository)
        {
            _objectsRepository = objectsRepository;
        }

        private void OpenForm(IDataObject dataObjects)
        {
            _dataObject = dataObjects;

            _currentWindow = WindowHelper.CreateWindowWithUserControl<CurrentObjectUserControl>();

            LoadAttributesCommand.Execute(null);
            LoadMainInfoCommand.Execute(null);
            ChangeObjectNameLabelContentCommand.Execute(dataObjects.DisplayName);

            FindObjectById(_dataObject.ParentId, OnObjectsFind);

            UpdateTree(_dataObject);

            _currentWindow.DataContext = this;
            _currentWindow.Show();
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
            currentObjectFormVM.OpenCommand.Execute(_parentDataObject);
        }

        #region Find Object
        private void FindObjectById(Guid id, LoadObjectsHandler loadObjectsHandler)
        {
            try
            {
                Guid[] guids = new Guid[] { id };

                var dataObjects = _objectsRepository.SubscribeObjects(guids);
                ObserverFindObjectById observer = new ObserverFindObjectById(loadObjectsHandler);
                dataObjects.Subscribe(observer);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void OnObjectsFind(IDataObject obj)
        {
            _parentDataObject = obj;
            ChangeParentNameLabelContentCommand.Execute(obj.DisplayName);
        }
        #endregion

        #region 
        //Начало 00000001-0001-0001-0001-000000000001

        private void AddTreeElement(IDataObject dataObject)
        {

            if (dataObject.Id != new Guid("00000001-0001-0001-0001-000000000001"))
            {
                var dataObjects = _objectsRepository.SubscribeObjects(new Guid[] { dataObject.ParentId });
                ObserverFindObjectById observer = new ObserverFindObjectById(UpdateTree);
                dataObjects.Subscribe(observer);
            }
        }

        private void UpdateTree(IDataObject dataObject)
        {
            TreeItem treeItem;

            if (TreeItems != null)
            {
                treeItem = TreeItems[0];

                TreeItems = new ObservableCollection<TreeItem>
                {
                    new TreeItem { Name = dataObject.DisplayName, IsExpanded = true, Children = new List<TreeItem>() { treeItem } }
                };
            }
            else
            {
                TreeItems = new ObservableCollection<TreeItem>
                {
                    new TreeItem { Name = dataObject.DisplayName, IsExpanded = true, Children = null }
                };
            } 

            AddTreeElement(dataObject);
        }
        #endregion
    }
}