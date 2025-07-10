using Ascon.Pilot.SDK;
using Ascon.Pilot.Theme.Controls;
using MyIceLibrary.Command;
using MyIceLibrary.Helper;
using MyIceLibrary.Model;
using MyIceLibrary.View;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MyIceLibrary.ViewModel
{
    internal class MainMenuDataGridVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<AttributeValues> _attributesValues;
        public ObservableCollection<AttributeValues> AttributesValues
        {
            get => _attributesValues;
            set
            {
                _attributesValues = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<MainInfoValue> _mainInfoValues;
        public ObservableCollection<MainInfoValue> MainInfoValues
        {
            get => _mainInfoValues;
            set
            {
                _mainInfoValues = value;
                OnPropertyChanged();
            }
        }

        private bool _deleteButtonEnabled;
        public bool DeleteButtonEnabled
        {
            get => _deleteButtonEnabled;
            set
            {
                _deleteButtonEnabled = value;
                OnPropertyChanged();
            }
        }

        private System.Windows.Visibility _deleteButtonVisibility;
        public System.Windows.Visibility DeleteButtonVisibility
        {
            get => _deleteButtonVisibility;
            set
            {
                _deleteButtonVisibility = value;
                OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private IObjectModifier _modifier;
        private IObjectsRepository _objectsRepository;
        private IDataObject[] _dataObjects;

        private IPilotDialogService _pilotDialogService;

        private DialogWindow _mainWindow;

        public MainMenuDataGridVM(IObjectModifier modifier, IObjectsRepository objectsRepository, IPilotDialogService pilotDialogService)
        {
            _modifier = modifier;
            _objectsRepository = objectsRepository;
            _pilotDialogService = pilotDialogService;
        }

        public ICommand OpenFormCommand => new RelayCommand<IDataObject[]>(OpenForm);
        public ICommand DeleteSelectedCommand => new RelayCommand<IList>(DeleteSelectedItems);
        public ICommand RowDoubleClickCommand => new RelayCommand<MainInfoValue>(OnRowDoubleClick);
        public ICommand LoadAttributesCommand => new RelayCommand<object>(_ => LoadAttributes());
        public ICommand LoadMainInfoCommand => new RelayCommand<object>(_ => LoadMainInfo());
        public ICommand CheckCurrentPersonIsAdminCommand => new RelayCommand<object>(_ => CheckCurrentPersonIsAdmin());

        private void OpenForm(IDataObject[] dataObjects)
        {            
            _dataObjects = dataObjects;

            _mainWindow = WindowHelper.CreateWindowWithUserControl<MainUserControl>();
            _mainWindow.DataContext = this;

            LoadAttributesCommand.Execute(null);
            LoadMainInfoCommand.Execute(null);
            CheckCurrentPersonIsAdminCommand.Execute(null);

            _mainWindow.Show();
        }

        private void LoadAttributes()
        {
            try
            {
                var attributesDictionaries = new List<IDictionary<string, object>>();

                foreach (var dataObject in _dataObjects)
                {
                    attributesDictionaries.Add(dataObject.Attributes);
                }

                var attributes = DataGridHelper.BringTogetherAllAttributes(attributesDictionaries.ToArray()).ToArray();

                AttributesValues = new ObservableCollection<AttributeValues>(attributes);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void LoadMainInfo()
        {
            try
            {
                MainInfoValues = DataGridHelper.GetMainInfoObservableCollectionByObjects(_dataObjects);
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
                var itemsToRemove = selectedItems?.Cast<MainInfoValue>().ToList();

                foreach (var obj in itemsToRemove)
                {
                    if (Guid.TryParse(obj.ID?.ToString(), out Guid id))
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
                System.Windows.MessageBox.Show($"Объекты успешно удалены ");
                _mainWindow?.Close();
            }
        }

        private void CheckCurrentPersonIsAdmin()
        {
            bool isAdmin = _objectsRepository.GetCurrentPerson().IsAdmin;
            
            if (isAdmin)
            {
                DeleteButtonVisibility = System.Windows.Visibility.Visible;
            }
            else
            {
                DeleteButtonVisibility = System.Windows.Visibility.Hidden;
            }

            DeleteButtonEnabled = _objectsRepository.GetCurrentPerson().IsAdmin;
        }

        private void OnRowDoubleClick(MainInfoValue selectedItem)
        {
            try
            {
                Guid[] guids = new Guid[] { Guid.Parse(selectedItem.ID.ToString()) };

                var dataObjects = _objectsRepository.SubscribeObjects(guids);
                ObserverFindObjectById observer = new ObserverFindObjectById(OnObjectsFind);
                dataObjects.Subscribe(observer);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void OnObjectsFind(IDataObject obj)
        {
            CurrentObjectFormVM currentObjectFormVM = new CurrentObjectFormVM(_objectsRepository);

            currentObjectFormVM.OpenCommand.Execute(obj);
        }
    }
}