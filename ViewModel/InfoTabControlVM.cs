using Ascon.Pilot.SDK;
using MyIceLibrary.Command;
using MyIceLibrary.Model;
using MyIceLibrary.ViewModel.Pages;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
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
        #endregion

        public InfoTabControlVM(IObjectModifier modifier, IObjectsRepository objectsRepository)
        {
            SelectedElementAttributesVM = new AttributesPageVM();
            SelectedElementChildrenPageVM = new ChildrenPageVM(objectsRepository);
            SelectedElementTypePageVM = new TypePageVM();
            SelectedElementCreatorPageVM = new CreatorPageVM();
            SelectedElementFilesPageVM = new FilesPageVM(modifier);
        }

        public ICommand UpdateInfoCommand => new RelayCommand<IDataObject>(UpdateInfo);

        private void UpdateInfo(IDataObject dataObject)
        {
            SelectedElementChildrenPageVM.LoadChildrenCommand.Execute(dataObject);
            SelectedElementAttributesVM.LoadAttributesCommand.Execute(dataObject);
            SelectedElementTypePageVM.LoadTypeInfoCommand.Execute(dataObject);
            SelectedElementCreatorPageVM.LoadCreatorInfoCommand.Execute(dataObject);
            SelectedElementFilesPageVM.LoadFilesInfoCommand.Execute(dataObject);
        }
    }
}
