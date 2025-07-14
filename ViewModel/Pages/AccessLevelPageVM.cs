using Ascon.Pilot.SDK;
using MyIceLibrary.AccessNames;
using MyIceLibrary.Command;
using MyIceLibrary.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
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

            var persons = new List<AccessLevelInfo>();


            var currentObjectAccess = currentObject.Access2;

            foreach (var access in currentObjectAccess)
            {
                IOrganisationUnit uniy = _objectsRepository.GetOrganisationUnit(access.OrgUnitId);
                var personId = uniy.Person();


                if (personId != -1)
                {
                    var dataPerson = _objectsRepository.GetPerson(personId);
                    string name = AccessNames.AccessNames.GetAccessName(access.Access.AccessLevel);

                    persons.Add(new AccessLevelInfo() { PersonName = dataPerson.ActualName, AccsessName = name });
                }
            }

            CurrentObjectAttributesValue = new ObservableCollection<AccessLevelInfo>(persons);


            //_________________________________________________

            //var access = _objectsRepository.GetOrganisationUnits();

            //var childs = new List<IOrganisationUnit>();

            //foreach (var unit in access)
            //{
            //    foreach (var childId in unit.Children)
            //    {
            //        childs.Add(_objectsRepository.GetOrganisationUnit(childId));

            //        if (personId != -1)
            //        {
            //            var dataPerson = _objectsRepository.GetPerson(personId);
            //            string name = AccessNames.AccessNames.GetAccessName(access.Access.AccessLevel);

            //            persons.Add(new AccessLevelInfo() { PersonName = dataPerson.ActualName, AccsessName = name });
            //        }
            //    }
            //}

            //CurrentObjectAttributesValue = new ObservableCollection<IOrganisationUnit>(access);
        }
    }
}
