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

        private const string CUSTOM_B_BUTTON_NAME = "AccessTree";
        private const string CUSTOM_B_BUTTON_HEADER = "Access Tree";

        private IMenuItemBuilder _customButton;
        private IObjectModifier _modifier;
        private IObjectsRepository _objectsRepository;
        private IPilotDialogService _pilotDialogService;
        private IOrganisationUnit _organisationUnit;

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
            _customButton = builder.AddItem(CUSTOM_BUTTON_NAME, 0);
            _customButton.WithIcon(Properties.Resources.InfoSquare);
            _customButton.WithHeader(CUSTOM_BUTTON_HEADER);

            _customButton = builder.AddItem(CUSTOM_B_BUTTON_NAME, 1);
            _customButton.WithIcon(Properties.Resources.InfoSquare);
            _customButton.WithHeader(CUSTOM_B_BUTTON_HEADER);
        }

        public void OnMenuItemClick(string name, ObjectsViewContext context)
        {
            try
            {
                if (name == CUSTOM_BUTTON_NAME)
                {
                    if (context.SelectedObjects.ToArray().Length > 1)
                    {
                        _mainMenuDataGridVM = new MainMenuDataGridVM(_modifier, _objectsRepository, _pilotDialogService);
                        _mainMenuDataGridVM.OpenFormCommand.Execute(context.SelectedObjects.ToArray());
                    }
                    else if (context.SelectedObjects.ToArray().Length == 1)
                    {
                        CurrentObjectFormVM currentObjectFormVM = new CurrentObjectFormVM(_modifier, _objectsRepository);
                        currentObjectFormVM.OpenCommand.Execute(context.SelectedObjects.ToArray()[0]);
                    }
                    
                }

                else if (name == CUSTOM_B_BUTTON_NAME)
                {
                    AccessInfoWindowVM access = new AccessInfoWindowVM(_objectsRepository);
                    access.OpenDialogCommand.Execute(null);
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