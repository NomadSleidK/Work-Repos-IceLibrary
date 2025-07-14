using Ascon.Pilot.SDK;
using MyIceLibrary.Command;
using MyIceLibrary.Model;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

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

        #region Property
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
        #endregion

        private readonly IObjectsRepository _objectsRepository;
        private readonly IObjectModifier _modifier;
        private readonly ObjectLoader _objectLoader;

        private Guid _currentObject;

        public ChildrenPageVM(IObjectModifier modifier, IObjectsRepository objectsRepository)
        {
            _objectLoader = new ObjectLoader(objectsRepository);
            _modifier = modifier;
            _objectsRepository = objectsRepository;
        }

        public ICommand LoadChildrenCommand => new RelayCommand<Guid>(LoadChildren);
        public ICommand DeleteSelectedCommand => new RelayCommand<IList>(DeleteSelectedItems);

        private async void LoadChildren(Guid dataObjectId)
        {
            try
            {
                _currentObject = dataObjectId;

                CurrentObjectChildren = new ObservableCollection<ObjectChild>();
                var childrenCollection = CurrentObjectChildren;

                var currentObject = await _objectLoader.Load(_currentObject);
                var childrenObjects = await _objectLoader.Load(currentObject.Children);

                foreach (var child in childrenObjects)
                {
                    childrenCollection.Add(new ObjectChild() { Name = child.DisplayName, Id = child.Id });
                }

                CurrentObjectChildren = childrenCollection;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void DeleteSelectedItems(IList selectedItems)
        {
            try
            {
                var itemsToRemove = selectedItems?.Cast<ObjectChild>().ToList();

                foreach (var obj in itemsToRemove)
                {
                    if (Guid.TryParse(obj.Id.ToString(), out Guid id))
                    {
                        _modifier.DeleteById(id);
                    }
                }

                _modifier.Apply();

                CurrentObjectChildren = new ObservableCollection<ObjectChild>();

                LoadChildren(_currentObject);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при удалении файла Id: {ex}");
            }
            finally
            {
                System.Windows.MessageBox.Show($"Файлы успешно удалены ");
            }
        }
    }
}