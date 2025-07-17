using Ascon.Pilot.SDK;
using MyIceLibrary.Command;
using MyIceLibrary.Helper;
using MyIceLibrary.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MyIceLibrary.ViewModel.Pages
{
    public class ObjectPathTreePageVM : INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Properties
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
        public InfoTabControlVM SelectedObjectInfoTabControlVM
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
        private readonly IObjectModifier _objectModifier;
        private readonly ObjectLoader _objectLoader;
        private readonly ObjectsTreeBuilder _objectsTreeBuilder;

        private Guid _currentObject;
        private ObservableCollection<TreeItem> _originTreeItems;

        public ObjectPathTreePageVM(IObjectsRepository objectsRepository, IObjectModifier objectModifier)
        {
            _objectsRepository = objectsRepository;
            _objectModifier = objectModifier;

            _objectLoader = new ObjectLoader(_objectsRepository);
            _objectsTreeBuilder = new ObjectsTreeBuilder(_objectsRepository);

            SelectedObjectInfoTabControlVM = new InfoTabControlVM(_objectModifier, _objectsRepository);
        }

        public ICommand LoadPageCommand => new RelayCommand<Guid>(LoadPage);
        public ICommand SelectedElementCommand => new RelayCommand<TreeItem>(OnTabSelected);
        public ICommand FilteredBoxExecuteEnterCommand => new RelayCommand<string>(FilteredBoxExecuteEnter);

        private async void LoadPage(Guid objectGuid)
        {
            _currentObject = objectGuid;
            IDataObject dataObject = await _objectLoader.Load(objectGuid);
            
            if (TreeItems != null)
                TreeItems.Clear();

            _originTreeItems = await _objectsTreeBuilder.CreateFullTreeAsync(dataObject, TreeItems);

            TreeItems = new ObservableCollection<TreeItem>(_originTreeItems.DeepCopy());
        }

        private async void FilteredBoxExecuteEnter(string parameter)
        {
            TreeItems = await _objectsTreeBuilder.FilteredTreeItemsAsync(parameter, _originTreeItems);
        }

        private void OnTabSelected(TreeItem selectedTab)
        {
            SelectedObjectInfoTabControlVM.UpdateInfoCommand.Execute(selectedTab.DataObject);
        }
    }
}