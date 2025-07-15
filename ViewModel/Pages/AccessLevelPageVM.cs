using Ascon.Pilot.SDK;
using MyIceLibrary.Command;
using MyIceLibrary.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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

        private Guid _currentObjectGuid;

        public AccessLevelPageVM(IObjectsRepository objectsRepository)
        {
            _objectLoader = new ObjectLoader(objectsRepository);
            _objectsRepository = objectsRepository;
        }

        private async void LoadAccessLevel(Guid currentObjectGuid)
        {
            _currentObjectGuid = currentObjectGuid;
            IDataObject currentObject = await _objectLoader.Load(_currentObjectGuid);

            var currentObjectAccess = currentObject.Access2;

            CurrentObjectAttributesValue = new ObservableCollection<AccessLevelInfo>();

            foreach (var person in currentObjectAccess)
            {
                UpdateAccessLayers(person, currentObject);
            }
        }

        private async void UpdateAccessLayers(IAccessRecord currentAccessRecord, IDataObject currentDataObject)
        {
            var organizationUnit = _objectsRepository.GetOrganisationUnit(currentAccessRecord.OrgUnitId);

            var layer = new AccessLevelInfo() { PersonName = AccessNames.AccessNames.GetAccessName(currentAccessRecord.Access.AccessLevel), AccessName = organizationUnit.Title };
            if (organizationUnit.Kind() == OrganizationUnitKind.Position && organizationUnit.Person() != -1)
            {
                layer.MoreInfo = _objectsRepository.GetPerson(organizationUnit.Person()).DisplayName;
            }

            CurrentObjectAttributesValue.Add(layer);

            if (currentDataObject.ParentId != Guid.Empty)
            {
                var nextOrganizationUnit = await _objectLoader.Load(currentDataObject.ParentId);
                var accessRecords = nextOrganizationUnit.Access2;            

                foreach (var accessRecord in accessRecords)
                {
                    UpdateAccessLayers(accessRecord, nextOrganizationUnit);
                }
            }
        }
    }
}
