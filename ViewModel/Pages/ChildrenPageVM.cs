using MyIceLibrary.Command;
using MyIceLibrary.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Ascon.Pilot.SDK;
using System.Linq;

namespace MyIceLibrary.ViewModel.Pages
{
    public class ChildrenPageVM : INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private ObservableCollection<ObjectChild> _currentObjectChildren;
        public ObservableCollection<ObjectChild> CurrentObjectChildren
        {
            get => _currentObjectChildren;
            private set
            {
                _currentObjectChildren = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadChildrenCommand => new RelayCommand<IDataObject>(LoadChildren);

        private IObjectsRepository _objectsRepository;

        public ChildrenPageVM(IObjectsRepository objectsRepository)
        {
            _objectsRepository = objectsRepository;
        }

        private void LoadChildren(IDataObject dataObject)
        {
            try
            {
                var children = dataObject.Children;

                var dataObjects = _objectsRepository.SubscribeObjects(dataObject.Children);
                ObserverFindObjectById observer = new ObserverFindObjectById(AddChildToGrid);
                dataObjects.Subscribe(observer);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void AddChildToGrid(IDataObject dataObject)
        {
            var children = CurrentObjectChildren.ToList();
            children.Add(new ObjectChild() { Name = dataObject.DisplayName, Id = dataObject.Id });

            CurrentObjectChildren = new ObservableCollection<ObjectChild>(children);
        }
    }
}
