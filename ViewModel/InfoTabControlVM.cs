using Ascon.Pilot.SDK;
using MyIceLibrary.Command;
using MyIceLibrary.Model;
using MyIceLibrary.ViewModel.Pages;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
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

        #region View Model Properties

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


        #endregion

        public InfoTabControlVM(IObjectsRepository objectsRepository)
        {
            SelectedElementAttributesVM = new AttributesPageVM();
            SelectedElementChildrenPageVM = new ChildrenPageVM(objectsRepository);
        }

        public ICommand SelectedElementCommand => new RelayCommand<TreeItem>(OnTabSelected);

        private void OnTabSelected(TreeItem selectedTab)
        {
            MessageBox.Show("Нажатие");

            //var dataObject = (Ascon.Pilot.SDK.IDataObject) selectedTab;

            //SelectedElementChildrenPageVM.LoadChildrenCommand.Execute(dataObject);
            //SelectedElementAttributesVM.LoadAttributesCommand.Execute(dataObject);
        }
    }
}
