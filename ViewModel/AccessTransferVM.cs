using Ascon.Pilot.SDK;
using Ascon.Pilot.Theme.Controls;
using MyIceLibrary.Command;
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
using static MyIceLibrary.ViewModel.Pages.AccessTransferVM;

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

        private ParentInfo _selectedParent;
        public ParentInfo SelectedParent
        {
            get => _selectedParent;
            set
            {
                _selectedParent = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public class ParentInfo
        {
            public string Name { get; set; }
            public Guid ObjectGuid { get; set; }
        }

        private readonly IObjectsRepository _objectsRepository;
        private readonly DialogWindow _dialogWindow;

        private readonly ObjectLoader _objectLoader;
        private readonly IObjectModifier _objectModifier;
        private readonly ObjectAccessHelper _objectAccessHelper;

        private Guid _currntObjectGuid;

        public AccessTransferVM(IObjectsRepository objectsRepository, IObjectModifier objectModifier)
        {
            _objectsRepository = objectsRepository;
            _objectModifier = objectModifier;

            _objectLoader = new ObjectLoader(objectsRepository);
            _objectAccessHelper = new ObjectAccessHelper(objectsRepository);

            _dialogWindow = WindowHelper.CreateWindowWithUserControl<AccessTransferUserControl>();
            _dialogWindow.DataContext = this;
        }

        public ICommand OpenWindowCommand => new RelayCommand<Guid>(OpenWindow);
        public ICommand ButtonClickCommand => new RelayCommand<object>(_ => Buttoncick());

        private async void OpenWindow(Guid objectGuid)
        {
            _dialogWindow.Show();
            _currntObjectGuid = objectGuid;

            //var access = _objectAccessHelper.GetObjectAccess(objectGuid);

            ParentObjects = new ObservableCollection<ParentInfo>(await GetParentsAsync(objectGuid));
            AccessItems = new ObservableCollection<AccessCheckBox>(await LoadObjectAccessInfo(objectGuid));
        }



        private async Task<IEnumerable<ParentInfo>> GetParentsAsync(Guid objectGuid)
        {
            var parentsInfo = new List<ParentInfo>();

            if (objectGuid != new Guid("00000001-0001-0001-0001-000000000001") && objectGuid != Guid.Empty)
            {
                var dataObject = await _objectLoader.Load(objectGuid);

                parentsInfo.Add(new ParentInfo() { Name = dataObject.DisplayName, ObjectGuid = dataObject.Id});
                
                parentsInfo.AddRange(await GetParentsAsync(dataObject.ParentId));
            }

            return parentsInfo;
        }



        private async Task<IEnumerable<AccessCheckBox>> LoadObjectAccessInfo(Guid objectGuid)
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

        private async Task Buttoncick()
        {
            if (SelectedParent == null)
            {
                System.Windows.MessageBox.Show("! Выберите объект до которого нужно передать доступ !");
                return;
            }

            try
            {
                var parents = ParentObjects.ToList();

                foreach (var accessRecord in AccessItems)
                {
                    if (accessRecord.IsSelected)
                    {

                        foreach (var parent in parents)
                        {
                            await AddAccessToObject(accessRecord.AccessRecord, parent.ObjectGuid);
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

        private async Task AddAccessToObject(IAccessRecord accessRecord, Guid targetObject)
        {
            var objectInfo = await _objectLoader.Load(targetObject);

            var builder = _objectModifier.EditById(objectInfo.ParentId);

            builder.AddAccessRecords(accessRecord.OrgUnitId, accessRecord.Access.AccessLevel,
                accessRecord.Access.ValidThrough, accessRecord.Access.Inheritance,
                accessRecord.Access.Type, accessRecord.Access.TypeIds);

            _objectModifier.Apply();
        }
    }
}
