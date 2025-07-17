using Ascon.Pilot.SDK;
using Ascon.Pilot.Theme.Controls;
using MyIceLibrary.Command;
using MyIceLibrary.Helper;
using MyIceLibrary.Model;
using MyIceLibrary.View;
using MyIceLibrary.ViewModel.Pages;
using System;
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
        private ObservableCollection<TreeItem> _originTreeItems;

        private readonly ObjectsTreeBuilder _objectsTreeBuilder;

        public CurrentObjectFormVM(IObjectModifier modifier, IObjectsRepository objectsRepository)
        {
            _currentWindow = WindowHelper.CreateWindowWithUserControl<CurrentObjectUserControl>();
            _currentWindow.DataContext = this;
            _currentWindow.ShowInTaskbar = true;

            _objectsRepository = objectsRepository;

            _objectsTreeBuilder = new ObjectsTreeBuilder(_objectsRepository);

            CurrentObjectMainInfoPageVM = new MainInfoPageVM();
            CurrentObjectAttributesVM = new AttributesPageVM();
            SelectedElementTabControlVM = new InfoTabControlVM(modifier, objectsRepository);
        }

        public ICommand OpenCommand => new RelayCommand<IDataObject>(OpenDialogWindow);
        public ICommand GoToParentCommand => new RelayCommand<object>(_ => GoToParent());
        public ICommand ChangeParentNameLabelContentCommand => new RelayCommand<string>(ChangeParentNameLabelContent);
        public ICommand SelectedElementCommand => new RelayCommand<TreeItem>(OnTabSelected);
        public ICommand FilteredBoxExecuteEnterCommand => new RelayCommand<string>(FilteredBoxExecuteEnter);

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

            if(TreeItems != null)
                TreeItems.Clear();

            _originTreeItems = await _objectsTreeBuilder.CreateFullTreeAsync(_dataObject, TreeItems);

            TreeItems = new ObservableCollection<TreeItem>(_originTreeItems.DeepCopy());
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

        private async void FilteredBoxExecuteEnter(string parameter)
        {
            TreeItems = await _objectsTreeBuilder.FilteredTreeItemsAsync(parameter, _originTreeItems);
        }       
    }
}