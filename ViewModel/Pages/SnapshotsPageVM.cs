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
using System.Runtime.InteropServices;
using System.Windows.Documents;
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
                //System.Windows.MessageBox.Show($"{_isSelected}");
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
        private readonly IObjectModifier _objectModifier;
        private readonly IFileProvider _fileProvider;
        private readonly ObjectLoader _objectLoader;
        private readonly FileManager _fileManager;
        private readonly ObjectsTreeBuilder _treeBuilder;

        private Guid _currentObjectGuid;
        private ObservableCollection<TreeItem> _originTree;
        public SnapshotsPageVM(IObjectsRepository objectsRepository, IObjectModifier objectModifier, IFileProvider fileProvider)
        {
            _objectsRepository = objectsRepository;
            _objectModifier = objectModifier;
            _fileProvider = fileProvider;
            _objectLoader = new ObjectLoader(_objectsRepository);
            _fileManager = new FileManager(_objectsRepository, objectModifier, fileProvider);
            _treeBuilder = new ObjectsTreeBuilder(objectsRepository);
        }

        public ICommand LoadFilesInfoCommand => new RelayCommand<Guid>(UpdateFilesInfo);
        public ICommand DownloadFilesCommand => new RelayCommand<TreeItem>(DownloadFiles);
        public ICommand LoadObjectInfoCommand => new RelayCommand<TreeItem>(LoadObjectInfoToGrid);
        //public ICommand DeleteFileCommand => new RelayCommand<TreeItem>(DeleteFile);
        public ICommand FilteredBoxExecuteEnterCommand => new RelayCommand<string>(FilteredBoxExecuteEnter);

        private async void UpdateFilesInfo(Guid objectGuid)
        {
            _currentObjectGuid = objectGuid;

            var dataObject = await _objectLoader.Load(objectGuid);
            var snapshots = dataObject.PreviousFileSnapshots;

            _originTree = _treeBuilder.CreateSnapshotsTree(snapshots.ToArray());
            TreeItems = _originTree.DeepCopy();
        }

        private async void FilteredBoxExecuteEnter(string parameter)
        {
            TreeItems = await _treeBuilder.FilteredTreeItemsAsync(parameter, _originTree);
        }

        public void DownloadFiles(TreeItem selectedItems)
        {
            var fileToRemove = selectedItems.DataObject as IFile;

            if (fileToRemove != null)
            {
                _fileManager.DownloadFiles(new Guid[] { fileToRemove.Id }, _currentObjectGuid);
            }
        }

        //public void DeleteFile(TreeItem selectedItems)
        //{
        //    var fileToRemove = selectedItems.DataObject as IFile;

        //    if (fileToRemove != null)
        //    {
        //        _fileManager.DeleteFiles(new Guid[] { fileToRemove.Id }, _currentObjectGuid);
        //    }

        //    UpdateFilesInfo(_currentObjectGuid);
        //}

        private void LoadObjectInfoToGrid(TreeItem selectedItems)
        {
            var info = new List<CurrentObjectInfo>();

            if (selectedItems.DataObject as IFile != null)
            {
                var file = selectedItems.DataObject as IFile;

                info.Add(new CurrentObjectInfo() { Name = "Имя", Value = file.Name});
                info.Add(new CurrentObjectInfo() { Name = "Id", Value = file.Id });
                info.Add(new CurrentObjectInfo() { Name = "Размер", Value = file.Size });
                info.Add(new CurrentObjectInfo() { Name = "Md5", Value = file.Md5 });
                info.Add(new CurrentObjectInfo() { Name = "Modified", Value = file.Modified });
                info.Add(new CurrentObjectInfo() { Name = "Создан", Value = file.Created.ToString("dd-MM-yyyy HH:mm:ss") });
                info.Add(new CurrentObjectInfo() { Name = "Accessed", Value = file.Accessed.ToString("dd-MM-yyyy HH:mm:ss") });
            }
            else if (selectedItems.DataObject as IFilesSnapshot != null)
            {
                var snapshot = selectedItems.DataObject as IFilesSnapshot;

                info.Add(new CurrentObjectInfo() { Name = "Дата создания", Value = snapshot.Created.ToString("dd-MM-yyyy HH:mm:ss") });
                info.Add(new CurrentObjectInfo() { Name = "Создатель", Value = _objectsRepository.GetPerson(snapshot.CreatorId).DisplayName });
                info.Add(new CurrentObjectInfo() { Name = "Reason", Value = snapshot.Reason });
                info.Add(new CurrentObjectInfo() { Name = "Колличество файлов", Value = snapshot.Files.Count });
            }

            SelectedObjectInfo = new ObservableCollection<CurrentObjectInfo>(info);
        }
    }
}
