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
using System.Runtime.CompilerServices;
using System.Windows.Input;
using static MyIceLibrary.ObserverFindObjectById;

namespace MyIceLibrary.ViewModel
{
    public class CurrentObjectFormVM : INotifyPropertyChanged
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

        private MainInfoPageVM _currentObjectMainInfoPageVM;
        public MainInfoPageVM CurrentObjectMainInfoPageVM
        {
            get => _currentObjectMainInfoPageVM;
            private set
            {
                _currentObjectMainInfoPageVM = value;
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

        private readonly IObjectsRepository _objectsRepository;
        
        private IDataObject _dataObject;
        private IDataObject _parentDataObject;

        private readonly DialogWindow _currentWindow;
        private readonly IObjectModifier _modifier;

        public CurrentObjectFormVM(IObjectModifier modifier, IObjectsRepository objectsRepository)
        {
            _currentWindow = WindowHelper.CreateWindowWithUserControl<CurrentObjectUserControl>();
            _currentWindow.DataContext = this;
            _objectsRepository = objectsRepository;

            CurrentObjectMainInfoPageVM = new MainInfoPageVM();
            CurrentObjectAttributesVM = new AttributesPageVM();
            SelectedElementTabControlVM = new InfoTabControlVM(modifier, objectsRepository);
        }

        public ICommand OpenCommand => new RelayCommand<IDataObject>(OpenDialogWindow);
        public ICommand GoToParentCommand => new RelayCommand<object>(_ => GoToParent());
        public ICommand ChangeParentNameLabelContentCommand => new RelayCommand<string>(ChangeParentNameLabelContent);
        public ICommand SelectedElementCommand => new RelayCommand<TreeItem>(OnTabSelected);

        private void OpenDialogWindow(IDataObject dataObject)
        {
            _dataObject = dataObject;

            UpdateWindow();
            _currentWindow.Show();
        }

        private void UpdateWindow()
        {
            CurrentObjectMainInfoPageVM.LoadMainInfoCommand.Execute(_dataObject);
            CurrentObjectAttributesVM.LoadAttributesCommand.Execute(_dataObject);

            ChangeObjectNameLabelContent(_dataObject.DisplayName);
            FindObjectById(_dataObject.ParentId, OnParentFind);

            CheckRootParent();
            UpdateTree(_dataObject);
        }

        private void GoToParent()
        {
            CurrentObjectFormVM currentObjectFormVM = new CurrentObjectFormVM(_modifier, _objectsRepository);
            currentObjectFormVM.OpenCommand.Execute(_parentDataObject);
        }
     
        private void ChangeParentNameLabelContent(string newName)
        {
            ParentObjectName = newName;
        }

        private void OnTabSelected(TreeItem selectedTab)
        {

            SelectedElementTabControlVM.UpdateInfoCommand.Execute(selectedTab.DataObject);
        }

        private void ChangeObjectNameLabelContent(string newName)
        {
            CurrentObjectName = newName;
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