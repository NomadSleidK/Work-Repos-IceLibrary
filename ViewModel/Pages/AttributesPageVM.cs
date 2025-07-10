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
    public class AttributesPageVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<AttributeValue> _attributesValue;
        public ObservableCollection<AttributeValue> CurrentObjectAttributesValue
        {
            get => _attributesValue;
            private set
            {
                _attributesValue = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadAttributesCommand => new RelayCommand<IDataObject>(LoadObjectAttributes);

        public AttributesPageVM() { }

        private void LoadObjectAttributes(IDataObject dataObject)
        {
            try
            {
                List<AttributeValue> attributes = new List<AttributeValue>();

                foreach (var item in dataObject.Attributes)
                {
                    attributes.Add(new AttributeValue(item.Key, item.Value));
                }

                ObservableCollection<AttributeValue> observableCollection = new ObservableCollection<AttributeValue>(attributes);

                CurrentObjectAttributesValue = observableCollection;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}