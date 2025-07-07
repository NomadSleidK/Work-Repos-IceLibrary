using System.Collections.Generic;
using System.Windows;
using static MyIceLibrary.DataGridLoadInfoHelper;

namespace MyIceLibrary
{
    public partial class CurrentObjectForm : Window
    {
        public event OpenNewWindowHandler OnOpenNewWindow;

        private readonly Ascon.Pilot.SDK.IDataObject _dataObject;

        public CurrentObjectForm(Ascon.Pilot.SDK.IDataObject[] dataObject)
        {
            InitializeComponent();

            _dataObject = dataObject[0];

            LoadAttributesObjectsToGrid(dataObject);
            LoadMainInfoToGrid(dataObject);

            ObjectName.Content = dataObject[0].DisplayName;
        }
     
        #region Buttons Click
        public void ParentDoubleClick(object sender, RoutedEventArgs e)
        {
            OnOpenNewWindow?.Invoke(_dataObject.ParentId);
        }
        #endregion

        #region Load Info
        public void SetParentObject(Ascon.Pilot.SDK.IDataObject[] dataObject)
        {
            ParentNameLabel.Content = dataObject[0].DisplayName;
        }

        private void LoadAttributesObjectsToGrid(Ascon.Pilot.SDK.IDataObject[] objects)
        {
            var attributes = new List<IDictionary<string, object>>();

            foreach (var obj in objects)
            {
                attributes.Add(obj.Attributes);
            }

            DataGridLoadInfoHelper.LoadAttributeToGrid(AttributesDataGrid, attributes.ToArray());
        }

        private void LoadMainInfoToGrid(Ascon.Pilot.SDK.IDataObject[] dataObjects)
        {
            DataGridLoadInfoHelper.LoadMainInfoToGrid(MainInfoDataGrid, dataObjects);
        }
        #endregion
    }
}
