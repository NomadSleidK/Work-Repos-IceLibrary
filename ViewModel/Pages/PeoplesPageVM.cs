using Ascon.Pilot.SDK;
using Ascon.Pilot.Theme.Controls;
using MyIceLibrary.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MyIceLibrary.Model
{
    public class PeoplesPageVM : INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Property
        private ObservableCollection<PersonInfo> _peopleInfo;
        public ObservableCollection<PersonInfo> PeopleInfo
        {
            get => _peopleInfo;
            private set
            {
                _peopleInfo = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<CurrentObjectInfo> _currentObjectFilesInfo;
        public ObservableCollection<CurrentObjectInfo> CurrentObjectFilesInfo
        {
            get => _currentObjectFilesInfo;
            private set
            {
                _currentObjectFilesInfo = value;
                OnPropertyChanged();
            }
        }
        #endregion

        private readonly IObjectsRepository _objectsRepository;
        private ObservableCollection<PersonInfo> _originPeopleInfo;

        public PeoplesPageVM(IObjectsRepository objectsRepository)
        {
            _objectsRepository = objectsRepository;
        }

        public ICommand LoadPeopleInfoCommand => new RelayCommand<object>(_ => LoadPeopleInfo());
        public ICommand SelectedElementCommand => new RelayCommand<PersonInfo>(SelectedElement);
        public ICommand FilteredBoxExecuteEnterCommand => new RelayCommand<string>(FilteredInfo);

        private void LoadPeopleInfo()
        {
            _originPeopleInfo = CreatePeopleInfoCollection(_objectsRepository.GetPeople().ToArray());
            PeopleInfo = new ObservableCollection<PersonInfo>(_originPeopleInfo);
        }

        private ObservableCollection<PersonInfo> CreatePeopleInfoCollection(IPerson[] people)
        {
            var peopleInfo = new List<PersonInfo>();

            foreach (var person in people)
            {
                peopleInfo.Add(new PersonInfo()
                {
                    Name = person.ActualName,
                    Id = person.Id,
                });
            }

            return new ObservableCollection<PersonInfo>(peopleInfo);
        }

        private void FilteredInfo(string input)
        {
            if (input != "")
            {
                var filteredPeople = _originPeopleInfo
                    .Where(p => p.Name.ToLower().Contains(input.ToLower())).ToList();

                filteredPeople.AddRange(_originPeopleInfo
                    .Where(p => p.Id.ToString().ToLower().Contains(input.ToLower())).ToList());

                PeopleInfo = new ObservableCollection<PersonInfo>(filteredPeople);
            }
            else
            {
                PeopleInfo = new ObservableCollection<PersonInfo>(_originPeopleInfo);
            }    
        }

        private void SelectedElement(PersonInfo personInfo)
        {
            var dataPerson = new List<CurrentObjectInfo>();
            var person = _objectsRepository.GetPerson(personInfo.Id);

            dataPerson.Add(new CurrentObjectInfo() { Name = "ActualName", Value = person.ActualName });
            dataPerson.Add(new CurrentObjectInfo() { Name = "Id", Value = person.Id });
            dataPerson.Add(new CurrentObjectInfo() { Name = "DisplayName", Value = person.DisplayName });
            dataPerson.Add(new CurrentObjectInfo() { Name = "Login", Value = person.Login });
            dataPerson.Add(new CurrentObjectInfo() { Name = "ServiceInfo", Value = person.ServiceInfo });
            dataPerson.Add(new CurrentObjectInfo() { Name = "Sid", Value = person.Sid });
            dataPerson.Add(new CurrentObjectInfo() { Name = "IsAdmin", Value = person.IsAdmin });
            dataPerson.Add(new CurrentObjectInfo() { Name = "IsDeleted", Value = person.IsDeleted });
            dataPerson.Add(new CurrentObjectInfo() { Name = "CreatedUtc", Value = person.CreatedUtc.ToString("dd-MM-yyyy HH:mm:ss") });
            dataPerson.Add(new CurrentObjectInfo() { Name = "Comment", Value = person.Comment });

            CurrentObjectFilesInfo = new ObservableCollection<CurrentObjectInfo>(dataPerson);
        }
    }
}
