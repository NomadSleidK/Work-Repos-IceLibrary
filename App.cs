using Ascon.Pilot.SDK;
using Ascon.Pilot.SDK.Menu;
using MyIceLibrary.ViewModel;
using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace MyIceLibrary
{
    [Export(typeof(IMenu<ObjectsViewContext>))]
    internal class App : IMenu<ObjectsViewContext>
    {
        private const string CUSTOM_BUTTON_NAME = "CustomProperties";
        private const string CUSTOM_BUTTON_HEADER = "My Object Info";

        private IMenuItemBuilder _customButton;
        private IObjectModifier _modifier;
        private IObjectsRepository _objectsRepository;
        private IPilotDialogService _pilotDialogService;

        private MainMenuDataGridVM _mainMenuDataGridVM;

        #region IMenu <ObjectsViewContext>

        [ImportingConstructor]
        public App(IObjectModifier modifier, IObjectsRepository objectsRepository, IPilotDialogService pilotDialogService)
        {
            _modifier = modifier;
            _objectsRepository = objectsRepository;
            _pilotDialogService = pilotDialogService;
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
                    _mainMenuDataGridVM = new MainMenuDataGridVM(_modifier, _objectsRepository, _pilotDialogService);
                    _mainMenuDataGridVM.OpenMainInfoFormCommand.Execute(context.SelectedObjects.ToArray());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        #endregion
    }
}