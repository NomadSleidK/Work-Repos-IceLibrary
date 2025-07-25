﻿using Ascon.Pilot.SDK;
using Ascon.Pilot.Theme.Controls;
using MyIceLibrary.Command;
using MyIceLibrary.Helper;
using MyIceLibrary.Model;
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

        private PeoplesPageVM _basePeoplesPageVM;
        public PeoplesPageVM BasePeoplesPageVM
        {
            get => _basePeoplesPageVM;
            set
            {
                _basePeoplesPageVM = value;
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
            BasePeoplesPageVM = new PeoplesPageVM(objectsRepository);

            _currentWindow = WindowHelper.CreateWindowWithUserControl<BaseStructureWindow>(this, true, "Структура базы");
            _currentWindow.DataContext = this;
        }


        private void OpenWindow()
        {
            BaseHierarchyPageVM.LoadHierarchyCommand.Execute(null);
            BaseTypesPageVM.LoadTypesCommand.Execute(null);
            BasePeoplesPageVM.LoadPeopleInfoCommand.Execute(null);

            _currentWindow.Show();
        }
    }
}