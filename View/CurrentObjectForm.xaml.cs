using System.Collections.Generic;
using System.Windows;
using static MyIceLibrary.DataGridHelper;

namespace MyIceLibrary.View
{
    public partial class CurrentObjectForm : Window
    {
        public event OpenNewWindowHandler OnOpenNewWindow;

        private readonly Ascon.Pilot.SDK.IDataObject _dataObject;

        public CurrentObjectForm()
        {
            InitializeComponent();
            //Ascon.Pilot.SDK.IDataObject[] dataObject
            //_dataObject = dataObject[0];

            //LoadAttributesObjectsToGrid(dataObject);
            //LoadMainInfoToGrid(dataObject);

            //CurrentObjectName.Content = dataObject[0].DisplayName;
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
            //ParentNameLabel.Content = dataObject[0].DisplayName;
        }

        private void LoadAttributesObjectsToGrid(Ascon.Pilot.SDK.IDataObject[] objects)
        {
            //var attributes = new List<IDictionary<string, object>>();

            //foreach (var obj in objects)
            //{
            //    attributes.Add(obj.Attributes);
            //}

            //DataGridHelper.LoadAttributeToGrid(AttributesDataGrid, attributes.ToArray());
        }

        private void LoadMainInfoToGrid(Ascon.Pilot.SDK.IDataObject[] dataObjects)
        {
            //DataGridHelper.LoadMainInfoToGrid(MainInfoDataGrid, dataObjects);
        }
        #endregion
    }
}
