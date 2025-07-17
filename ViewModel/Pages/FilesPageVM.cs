using Ascon.Pilot.SDK;
using Microsoft.Win32;
using MyIceLibrary.Command;
using MyIceLibrary.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        private readonly ObjectLoader _objectLoader;

        private Guid _currentObjectGuid;

        public FilesPageVM(IObjectsRepository objectsRepository, IObjectModifier objectModifier)
        {
            _objectsRepository = objectsRepository;
            _objectModifier = objectModifier;
            _objectLoader = new ObjectLoader(_objectsRepository);
        }

        public ICommand LoadFilesInfoCommand => new RelayCommand<Guid>(UpdateFilesInfo);
        public ICommand DeleteFilesCommand => new RelayCommand<IList>(DeleteFiles);

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

        private async Task DownloadFiles(IList selectedItems)
        {
            //var filesToDownload = selectedItems?.Cast<Model.FileInfo>().ToList();
            ////var files = await _objectLoader.Load();
            
            //foreach (var file in filesToDownload)
            //{
                
            //    if (file == null) return;

            //    var dlg = new SaveFileDialog();
            //    dlg.DefaultExt = Path.GetExtension(file.Name);
            //    dlg.FileName = file.Name;
            //    if (dlg.ShowDialog() != true) return;
            //    using (var stream = _fileProvider.OpenRead(file))
            //    {
            //        try
            //        {
            //            using (FileStream output = new FileStream(dlg.FileName, FileMode.Create))
            //            {
            //                stream.CopyTo(output);
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK,
            //                System.Windows.MessageBoxImage.Error);
            //        }
            //    }
            //}       

            //using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            //{
            //    var filesToRemove = selectedItems?.Cast<Model.FileInfo>().ToList();
            //    var files = await _objectLoader.Load()

            //    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            //    if (result != System.Windows.Forms.DialogResult.OK) return;

            //    foreach (var file in _files)
            //    {
            //        using (var stream = _fileProvider.OpenRead(file))
            //        {
            //            try
            //            {
            //                using (FileStream output = new FileStream(Path.Combine(dialog.SelectedPath, file.Name),
            //                           FileMode.Create))
            //                {
            //                    stream.CopyTo(output);
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK,
            //                    System.Windows.MessageBoxImage.Error);
            //            }
            //        }
            //    }           
            //}
        }
    }
}