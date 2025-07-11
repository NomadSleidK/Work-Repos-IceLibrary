using Ascon.Pilot.SDK;
using MyIceLibrary.Command;
using MyIceLibrary.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        private ObservableCollection<FileInfo> _currentObjectFilesInfo;
        public ObservableCollection<FileInfo> CurrentObjectFilesInfo
        {
            get => _currentObjectFilesInfo;
            private set
            {
                _currentObjectFilesInfo = value;
                OnPropertyChanged();
            }
        }
        #endregion

        private IObjectModifier _modifier;
        private IDataObject _currentDataObject;
        public FilesPageVM(IObjectModifier modifier)
        {
            _modifier = modifier;
        }

        public ICommand LoadFilesInfoCommand => new RelayCommand<IDataObject>(LoadFilesInfo);
        public ICommand DeleteSelectedCommand => new RelayCommand<IList>(DeleteSelectedItems);

        private void LoadFilesInfo(IDataObject dataObject)
        {
            _currentDataObject = dataObject;

            var filesInfo = new List<FileInfo>();

            var files =  dataObject.Files;

            foreach (var file in files)
            {
                filesInfo.Add(new FileInfo()
                { 
                    Id = file.Id,
                    Name = file.Name,
                    Created = file.Created,
                    Accessed = file.Accessed,
                    Size = file.Size,
                    Md5 = file.Md5,
                });
            }

            CurrentObjectFilesInfo = new ObservableCollection<FileInfo>(filesInfo);
        }

        private void DeleteSelectedItems(IList selectedItems)
        {
            try
            {
                var itemsToRemove = selectedItems?.Cast<IFile>().ToList();

                foreach (var obj in itemsToRemove)
                {
                    if (Guid.TryParse(obj.Id.ToString(), out Guid id))
                    {
                        _modifier.DeleteById(id);
                        _modifier.Apply();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                System.Windows.MessageBox.Show($"Файлы успешно удалены ");
            }
        }
    }
}
