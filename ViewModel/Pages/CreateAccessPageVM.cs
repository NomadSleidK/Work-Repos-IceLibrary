using Ascon.Pilot.SDK;
using Ascon.Pilot.Theme.Controls;
using MyIceLibrary.Command;
using MyIceLibrary.Helper;
using MyIceLibrary.Model;
using MyIceLibrary.View.Pages;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MyIceLibrary.ViewModel.Pages
{
    public class CreateAccessPageVM : INotifyPropertyChanged
    {
        public event Action OnAccessCreated;

        #region Property Changed
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Property
        private ObservableCollection<TreeItem> _treeItems;
        public ObservableCollection<TreeItem> TreeItems
        {
            get => _treeItems;
            set
            {
                _treeItems = value;
                OnPropertyChanged();
            }
        }

        private TreeItem _selectedUnit;

        private string _selectedUnitName;
        public string SelectedUnitName
        {
            get
            {
                return _selectedUnitName;
            }
            set
            {
                _selectedUnitName = value;
                OnPropertyChanged();
            }
        }

        private bool _isShareChecked;
        public bool IsShareChecked
        {
            get
            {
                return _isShareChecked;
            }
            set
            {
                _isShareChecked = value;
                OnPropertyChanged();
            }
        }

        private bool _isAgreementChecked;
        public bool IsAgreementChecked
        {
            get
            {
                return _isAgreementChecked;
            }
            set
            {
                _isAgreementChecked = value;
                OnPropertyChanged();
            }
        }

        private bool _isFreezeChecked;
        public bool IsFreezeChecked
        {
            get
            {
                return _isFreezeChecked;
            }
            set
            {
                _isFreezeChecked = value;
                OnPropertyChanged();
            }
        }

        private bool _isCreateChecked;
        public bool IsCreateChecked
        {
            get
            {
                return _isCreateChecked;
            }
            set
            {
                _isCreateChecked = value;
                OnPropertyChanged();
            }
        }

        private bool _isEditChecked;
        public bool IsEditChecked
        {
            get
            {
                return _isEditChecked;
            }
            set
            {
                _isEditChecked = value;
                OnPropertyChanged();
            }
        }

        private bool _isViewChecked;
        public bool IsViewChecked
        {
            get
            {
                return _isViewChecked;
            }
            set
            {
                _isViewChecked = value;
                OnPropertyChanged();
            }
        }


        private DateTime _selectedValidThroughDate;
        public DateTime SelectedValidThroughDate
        {
            get => _selectedValidThroughDate;
            set
            {
                _selectedValidThroughDate = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<InheritanceType> _inheritanceSource;
        public ObservableCollection<InheritanceType> InheritanceSource
        {
            get => _inheritanceSource;
            set
            {
                _inheritanceSource = value;
                OnPropertyChanged();
            }
        }

        private InheritanceType _selectedInheritance;
        public InheritanceType SelectedInheritance
        {
            get => _selectedInheritance;
            set
            {
                _selectedInheritance = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<AccessTypeInfo> _accessTypeSource;
        public ObservableCollection<AccessTypeInfo> AccessTypeSource
        {
            get => _accessTypeSource;
            set
            {
                _accessTypeSource = value;
                OnPropertyChanged();
            }
        }

        private AccessTypeInfo _selectedAccessType;
        public AccessTypeInfo SelectedAccessType
        {
            get => _selectedAccessType;
            set
            {
                _selectedAccessType = value;
                OnPropertyChanged();
            }
        }
        #endregion

        private readonly ObjectLoader _objectLoader;
        private readonly IObjectModifier _objectModifier;

        private readonly ObjectsTreeBuilder _treeBuilder;

        private DialogWindow _dialogWindowAddAccess;

        private Guid _currentObjectGuid;
        private ObservableCollection<TreeItem> _originalTree;

        public CreateAccessPageVM(IObjectsRepository objectsRepository, IObjectModifier objectModifier)
        {
            _objectModifier = objectModifier;

            _treeBuilder = new ObjectsTreeBuilder(objectsRepository);
            _objectLoader = new ObjectLoader(objectsRepository);

            InheritanceSource = new ObservableCollection<InheritanceType>()
            {
                new InheritanceType(){ Type = AccessInheritance.None, Name = "Без наследования"},
                new InheritanceType(){ Type = AccessInheritance.InheritUntilSecret, Name = "Наследовать для общих"},
                new InheritanceType(){ Type = AccessInheritance.InheritWholeSubtree, Name = "Наследовать для всех"},
            };

            SelectedInheritance = InheritanceSource[0];

            AccessTypeSource = new ObservableCollection<AccessTypeInfo>()
            {
                new AccessTypeInfo(){ Type = AccessType.Allow, Name = "Разрешать"},
                new AccessTypeInfo(){ Type = AccessType.Deny, Name = "Запрещать"},
            };

            SelectedAccessType = AccessTypeSource[0];

            SelectedValidThroughDate = DateTime.MaxValue;
        }

        public ICommand OpenWindowCommand => new RelayCommand<Guid>(OpenWindow);
        public ICommand SelectedElementCommand => new RelayCommand<TreeItem>(SelectedElementChange);
        public ICommand FilteredBoxExecuteEnterCommand => new RelayCommand<string>(FilteredBoxExecuteEnter);
        public ICommand CreateButtonClickCommand => new RelayCommand<string>(_ => CreateButtonClick(), o => _selectedUnit != null && GetSelectedAccessLevel() != AccessLevel.None );

        private void OpenWindow(Guid objectGuid)
        {
            _currentObjectGuid = objectGuid;

            _dialogWindowAddAccess = WindowHelper.CreateWindowWithUserControl<CreateAccessPage>(this, true, "Создание доступа");

            _dialogWindowAddAccess.Show();

            LoadAccessTree();
        }

        private void LoadAccessTree()
        {
            _originalTree = _treeBuilder.CreateOrganisationUnitTreeTopToButtomAsync();
            TreeItems = new ObservableCollection<TreeItem>(_originalTree.DeepCopy());
        }

        private async void FilteredBoxExecuteEnter(string parameter)
        {
            var result = await _treeBuilder.FilteredTreeItemsAsync(parameter, _originalTree);
            TreeItems = result;
        }

        private void SelectedElementChange(TreeItem selectedTab)
        {
            if (selectedTab != null)
            {
                _selectedUnit = selectedTab;
                SelectedUnitName = selectedTab.Name;
            }
        }

        private AccessLevel GetSelectedAccessLevel()
        {
            AccessLevel level = AccessLevel.None;

            if (IsViewChecked) level |= AccessLevel.View;
            if (IsEditChecked) level |= AccessLevel.Edit;
            if (IsCreateChecked) level |= AccessLevel.Create;
            if (IsFreezeChecked) level |= AccessLevel.Freeze;
            if (IsAgreementChecked) level |= AccessLevel.Agreement;
            if (IsShareChecked) level |= AccessLevel.Share;

            return level;
        }

        private async void CreateButtonClick()
        {
            var orgUnitId = ((IOrganisationUnit)_selectedUnit.DataObject).Id;
            var accessLevel = GetSelectedAccessLevel();
            var validThrough = SelectedValidThroughDate;
            var inheritance = SelectedInheritance.Type;
            var accessType = SelectedAccessType.Type;


            try
            {
                var objectInfo = await _objectLoader.Load(_currentObjectGuid);
                var builder = _objectModifier.EditById(objectInfo.Id);

                builder.AddAccessRecords(
                    orgUnitId,
                    accessLevel,
                    validThrough,
                    inheritance,
                    accessType);

                _objectModifier.Apply();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Ошибка: " + ex.Message);
            }
            finally
            {
                System.Windows.MessageBox.Show("Доступ успешно создан");

                OnAccessCreated?.Invoke();
                _dialogWindowAddAccess.Close();
            }
        }
    }
}