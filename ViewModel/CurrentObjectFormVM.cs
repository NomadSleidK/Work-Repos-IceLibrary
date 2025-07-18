using Ascon.Pilot.SDK;
using Ascon.Pilot.Theme.Controls;
using MyIceLibrary.Command;
using MyIceLibrary.Helper;
using MyIceLibrary.View;
using MyIceLibrary.ViewModel.Pages;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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

        #region Properties

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

        private ObjectPathTreePageVM _selectedObjectPathObjectTreePageVM;
        public ObjectPathTreePageVM SelectedObjectPathObjectTreePageVM
        {
            get => _selectedObjectPathObjectTreePageVM;
            private set
            {
                _selectedObjectPathObjectTreePageVM = value;
                OnPropertyChanged();
            }
        }

        private SnapshotsPageVM _selectedObjectSnapshotsPageVM;
        public SnapshotsPageVM SelectedObjectSnapshotsPageVM
        {
            get => _selectedObjectSnapshotsPageVM;
            private set
            {
                _selectedObjectSnapshotsPageVM = value;
                OnPropertyChanged();
            }
        }
        #endregion

        private readonly IObjectsRepository _objectsRepository;
        
        private IDataObject _dataObject;
        private IDataObject _parentDataObject;

        private readonly DialogWindow _currentWindow;
        private readonly IObjectModifier _modifier;
        private readonly IFileProvider _fileProvider;

        public CurrentObjectFormVM(IObjectModifier modifier, IObjectsRepository objectsRepository, IFileProvider fileProvider)
        {
            _currentWindow = WindowHelper.CreateWindowWithUserControl<CurrentObjectUserControl>();
            _currentWindow.DataContext = this;
            _currentWindow.ShowInTaskbar = true;

            _objectsRepository = objectsRepository;
            _fileProvider = fileProvider;
            CurrentObjectMainInfoPageVM = new MainInfoPageVM(objectsRepository);

            SelectedObjectPathObjectTreePageVM = new ObjectPathTreePageVM(objectsRepository, modifier, _fileProvider);
            SelectedObjectInfoTabControlVM = new InfoTabControlVM(modifier, objectsRepository, fileProvider);
            SelectedObjectSnapshotsPageVM = new SnapshotsPageVM(objectsRepository, modifier, fileProvider);
        }

        public ICommand OpenCommand => new RelayCommand<IDataObject>(OpenDialogWindow);
        public ICommand GoToParentCommand => new RelayCommand<object>(_ => GoToParent());
        public ICommand ChangeParentNameLabelContentCommand => new RelayCommand<string>(ChangeParentNameLabelContent);

        private void OpenDialogWindow(IDataObject dataObject)
        {
            _dataObject = dataObject;

            UpdateWindow();
            _currentWindow.Show();
        }

        private void UpdateWindow()
        {
            CurrentObjectMainInfoPageVM.LoadMainInfoCommand.Execute(_dataObject);
            SelectedObjectInfoTabControlVM.UpdateInfoCommand.Execute(_dataObject);
            SelectedObjectPathObjectTreePageVM.LoadPageCommand.Execute(_dataObject.Id);
            SelectedObjectSnapshotsPageVM.LoadFilesInfoCommand.Execute(_dataObject.Id);

            var snap = _dataObject.ActualFileSnapshot;

            ChangeObjectNameLabelContent(_dataObject.DisplayName);
            FindParentObject(_dataObject);
        }

        private void GoToParent()
        {
            CurrentObjectFormVM currentObjectFormVM = new CurrentObjectFormVM(_modifier, _objectsRepository, _fileProvider);
            currentObjectFormVM.OpenCommand.Execute(_parentDataObject);
        }
     
        private void ChangeParentNameLabelContent(string newName)
        {
            ParentObjectName = newName;
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
    }
}