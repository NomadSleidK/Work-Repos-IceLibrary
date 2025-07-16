using Ascon.Pilot.SDK;
using MyIceLibrary.Command;
using MyIceLibrary.Model;
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
    public class AccessLevelPageVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<AccessLevelInfo> _attributesValue;
        public ObservableCollection<AccessLevelInfo> CurrentObjectAttributesValue
        {
            get => _attributesValue;
            private set
            {
                _attributesValue = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadAccessLevelCommand => new RelayCommand<Guid>(LoadAccessLevel);

        private readonly IObjectsRepository _objectsRepository;
        private readonly ObjectLoader _objectLoader;

        public AccessLevelPageVM(IObjectsRepository objectsRepository)
        {
            _objectLoader = new ObjectLoader(objectsRepository);
            _objectsRepository = objectsRepository;
        }

        private async void LoadAccessLevel(Guid currentObjectGuid)
        {
            IDataObject currentObject = await _objectLoader.Load(currentObjectGuid);

            var access = (await CheckAccess(currentObject)).Distinct().ToArray();

            CurrentObjectAttributesValue = new ObservableCollection<AccessLevelInfo>(access);
        }

        private async Task<List<AccessLevelInfo>> CheckAccess(IDataObject currentDataObject)
        {
            var resultAccess = new List<AccessLevelInfo>();
            var currentObjectAccess = currentDataObject.Access2;
            
            foreach (var access in currentObjectAccess)
            {
                var layerAccess = GetAccesLevelInfo(access, currentDataObject);
                resultAccess.Add(layerAccess);
            }

            if (currentDataObject.ParentId != Guid.Empty)
            {
                var parentObject = await _objectLoader.Load(currentDataObject.ParentId);

                resultAccess.AddRange(await CheckAccess(parentObject));
            }

            return resultAccess;
        }

        private AccessLevelInfo GetAccesLevelInfo(IAccessRecord currentAccessRecord, IDataObject currentDataObject)
        {
            var organizationUnit = _objectsRepository.GetOrganisationUnit(currentAccessRecord.OrgUnitId);
            var accessInfo = new AccessLevelInfo() { PersonName = organizationUnit.Title, 
                None = false, Create = false, Edit = false, View = false, Freeze = false, Agreement = false, Share = false,
                ViewCreate = false, ViewEdit = false, ViewEditAgrement = false, Full = false};

            if (organizationUnit.Kind() == OrganizationUnitKind.Position && organizationUnit.Person() != -1)
            {
                accessInfo.PersonName = _objectsRepository.GetPerson(organizationUnit.Person()).DisplayName;
            }

            AccessLevel level = currentAccessRecord.Access.AccessLevel;

            foreach (AccessLevel value in Enum.GetValues(typeof(AccessLevel)))
            {
                if (value != AccessLevel.None && (level & value) == value)
                {
                    switch (value)
                    {
                        case AccessLevel.None:
                            accessInfo.None = true;
                            break;
                        case AccessLevel.Create:
                            accessInfo.Create = true;
                            break;
                        case AccessLevel.Edit:
                            accessInfo.Edit = true;
                            break;
                        case AccessLevel.View:
                            accessInfo.View = true;
                            break;
                        case AccessLevel.Freeze:
                            accessInfo.Freeze = true;
                            break;
                        case AccessLevel.Agreement:
                            accessInfo.Agreement = true;
                            break;
                        case AccessLevel.Share:
                            accessInfo.Share = true;
                            break;
                        case AccessLevel.ViewCreate:
                            accessInfo.ViewCreate = true;
                            break;
                        case AccessLevel.ViewEdit:
                            accessInfo.ViewEdit = true;
                            break;
                        case AccessLevel.ViewEditAgrement:
                            accessInfo.ViewEditAgrement = true;
                            break;
                        case AccessLevel.Full:
                            accessInfo.Full = true;
                            break;
                    }
                }
            }

            return accessInfo;
        }
    }
}