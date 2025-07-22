using Ascon.Pilot.SDK;
using Ascon.Pilot.Theme.Controls;
using MathNet.Numerics.Distributions;
using MyIceLibrary.Command;
using MyIceLibrary.Extensions;
using MyIceLibrary.Helper;
using MyIceLibrary.Model;
using MyIceLibrary.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyIceLibrary.ViewModel.Pages
{
    public class AccessTransferVM : INotifyPropertyChanged
    {
        #region Property Changed
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Property
        private ObservableCollection<AccessCheckBox> _accessItems;
        public ObservableCollection<AccessCheckBox> AccessItems
        {
            get => _accessItems;
            set
            {
                _accessItems = value;

                UpdateButtonsEnabled();
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ParentInfo> _parentsInfo;
        public ObservableCollection<ParentInfo> ParentObjects
        {
            get => _parentsInfo;
            set
            {
                _parentsInfo = value;
                OnPropertyChanged();
            }
        }

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

        private ParentInfo _selectedParent;
        public ParentInfo SelectedParent
        {
            get => _selectedParent;
            set
            {
                _selectedParent = value;

                CheckOnElementsOnTree(_treeItems[0], false);
                TreeItems = new ObservableCollection<TreeItem>( _treeItems);

                UpdateButtonsEnabled();
                OnPropertyChanged();
            }
        }

        private bool _copyDeleteEnabled;
        public bool CopyDeleteButtonsEnabled
        {
            get => _copyDeleteEnabled;
            set
            {
                _copyDeleteEnabled = value;

                OnPropertyChanged();
            }
        }
        #endregion

        private readonly IObjectsRepository _objectsRepository;
        private readonly IObjectModifier _objectModifier;

        private readonly ObjectLoader _objectLoader;
        private readonly AccessLoader _accessLoader;
        private readonly AccessModifier _accessModifier;

        private readonly CreateAccessPageVM _createAccessPage;

        private DialogWindow _dialogWindow;

        private Guid _currentObjectGuid;

        public AccessTransferVM(IObjectsRepository objectsRepository, IObjectModifier objectModifier)
        {
            _objectsRepository = objectsRepository;
            _objectModifier = objectModifier;

            _objectLoader = new ObjectLoader(objectsRepository);
            _accessLoader = new AccessLoader(objectsRepository);
            _accessModifier = new AccessModifier(objectsRepository, objectModifier);

            _createAccessPage = new CreateAccessPageVM(objectsRepository, objectModifier);
            _createAccessPage.OnAccessCreated += UpdateAccessItems;
        }

        public ICommand OpenWindowCommand => new RelayCommand<Guid>(OpenWindowAsync);
        public ICommand CopyAccessToParentObjectsCommand => new RelayCommand<object>(_ => CopyAccessToParentObjectsAsync());
        public ICommand RemoveAccessFromParentObjectsCommand => new RelayCommand<object>(_ => RemoveAccessFromParentObjectsAsync());
        public ICommand OpenAddAccessDialogWindowCommand => new RelayCommand<object>(_ => OpenAddAccessDialogWindow());
        public ICommand UpdateCopeDeleteButtonsEnabledCommand => new RelayCommand<object>(_ => UpdateButtonsEnabled());

        private async void OpenWindowAsync(Guid objectGuid)
        {
            _dialogWindow = WindowHelper.CreateWindowWithUserControl<AccessTransferUserControl>(this, true, "Передать права наверх");
            _dialogWindow.Show();

            _currentObjectGuid = objectGuid;

            await UpdateParentsInfo(_currentObjectGuid);
        }

        private async Task UpdateParentsInfo(Guid objectGuid)
        {
            var dataObject = await _objectLoader.Load(objectGuid);

            if (!objectGuid.IsRoot() && !objectGuid.IsEmpty())
            {
                var parents = await GetAllParentsInfoAsync(dataObject.ParentId);

                ParentObjects = new ObservableCollection<ParentInfo>(parents.Reverse());

                TreeItems = new ObservableCollection<TreeItem>(await ConvertParentsInfoToTreeItemsAsync(parents.ToArray()));

                UpdateAccessItems();
            }
            else
            {
                ParentObjects = new ObservableCollection<ParentInfo>(new List<ParentInfo>());
                TreeItems = new ObservableCollection<TreeItem>(new List<TreeItem>());
            }
        }

        private void UpdateButtonsEnabled()
        {
            bool hasSelectedParent = (_treeItems.Count > 0)? GetSelectedParents(TreeItems[0]).ToArray().Length > 0 : false;

            bool hasAnySelectedAccess = false;

            foreach (var access in AccessItems)
            {
                if (access.IsSelected)
                {
                    hasAnySelectedAccess = true;
                    break;
                }
            }

            CopyDeleteButtonsEnabled = hasSelectedParent && hasAnySelectedAccess;
        }

        private async void UpdateAccessItems()
        {
            AccessItems = new ObservableCollection<AccessCheckBox>(await GetAccessCheckBoxesAsync(_currentObjectGuid));
        }

        private void OpenAddAccessDialogWindow()
        {
            _createAccessPage.OpenWindowCommand.Execute(_currentObjectGuid);
        }

        private void CheckOnElementsOnTree(TreeItem treeItem, bool objectFind)
        {
            try
            {
                if (!objectFind)
                {
                    treeItem.IsSelected = false;
                }
                else
                {
                    treeItem.IsSelected = true;
                }

                if (((ParentInfo) treeItem.DataObject).ObjectGuid == _selectedParent.ObjectGuid && !objectFind)
                {
                    objectFind = true;
                    treeItem.IsSelected = true;
                }

                if (treeItem.Children != null && treeItem.Children.Count > 0)
                {
                    CheckOnElementsOnTree(treeItem.Children[0], objectFind);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка {ex}");
            }
        }

        private async Task<IEnumerable<ParentInfo>> GetAllParentsInfoAsync(Guid objectGuid)
        {
            var parentsInfo = new List<ParentInfo>();

            if (!objectGuid.IsRoot() && !objectGuid.IsEmpty())
            {
                var dataObject = await _objectLoader.Load(objectGuid);

                parentsInfo.Add(new ParentInfo() { Name = dataObject.DisplayName, ObjectGuid = dataObject.Id});
                
                parentsInfo.AddRange(await GetAllParentsInfoAsync(dataObject.ParentId));
            }

            return parentsInfo;
        }

        private async Task<IEnumerable<TreeItem>> ConvertParentsInfoToTreeItemsAsync(ParentInfo[] parentInfo)
        {
            var tree = new List<TreeItem>();

            foreach (var parent in parentInfo)
            {
                tree = new List<TreeItem>(){ new TreeItem()
                {
                    Name = parent.Name, 
                    Children = tree,
                    DataObject = parent,
                    IsExpanded = true,
                    IsSelected = false}
                };
            }

            return tree;
        }

        private async Task<IEnumerable<AccessCheckBox>> GetAccessCheckBoxesAsync(Guid objectGuid)
        {
            var accessInfo = await _accessLoader.GetObjectAccess(objectGuid);
            var content = new List<AccessCheckBox>();

            foreach (var info in accessInfo)
            {
                var orgUnit = _objectsRepository.GetOrganisationUnit(info.OrgUnitId);

                var newUnit = new AccessCheckBox()
                {
                    Description = _accessLoader.GetAccessNameByEnum(info.Access.AccessLevel),
                    IsSelected = false,
                    AccessRecord = info,
                };

                if (orgUnit.Kind() == OrganizationUnitKind.Position && orgUnit.Person() != -1)
                {
                    newUnit.Name = _objectsRepository.GetPerson(orgUnit.Person()).DisplayName;
                }
                else
                {
                    newUnit.Name = orgUnit.Title;
                }

                content.Add(newUnit);
            }
            return content;
        }

        private IEnumerable<ParentInfo> GetSelectedParents(TreeItem treeItem)
        {
            var parents = new List<ParentInfo>();

            if (treeItem.IsSelected)
            {
                parents.Add((ParentInfo)treeItem.DataObject);
            }

            if (treeItem.Children != null && treeItem.Children.Count > 0)
            {
                parents.AddRange(GetSelectedParents(treeItem.Children[0]));
            }

            return parents;
        }

        #region Copy and Remove
        private async void CopyAccessToParentObjectsAsync()
        {
            try
            {
                var parents = GetSelectedParents(TreeItems[0]);

                foreach (var accessRecord in AccessItems)
                {
                    if (accessRecord.IsSelected)
                    {

                        foreach (var parent in parents)
                        {
                            var unitFind = await _accessModifier.IsObjectHasOrgUnitIdAsync(accessRecord.AccessRecord.OrgUnitId, parent.ObjectGuid);

                            if (unitFind.unitFind)
                            {
                                await _accessModifier.RemoveAccessRecordByOrgUnitIdAsync(accessRecord.AccessRecord, parent.ObjectGuid);
                            }

                            await _accessModifier.AddAccessToObjectAsync(accessRecord.AccessRecord, parent.ObjectGuid);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка: {ex}");
            }
            finally
            {
                UpdateAccessItems();
                System.Windows.MessageBox.Show($"Права успешно скопированы");
            }
        }

        private async void RemoveAccessFromParentObjectsAsync()
        {
            var parents = GetSelectedParents(TreeItems[0]);

            await _accessModifier.RemoveAccessFromParentObjectsAsync(AccessItems.ToArray(), parents.ToArray(), _currentObjectGuid, true);
            
            UpdateAccessItems();
        }
        #endregion
    }
}