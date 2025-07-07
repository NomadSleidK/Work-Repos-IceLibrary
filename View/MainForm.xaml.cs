using System;
using System.Collections.Generic;
using System.Windows;
using static MyIceLibrary.DataGridHelper;

namespace MyIceLibrary.View
{
    public partial class MainForm : Window
    {
        public delegate void AccountHandler(IEnumerable<Guid> selectedGuids);

        public event AccountHandler OnDeleteButtonClick;
        public event OpenNewWindowHandler OnOpenNewWindow;

        private readonly bool _isAdmin;

        public MainForm()
        {
            //bool isAdmin, Ascon.Pilot.SDK.IDataObject[] objects

            InitializeComponent();
            //_isAdmin = isAdmin;

            //DeleteButton.IsEnabled = _isAdmin;
            //DeleteButton.Visibility = _isAdmin ? Visibility.Visible : Visibility.Collapsed;

            //LoadAttributesObjectsToGrid(objects);
            //LoadMainInfoToGrid(objects);
        }

        #region Buttons Click
        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            //var rows = MainInfoDataGrid.SelectedItems;
            //var selectedGuids = new List<Guid>();

            //foreach (var selectedItem in rows)
            //{
            //    string result = ((MainInfoValue)selectedItem).ID.ToString();

            //    if (Guid.TryParse(result.ToString(), out Guid guid))
            //    {
            //        selectedGuids.Add(guid);
            //    }
            //}

            //OnDeleteButtonClick?.Invoke(selectedGuids);
        }

        private void MainInfoDataGridDoubleClick(object sender, RoutedEventArgs e)
        {
            //var row = MainInfoDataGrid.SelectedItem;
            //string result = ((MainInfoValue)row).ID.ToString();

            //OnOpenNewWindow?.Invoke(Guid.Parse(result));
        }
        #endregion

        #region Load Info
        private void LoadAttributesObjectsToGrid(Ascon.Pilot.SDK.IDataObject[] objects)
        {
            //var attributes = new List<IDictionary<string, object>>();

            //foreach (var obj in objects)
            //{
            //    attributes.Add(obj.Attributes);
            //}

            //LoadAttributeToGrid(AttributesDataGrid, attributes.ToArray());
        }

        private void LoadMainInfoToGrid(Ascon.Pilot.SDK.IDataObject[] dataObjects)
        {
            //DataGridHelper.LoadMainInfoToGrid(MainInfoDataGrid, dataObjects);
        }
        #endregion
    }
}