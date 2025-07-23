using Ascon.Pilot.SDK;
using MyIceLibrary.Command;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MyIceLibrary.ViewModel.Pages
{
    public class FilesPageVM : INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Property
        private ObservableCollection<Model.FileInfo> _currentObjectFilesInfo;
        public ObservableCollection<Model.FileInfo> CurrentObjectFilesInfo
        {
            get => _currentObjectFilesInfo;
            private set
            {
                _currentObjectFilesInfo = value;
                OnPropertyChanged();
            }
        }
        #endregion

        private readonly IObjectsRepository _objectsRepository;
        private readonly IObjectModifier _objectModifier;
        private readonly IFileProvider _fileProvider;
        private readonly ObjectLoader _objectLoader;

        private Guid _currentObjectGuid;

        public FilesPageVM(IObjectsRepository objectsRepository, IObjectModifier objectModifier, IFileProvider fileProvider)
        {
            _objectsRepository = objectsRepository;
            _objectModifier = objectModifier;
            _fileProvider = fileProvider;
            _objectLoader = new ObjectLoader(_objectsRepository);
        }

        public ICommand LoadFilesInfoCommand => new RelayCommand<Guid>(UpdateFilesInfo);
        public ICommand DeleteFilesCommand => new RelayCommand<IList>(DeleteFiles, o => o != null && o.Count > 0);
        public ICommand DownloadFilesCommand => new RelayCommand<IList>(DownloadFiles, o => o != null && o.Count > 0);

        private async void UpdateFilesInfo(Guid objectGuid)
        {
            _currentObjectGuid = objectGuid;

            var dataObject = await _objectLoader.Load(objectGuid);
            var filesInfo = new List<Model.FileInfo>();

            var files =  dataObject.Files;

            foreach (var file in files)
            {
                filesInfo.Add(new Model.FileInfo()
                { 
                    Id = file.Id,
                    Name = file.Name,
                    Created = file.Created,
                    Accessed = file.Accessed,
                    Size = file.Size,
                    Md5 = file.Md5,
                });
            }

            CurrentObjectFilesInfo = new ObservableCollection<Model.FileInfo>(filesInfo);
        }

        private void DeleteFiles(IList selectedItems)
        {
            var filesToRemove = selectedItems?.Cast<Model.FileInfo>().ToList();

            foreach (var file in filesToRemove)
            {
                var builder = _objectModifier.EditById(_currentObjectGuid);
                builder.RemoveFile(file.Id);
                _objectModifier.Apply();
            }

            UpdateFilesInfo(_currentObjectGuid);
        }

        private async void DownloadFiles(IList selectedItems)
        {     
            if (selectedItems.Count > 0)
            {
                var filesInfo = selectedItems?.Cast<Model.FileInfo>().ToList();
                var files = new List<IFile>();

                var allFiles = (await _objectLoader.Load(_currentObjectGuid)).Files;

                foreach (var fileInfo in filesInfo)
                {
                    files.Add(allFiles.SingleOrDefault(n => n.Id == fileInfo.Id));
                }

                using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    if (result != System.Windows.Forms.DialogResult.OK) return;

                    foreach (var file in files)
                    {
                        using (var stream = _fileProvider.OpenRead(file))
                        {
                            try
                            {
                                using (FileStream output = new FileStream(Path.Combine(dialog.SelectedPath, file.Name),
                                           FileMode.Create))
                                {
                                    stream.CopyTo(output);
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK,
                                    System.Windows.MessageBoxImage.Error);
                            }
                        }
                    }
                    System.Windows.MessageBox.Show($"Файлы ({selectedItems.Count}) успешно сканы в папку \n {dialog.SelectedPath}");
                }
            }     
        }
    }
}