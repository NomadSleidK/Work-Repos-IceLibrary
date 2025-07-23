using Ascon.Pilot.SDK;
using MyIceLibrary.Command;
using MyIceLibrary.ViewModel.Pages;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MyIceLibrary.ViewModel
{
    public class InfoTabControlVM : INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Properties

        private AttributesPageVM _selectedElementAttributesVM;
        public AttributesPageVM SelectedElementAttributesVM
        {
            get => _selectedElementAttributesVM;
            private set
            {
                _selectedElementAttributesVM = value;
                OnPropertyChanged();
            }
        }

        private ChildrenPageVM _selectedElementChildrenPageVM;
        public ChildrenPageVM SelectedElementChildrenPageVM
        {
            get => _selectedElementChildrenPageVM;
            private set
            {
                _selectedElementChildrenPageVM = value;
                OnPropertyChanged();
            }
        }

        private TypePageVM _selectedElementTypePageVM;
        public TypePageVM SelectedElementTypePageVM
        {
            get => _selectedElementTypePageVM;
            private set
            {
                _selectedElementTypePageVM = value;
                OnPropertyChanged();
            }
        }

        private CreatorPageVM _selectedElementCreatorPageVM;
        public CreatorPageVM SelectedElementCreatorPageVM
        { 
            get => _selectedElementCreatorPageVM;
            private set
            {
                _selectedElementCreatorPageVM = value;
                OnPropertyChanged();
            }
        }


        private FilesPageVM _selectedElementFilesPageVM;
        public FilesPageVM SelectedElementFilesPageVM
        {
            get => _selectedElementFilesPageVM;
            private set
            {
                _selectedElementFilesPageVM = value;
                OnPropertyChanged();
            }
        }

        private AccessLevelPageVM _accessPageVM;
        public AccessLevelPageVM SelectedElementAccessLevelPageVM 
        {
            get => _accessPageVM;
            private set
            {
                _accessPageVM = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public InfoTabControlVM(IObjectModifier modifier, IObjectsRepository objectsRepository, IFileProvider fileProvider)
        {
            SelectedElementAttributesVM = new AttributesPageVM(objectsRepository);
            SelectedElementChildrenPageVM = new ChildrenPageVM(modifier, objectsRepository);
            SelectedElementTypePageVM = new TypePageVM(objectsRepository);
            SelectedElementCreatorPageVM = new CreatorPageVM(objectsRepository);
            SelectedElementFilesPageVM = new FilesPageVM(objectsRepository, modifier, fileProvider);
            SelectedElementAccessLevelPageVM = new AccessLevelPageVM(objectsRepository);
        }

        public ICommand UpdateInfoCommand => new RelayCommand<Guid>(UpdateInfo);

        private void UpdateInfo(Guid objectGuid)
        {
            SelectedElementChildrenPageVM.LoadChildrenCommand.Execute(objectGuid);
            SelectedElementAttributesVM.LoadAttributesCommand.Execute(objectGuid);
            SelectedElementTypePageVM.LoadTypeInfoCommand.Execute(objectGuid);
            SelectedElementCreatorPageVM.LoadCreatorInfoCommand.Execute(objectGuid);
            SelectedElementFilesPageVM.LoadFilesInfoCommand.Execute(objectGuid);
            SelectedElementAccessLevelPageVM.LoadAccessLevelCommand.Execute(objectGuid);
        }
    }
}
