using Ascon.Pilot.SDK;
using MyIceLibrary.Command;
using MyIceLibrary.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MyIceLibrary.ViewModel.Pages
{
    public class TypesPageVM : INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private ObservableCollection<TypeInfo> _typesInfo;
        public ObservableCollection<TypeInfo> TypesInfo
        {
            get => _typesInfo;
            set
            {
                _typesInfo = value;
                OnPropertyChanged();
            }
        }

        private readonly IObjectsRepository _objectsRepository;
        private ObservableCollection<TypeInfo> _originalTypeInfo;

        public TypesPageVM(IObjectsRepository objectsRepository)
        {
            _objectsRepository = objectsRepository;
        }

        public ICommand LoadTypesCommand => new RelayCommand<object>(_ => LoadTypes());
        public ICommand FilteredBoxExecuteEnterCommand => new RelayCommand<string>(FilteredInfo);

        private void LoadTypes()
        {
            var types = _objectsRepository.GetTypes().ToArray();
            var typesInfo = new List<TypeInfo>();

            foreach (var type in types)
            {
                typesInfo.Add(new TypeInfo()
                {
                    Icon = type.SvgIcon,
                    Name = type.Name,
                    Id = type.Id,
                    Title = type.Title,
                    Kind = type.Kind.ToString(),
                    IsMountable = type.IsMountable,
                    IsDeleted = type.IsDeleted,
                    IsProject = type.IsProject,
                    IsService = type.IsService,
                });
            }

            _originalTypeInfo = new ObservableCollection<TypeInfo>(typesInfo);
            TypesInfo = new ObservableCollection<TypeInfo>(_originalTypeInfo);
        }

        private void FilteredInfo(string input)
        {
            if (input != "")
            {
                var filteredPeople = _originalTypeInfo
                    .Where(p => p.Name.ToLower().Contains(input.ToLower())).ToList();

                filteredPeople.AddRange(_originalTypeInfo
                    .Where(p => p.Title.ToLower().Contains(input.ToLower())).ToList());

                filteredPeople.AddRange(_originalTypeInfo
                    .Where(p => p.Kind.ToLower().Contains(input.ToLower())).ToList());

                filteredPeople.AddRange(_originalTypeInfo
                    .Where(p => p.Id.ToString().ToLower().Contains(input.ToLower())).ToList());

                TypesInfo = new ObservableCollection<TypeInfo>(filteredPeople);
            }
            else
            {
                TypesInfo = new ObservableCollection<TypeInfo>(_originalTypeInfo);
            }
        }
    }
}
