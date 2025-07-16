using Ascon.Pilot.SDK;
using MyIceLibrary.Command;
using MyIceLibrary.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;

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

        private ObservableCollection<TypeInfo> _typesinfo;
        public ObservableCollection<TypeInfo> TypesInfo
        {
            get => _typesinfo;
            set
            {
                _typesinfo = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadTypesCommand => new RelayCommand<object>(_ => LoadTypes());

        private readonly IObjectsRepository _objectsRepository;

        public TypesPageVM(IObjectsRepository objectsRepository)
        {
            _objectsRepository = objectsRepository;
        }

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
                    Kind = type.Kind,
                    IsMountable = type.IsMountable,
                    IsDeleted = type.IsDeleted,
                    IsProject = type.IsProject,
                    IsService = type.IsService,
                });
            }

            TypesInfo = new ObservableCollection<TypeInfo>(typesInfo);
        }

        public BitmapImage ToImage(byte[] array)
        {
            using (var ms = new System.IO.MemoryStream(array))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }
    }
}
