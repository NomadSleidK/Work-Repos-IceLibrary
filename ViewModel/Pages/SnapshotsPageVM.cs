using Ascon.Pilot.SDK;
using MyIceLibrary.Command;
using MyIceLibrary.Helper;
using MyIceLibrary.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MyIceLibrary.ViewModel.Pages
{
    public class SnapshotsPageVM : INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Property
        private ObservableCollection<TreeItem> _treeItems;
        public ObservableCollection<TreeItem> TreeItems
        {
            get => _treeItems;
            private set
            {
                _treeItems = value;
                OnPropertyChanged();
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            private set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        private bool _downloadButtonEnabled;
        public bool DownloadButtonEnabled
        {
            get => _downloadButtonEnabled;
            private set
            {
                _downloadButtonEnabled = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<CurrentObjectInfo> _selectedObjectInfo;
        public ObservableCollection<CurrentObjectInfo> SelectedObjectInfo
        {
            get => _selectedObjectInfo;
            private set
            {
                _selectedObjectInfo = value;
                OnPropertyChanged();
            }
        }
        #endregion

        private readonly IObjectsRepository _objectsRepository;
        private readonly ObjectLoader _objectLoader;
        private readonly FileManager _fileManager;
        private readonly ObjectsTreeBuilder _treeBuilder;

        private Guid _currentObjectGuid;
        private ObservableCollection<TreeItem> _originTree;

        public SnapshotsPageVM(IObjectsRepository objectsRepository, IObjectModifier objectModifier, IFileProvider fileProvider)
        {
            _objectsRepository = objectsRepository;
            _objectLoader = new ObjectLoader(_objectsRepository);
            _fileManager = new FileManager(_objectsRepository, objectModifier, fileProvider);
            _treeBuilder = new ObjectsTreeBuilder(objectsRepository);
        }

        public ICommand LoadFilesInfoCommand => new RelayCommand<Guid>(UpdateFilesInfo);
        public ICommand DownloadFilesCommand => new RelayCommand<TreeItem>(DownloadFiles, o => o != null && o.DataObject as IFile != null);
        public ICommand LoadObjectInfoCommand => new RelayCommand<TreeItem>(LoadObjectInfoToGrid);
        public ICommand FilteredBoxExecuteEnterCommand => new RelayCommand<string>(FilteredBoxExecuteEnter);

        private async void UpdateFilesInfo(Guid objectGuid)
        {
            _currentObjectGuid = objectGuid;

            var dataObject = await _objectLoader.Load(objectGuid);
            var snapshots = dataObject.PreviousFileSnapshots;

            _originTree = _treeBuilder.CreateSnapshotsTree(snapshots.ToArray());
            TreeItems = new ObservableCollection<TreeItem>(_originTree);
        }

        private async void FilteredBoxExecuteEnter(string parameter)
        {
            TreeItems = await _treeBuilder.FilteredTreeItemsAsync(parameter, _originTree);
        }

        public void DownloadFiles(TreeItem selectedItems)
        {
            var fileToRemove = selectedItems?.DataObject as IFile;

            if (fileToRemove != null)
            {
                _fileManager.DownloadFiles(new Guid[] { fileToRemove.Id }, _currentObjectGuid);
            }
        }

        private void LoadObjectInfoToGrid(TreeItem selectedItem)
        {
            var info = new List<CurrentObjectInfo>();

            if (selectedItem?.DataObject as IFile != null)
            {
                var file = selectedItem.DataObject as IFile;

                info.Add(new CurrentObjectInfo() { Name = "Name", Value = file.Name});
                info.Add(new CurrentObjectInfo() { Name = "Id", Value = file.Id });
                info.Add(new CurrentObjectInfo() { Name = "Size", Value = file.Size });
                info.Add(new CurrentObjectInfo() { Name = "Md5", Value = file.Md5 });
                info.Add(new CurrentObjectInfo() { Name = "Modified", Value = file.Modified.ToLocalTime().ToString("dd-MM-yyyy HH:mm:ss") });
                info.Add(new CurrentObjectInfo() { Name = "Created", Value = file.Created.ToLocalTime().ToString("dd-MM-yyyy HH:mm:ss") });
                info.Add(new CurrentObjectInfo() { Name = "Accessed", Value = file.Accessed.ToLocalTime().ToString("dd-MM-yyyy HH:mm:ss") });
            }
            else if (selectedItem?.DataObject as IFilesSnapshot != null)
            {
                var snapshot = selectedItem.DataObject as IFilesSnapshot;

                info.Add(new CurrentObjectInfo() { Name = "Created", Value = snapshot.Created.ToLocalTime().ToString("dd-MM-yyyy HH:mm:ss") });
                info.Add(new CurrentObjectInfo() { Name = "CreatorId", Value = _objectsRepository.GetPerson(snapshot.CreatorId).DisplayName });
                info.Add(new CurrentObjectInfo() { Name = "Reason", Value = snapshot.Reason });
                info.Add(new CurrentObjectInfo() { Name = "Files Count", Value = snapshot.Files.Count });
            }

            SelectedObjectInfo = new ObservableCollection<CurrentObjectInfo>(info);
        }
    }
}