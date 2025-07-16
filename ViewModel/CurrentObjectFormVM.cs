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
using System.Threading.Tasks;
using System.Windows.Input;

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

        private async void OpenDialogWindow(IDataObject dataObject)
        {
            _dataObject = dataObject;

            await UpdateWindow();
            _currentWindow.Show();
        }

        private async Task UpdateWindow()
        {
            CurrentObjectMainInfoPageVM.LoadMainInfoCommand.Execute(_dataObject);
            CurrentObjectAttributesVM.LoadAttributesCommand.Execute(_dataObject);

            ChangeObjectNameLabelContent(_dataObject.DisplayName);
            FindParentObject(_dataObject);

            if(TreeItems!= null)
                TreeItems.Clear();

            TreeItems = await UpdateTreeAsync(_dataObject, TreeItems);
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

        private async void FindParentObject(IDataObject currentObject)
        {
            if (currentObject.Id != new Guid("00000001-0001-0001-0001-000000000001"))
            {
                ParentLabelVisibility = System.Windows.Visibility.Visible;
            }
            else
            {
                ParentLabelVisibility = System.Windows.Visibility.Hidden;
            }

            ObjectLoader objectLoader = new ObjectLoader(_objectsRepository);

            _parentDataObject = await objectLoader.Load(currentObject.ParentId);
            ChangeParentNameLabelContentCommand.Execute(_parentDataObject.DisplayName);
        }

        private async Task<ObservableCollection<TreeItem>> UpdateTreeAsync(IDataObject dataObject, ObservableCollection<TreeItem> mainTreeItems)
        {
            TreeItem treeItem;

            if (mainTreeItems != null)
            {
                treeItem = mainTreeItems[0];

                mainTreeItems = new ObservableCollection<TreeItem>
                {
                    new TreeItem { Name = dataObject.DisplayName, DataObject = dataObject, IsExpanded = true, Children = new List<TreeItem>() { treeItem } }
                };
            }
            else
            {
                mainTreeItems = new ObservableCollection<TreeItem>
                {
                    new TreeItem { Name = dataObject.DisplayName, DataObject = dataObject, IsExpanded = true, Children = null }
                };
            }

            if (dataObject.Id != new Guid("00000001-0001-0001-0001-000000000001")) //Начало 00000001-0001-0001-0001-000000000001

            {
                ObjectLoader objectLoader = new ObjectLoader(_objectsRepository);

                var dataObjects = await objectLoader.Load(dataObject.ParentId);
                mainTreeItems = await UpdateTreeAsync(dataObjects, mainTreeItems);
            }

            return mainTreeItems;
        }
    }
}