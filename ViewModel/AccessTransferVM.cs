using Ascon.Pilot.SDK;
using Ascon.Pilot.Theme.Controls;
using MyIceLibrary.Command;
using MyIceLibrary.Extensions;
using MyIceLibrary.Helper;
using MyIceLibrary.Model;
using MyIceLibrary.View;
using MyIceLibrary.View.Pages;
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

                OnPropertyChanged();
            }
        }
        #endregion

        private readonly IObjectsRepository _objectsRepository;
        private readonly DialogWindow _dialogWindow;

        private readonly ObjectLoader _objectLoader;
        private readonly IObjectModifier _objectModifier;
        private readonly ObjectAccessHelper _objectAccessHelper;

        private readonly CreateAccessPageVM _createAccessPage;


        private Guid _currntObjectGuid;

        public AccessTransferVM(IObjectsRepository objectsRepository, IObjectModifier objectModifier)
        {
            _objectsRepository = objectsRepository;
            _objectModifier = objectModifier;

            _objectLoader = new ObjectLoader(objectsRepository);
            _objectAccessHelper = new ObjectAccessHelper(objectsRepository);

            _createAccessPage = new CreateAccessPageVM(objectsRepository, objectModifier);

            _dialogWindow = WindowHelper.CreateWindowWithUserControl<AccessTransferUserControl>();

            _dialogWindow.DataContext = this;
            _dialogWindow.ShowInTaskbar = true;
            _dialogWindow.Title = "Передать права наверх";
            _createAccessPage.OnAccessCreated += UpdateAccessItems;
        }

        public ICommand OpenWindowCommand => new RelayCommand<Guid>(OpenWindowAsync);
        public ICommand CopyAccessToParentObjectsCommand => new RelayCommand<object>(_ => CopyAccessToParentObjectsAsync());
        public ICommand RemoveAccessFromParentObjectsCommand => new RelayCommand<object>(_ => RemoveAccessFromParentObjects());
        public ICommand OpenAddAccessDialogWindowCommand => new RelayCommand<object>(_ => OpenAddAccessDialogWindow());

        private async void OpenWindowAsync(Guid objectGuid)
        {
            _dialogWindow.Show();
            _currntObjectGuid = objectGuid;

            var dataObject = await _objectLoader.Load(objectGuid);

            if (objectGuid.IsRoot() && objectGuid.IsEmpty())
            {
                var parents = await GetParentsInfoAsync(dataObject.ParentId);

                ParentObjects = new ObservableCollection<ParentInfo>(parents.Reverse());
                
                UpdateAccessItems();

                TreeItems = new ObservableCollection<TreeItem>(await CreateParentsTreeAsync(parents.ToArray()));
            }
            else
            {
                ParentObjects = new ObservableCollection<ParentInfo>(new List<ParentInfo>());
                TreeItems = new ObservableCollection<TreeItem>(new List<TreeItem>());
            }
        }

        private async void UpdateAccessItems()
        {
            AccessItems = new ObservableCollection<AccessCheckBox>(await LoadObjectAccessInfoAsync(_currntObjectGuid));
        }

        private void OpenAddAccessDialogWindow()
        {
            _createAccessPage.OpenWindowCommand.Execute(_currntObjectGuid);
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
            catch
            {

            }
        }

        private async Task<IEnumerable<ParentInfo>> GetParentsInfoAsync(Guid objectGuid)
        {
            var parentsInfo = new List<ParentInfo>();

            if (objectGuid.IsRoot() && objectGuid.IsEmpty())
            {
                var dataObject = await _objectLoader.Load(objectGuid);

                parentsInfo.Add(new ParentInfo() { Name = dataObject.DisplayName, ObjectGuid = dataObject.Id});
                
                parentsInfo.AddRange(await GetParentsInfoAsync(dataObject.ParentId));
            }

            return parentsInfo;
        }

        private async Task<IEnumerable<TreeItem>> CreateParentsTreeAsync(ParentInfo[] parentInfo)
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

        private async Task<IEnumerable<AccessCheckBox>> LoadObjectAccessInfoAsync(Guid objectGuid)
        {
            var dataObject = await _objectLoader.Load(objectGuid);
            var accessInfo = await _objectAccessHelper.GetObjectAccess(objectGuid);
            var content = new List<AccessCheckBox>();

            foreach (var info in accessInfo)
            {
                var orgUnit = _objectsRepository.GetOrganisationUnit(info.OrgUnitId);

                var newUnit = new AccessCheckBox()
                {
                    Description = info.Access.AccessLevel.ToString(),
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
           return (content);
        }

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
                            var unitFind = await IsObjectHasOrgUnitIdAsync(accessRecord.AccessRecord.OrgUnitId, parent.ObjectGuid);

                            if (unitFind.unitFind)
                            {
                                await RemoveAccessRecordByOrgUnitIdAsync(accessRecord.AccessRecord, parent.ObjectGuid);
                            }

                            await AddAccessToObjectAsync(accessRecord.AccessRecord, parent.ObjectGuid);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошиюка: {ex}");
            }
            finally
            {
                System.Windows.MessageBox.Show($"Права успешно скопированы");
            }
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

        private async Task AddAccessToObjectAsync(IAccessRecord accessRecord, Guid targetObject)
        {
            var objectInfo = await _objectLoader.Load(targetObject);

            var builder = _objectModifier.EditById(objectInfo.Id);

            builder.AddAccessRecords(
                accessRecord.OrgUnitId, 
                accessRecord.Access.AccessLevel,
                accessRecord.Access.ValidThrough, 
                accessRecord.Access.Inheritance,
                accessRecord.Access.Type, 
                accessRecord.Access.TypeIds);

            _objectModifier.Apply();
        }
   
        private async void RemoveAccessFromParentObjects()
        {
            foreach (var accessRecord in AccessItems)
            {
                if (accessRecord.IsSelected)
                {
                    await RemoveAccessRecordByOrgUnitIdAsync(accessRecord.AccessRecord, _currntObjectGuid);
                }
            }

            await RemoveAccessByAccessLevelAsync();

            AccessItems = new ObservableCollection<AccessCheckBox>(await LoadObjectAccessInfoAsync(_currntObjectGuid));
        }

        private async Task RemoveAccessByAccessLevelAsync()
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
                            var findUnit = await IsObjectHasOrgUnitIdAsync(accessRecord.AccessRecord.OrgUnitId, parent.ObjectGuid);

                            if (findUnit.unitFind &&
                                accessRecord.AccessRecord.Access.AccessLevel == findUnit.unitAccessRecord.Access.AccessLevel)
                            {
                                await RemoveAccessRecordByOrgUnitIdAsync(accessRecord.AccessRecord, parent.ObjectGuid);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошиюка: {ex}");
            }
            finally
            {
                System.Windows.MessageBox.Show($"Права успешно удалены");
            }
        }

        private async Task RemoveAccessRecordByOrgUnitIdAsync(IAccessRecord accessRecord, Guid targetObject)
        {
            var objectInfo = await _objectLoader.Load(targetObject);

            var builder = _objectModifier.EditById(objectInfo.Id);

            builder.RemoveAccessRecords(x => RemoveAccessRecordFunc(x, accessRecord.OrgUnitId));

            _objectModifier.Apply();
        }

        private bool RemoveAccessRecordFunc(IAccessRecord accessRecord, int targetOrgUnitId)
        {
            bool result = false;

            if (accessRecord.OrgUnitId == targetOrgUnitId)
            {
                result = true;
            }

            return result;
        }

        private async Task<(bool unitFind, IAccessRecord unitAccessRecord)> IsObjectHasOrgUnitIdAsync(int orgUnitId, Guid objectGuid)
        {
            var dataObject = await _objectLoader.Load(objectGuid);
            var accessRecords = dataObject.Access2;

            bool unitFind = false;
            IAccessRecord accessRecord = null;

            foreach ( var record in accessRecords)
            {
                if (record.OrgUnitId == orgUnitId)
                {
                    unitFind = true;
                    accessRecord = record;

                    break;
                }
            }

            return (unitFind, accessRecord);
        }
    }
}