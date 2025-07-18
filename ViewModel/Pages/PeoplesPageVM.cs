using Ascon.Pilot.SDK;
using MyIceLibrary.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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

        public PeoplesPageVM(IObjectsRepository objectsRepository)
        {
            _objectsRepository = objectsRepository;
        }

        public ICommand LoadPepleInfoCommand => new RelayCommand<object>(_ => LoadPepleInfo());
        public ICommand SelectedElementCommand => new RelayCommand<PersonInfo>(SelectedElement);

        private void LoadPepleInfo()
        {
            PeopleInfo = CreatePeopleInfoColliction(_objectsRepository.GetPeople().ToArray());


        }

        private ObservableCollection<PersonInfo> CreatePeopleInfoColliction(IPerson[] people)
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

        private void SelectedElement(PersonInfo personInfo)
        {
            var dataPerson = new List<CurrentObjectInfo>();
            var person = _objectsRepository.GetPerson(personInfo.Id);

            dataPerson.Add(new CurrentObjectInfo() { Name = "Имя", Value = person.ActualName });
            dataPerson.Add(new CurrentObjectInfo() { Name = "Id", Value = person.Id });

            CurrentObjectFilesInfo = new ObservableCollection<CurrentObjectInfo>(dataPerson);
        }
    }
}
