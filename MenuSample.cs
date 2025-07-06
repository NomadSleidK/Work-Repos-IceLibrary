using Ascon.Pilot.SDK;
using Ascon.Pilot.SDK.Menu;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace MyIceLibrary
{
    [Export(typeof(IMenu<ObjectsViewContext>))]
    public class MenuSample : IMenu<ObjectsViewContext>
    {
        private const string CUSTOM_BUTTON_NAME = "CustomProperties";
        private const string CUSTOM_BUTTON_HEADER = "My Object Info";

        private IMenuItemBuilder _customButton;
        private IObjectModifier _modifier;
        private IObjectsRepository _objectsRepository;

        private MainForm _mainWindow;

        [ImportingConstructor]
        public MenuSample(IObjectModifier modifier, IObjectsRepository objectsRepository)
        {
            _modifier = modifier;
            _objectsRepository = objectsRepository;
        }

        public void Build(IMenuBuilder builder, ObjectsViewContext context)
        {
            _customButton = builder.AddItem(CUSTOM_BUTTON_NAME, builder.Count);

            _customButton.WithIcon(Properties.Resources.InfoSquare);
            _customButton.WithHeader(CUSTOM_BUTTON_HEADER);
        }

        public void OnMenuItemClick(string name, ObjectsViewContext context)
        {          
            try
            {
                if (name == CUSTOM_BUTTON_NAME)
                {
                    OnCustomButtonClick(context);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void OnCustomButtonClick(ObjectsViewContext context)
        {
            IDataObject[] objects = context.SelectedObjects.ToArray();
            IPerson person = _objectsRepository.GetCurrentPerson();

            _mainWindow = new MainForm(person.IsAdmin);

            if (person.IsAdmin)
            {
                _mainWindow.OnDeleteButtonClick += OnDeleteByIdHandler;
            }

            _mainWindow.LoadAttributesObjectsToGrid(objects);
            _mainWindow.LoadMainInfoToGrid(objects);

            _mainWindow.ShowDialog();
        }

        private void OnDeleteByIdHandler(IEnumerable<Guid> selectedGuids)
        {
            try
            {
                foreach (var obj in selectedGuids)
                {
                    _modifier.DeleteById(obj);
                    _modifier.Apply();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                System.Windows.MessageBox.Show($"{selectedGuids.Count()} Объекта успешно удалены ");
                _mainWindow?.Close();
            }
        }
    }
}