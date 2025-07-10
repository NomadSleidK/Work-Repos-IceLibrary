using Ascon.Pilot.SDK;
using Ascon.Pilot.Theme.Controls;
using MyIceLibrary.Command;
using MyIceLibrary.Helper;
using MyIceLibrary.Model;
using MyIceLibrary.View;
using MyIceLibrary.ViewModel.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using static MyIceLibrary.ObserverFindObjectById;

namespace MyIceLibrary.ViewModel
{
    internal class CurrentObjectFormVM : INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region View Model Properties

        private string _currentObjectName;
        public string CurrentObjectName
        {
            get => _currentObjectName;
            private set
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

        private System.Windows.Visibility _parentLabelVisibility;
        public System.Windows.Visibility ParentLabelVisibility
        {
            get => _parentLabelVisibility;
            private set
            {
                _parentLabelVisibility = value;
                OnPropertyChanged();
            }
        }

        private AttributesPageVM _currentObjectAttributesVM;
        public AttributesPageVM CurrentObjectAttributesVM
        {
            get => _currentObjectAttributesVM;
            private set
            {
                _currentObjectAttributesVM = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<CurrentObjectInfo> _mainInfoValues;
        public ObservableCollection<CurrentObjectInfo> CurrentObjectMainInfoValues
        {
            get => _mainInfoValues;
            private set
            {
                _mainInfoValues = value;
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

        private InfoTabControlVM _selectedElementTabControlVM;
        public InfoTabControlVM SelectedElementTabControlVM
        {
            get => _selectedElementTabControlVM;
            private set
            {
                _selectedElementTabControlVM = value;
                OnPropertyChanged();
            }
        }



        #endregion



        public ICommand OpenCommand => new RelayCommand<IDataObject>(OpenForm);
        public ICommand GoToParentCommand => new RelayCommand<object>(_ => GoToParent());
        public ICommand ChangeObjectNameLabelContentCommand => new RelayCommand<string>(ChangeObjectNameLabelContent);
        public ICommand ChangeParentNameLabelContentCommand => new RelayCommand<string>(ChangeParentNameLabelContent);
        //public ICommand LoadChildrenCommand => new RelayCommand<object>(_ => LoadAttributes());
        public ICommand LoadMainInfoCommand => new RelayCommand<object>(_ => LoadMainInfo());

        private IObjectsRepository _objectsRepository;
        private IDataObject _dataObject;
        private IDataObject _parentDataObject;

        private DialogWindow _currentWindow;

        public CurrentObjectFormVM(IObjectsRepository objectsRepository)
        {
            _objectsRepository = objectsRepository;
            _currentWindow = WindowHelper.CreateWindowWithUserControl<CurrentObjectUserControl>();
            _currentWindow.DataContext = this;

            CurrentObjectAttributesVM = new AttributesPageVM();
            SelectedElementTabControlVM = new InfoTabControlVM(objectsRepository);
        }


        private object _selectedTreeItem;
        public object SelectedTreeItem
        {
            get => _selectedTreeItem;
            set
            {
                if (_selectedTreeItem != value)
                {
                    _selectedTreeItem = value;
                    OnPropertyChanged(nameof(SelectedTreeItem));
                    // Дополнительная логика при изменении выбранного элемента
                    System.Windows.MessageBox.Show("Нажатие");
                }
            }
        }

        private void OnSelectedItemChanged(TreeItem item)
        {
            // Обработка выбранного элемента
            if (item != null)
            {
                Debug.WriteLine($"Selected: {item.Name}");
            }
        }


        public ICommand SelectedElementCommand => new RelayCommand<TreeItem>(OnTabSelected);

        private void OnTabSelected(TreeItem selectedTab)
        {
            System.Windows.MessageBox.Show("Нажатие");

            //var dataObject = (Ascon.Pilot.SDK.IDataObject) selectedTab;

            //SelectedElementChildrenPageVM.LoadChildrenCommand.Execute(dataObject);
            //SelectedElementAttributesVM.LoadAttributesCommand.Execute(dataObject);
        }


        private void OpenForm(IDataObject dataObjects)
        {
            _dataObject = dataObjects;

            LoadMainInfoCommand.Execute(null);

            ChangeObjectNameLabelContentCommand.Execute(dataObjects.DisplayName);



            CurrentObjectAttributesVM.LoadAttributesCommand.Execute(dataObjects);



            FindObjectById(_dataObject.ParentId, OnParentFind);
            CheckRootParent();
            UpdateTree(_dataObject);

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

                //CurrentObjectChildren = observableCollection;
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
                CurrentObjectMainInfoValues = DataGridHelper.GetMainInfoObservableCollectionByObject(_dataObject);
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

        private void CheckRootParent()
        {
            if (_dataObject.Id != new Guid("00000001-0001-0001-0001-000000000001"))
            {
                ParentLabelVisibility = System.Windows.Visibility.Visible;
            }
            else
            {
                ParentLabelVisibility = System.Windows.Visibility.Hidden;
            }
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

        private void OnParentFind(IDataObject obj)
        {
            _parentDataObject = obj;
            ChangeParentNameLabelContentCommand.Execute(obj.DisplayName);
        }
        #endregion

        #region Tree View
        private void AddTreeElement(IDataObject dataObject)
        {

            if (dataObject.Id != new Guid("00000001-0001-0001-0001-000000000001")) //Начало 00000001-0001-0001-0001-000000000001

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
                    new TreeItem { Name = dataObject.DisplayName, DataObject = dataObject, IsExpanded = true, Children = new List<TreeItem>() { treeItem } }
                };
            }
            else
            {
                TreeItems = new ObservableCollection<TreeItem>
                {
                    new TreeItem { Name = dataObject.DisplayName, DataObject = dataObject, IsExpanded = true, Children = null }
                };
            } 

            AddTreeElement(dataObject);
        }
        #endregion
    }
}