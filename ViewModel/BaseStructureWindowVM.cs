using Ascon.Pilot.SDK;
using Ascon.Pilot.Theme.Controls;
using MyIceLibrary.Command;
using MyIceLibrary.Helper;
using MyIceLibrary.View;
using MyIceLibrary.ViewModel.Pages;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MyIceLibrary.ViewModel
{
    public class BaseStructureWindowVM : INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region View Model Properties
        private HierarchyPageVM _baseHierarchyPageVM;
        public HierarchyPageVM BaseHierarchyPageVM
        {
            get => _baseHierarchyPageVM;
            set
            {
                _baseHierarchyPageVM = value;
                OnPropertyChanged();
            }
        }

        private TypesPageVM _baseTypesPageVM;
        public TypesPageVM BaseTypesPageVM
        {
            get => _baseTypesPageVM;
            set
            {
                _baseTypesPageVM = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public ICommand OpenDialogCommand => new RelayCommand<object>(_ => OpenWindow());

        private readonly IObjectsRepository _objectsRepository;
        private readonly DialogWindow _currentWindow;

        public BaseStructureWindowVM(IObjectsRepository objectsRepository)
        {
            _objectsRepository = objectsRepository;

            BaseHierarchyPageVM = new HierarchyPageVM(objectsRepository);
            BaseTypesPageVM = new TypesPageVM(objectsRepository);

            _currentWindow = WindowHelper.CreateWindowWithUserControl<BaseStructureWindow>();
            _currentWindow.DataContext = this;
        }

        private void OpenWindow()
        {
            BaseHierarchyPageVM.LoadHierarchyCommand.Execute(null);
            BaseTypesPageVM.LoadTypesCommand.Execute(null);

            _currentWindow.Show();
        }
    }
}