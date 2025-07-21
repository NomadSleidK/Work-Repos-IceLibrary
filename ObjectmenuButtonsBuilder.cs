using Ascon.Pilot.SDK;
using Ascon.Pilot.SDK.Menu;
using MyIceLibrary.Helper;
using MyIceLibrary.ViewModel;
using MyIceLibrary.ViewModel.Pages;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace MyIceLibrary
{
    [Export(typeof(IMenu<ObjectsViewContext>))]
    internal class ObjectMenuButtonsBuilder : IMenu<ObjectsViewContext>
    {
        private const string CUSTOM_BUTTON_NAME = "CustomProperties";
        private const string CUSTOM_BUTTON_HEADER = "My Object Info";

        private const string COPY_BUTTON_NAME = "CopyButton";
        private const string COPY_BUTTON_HEADER = "Copy Ice";

        private const string ACCESS_TRANSFER_BUTTON_NAME = "AccessTransfer";
        private const string ACCESS_TRANSFER_BUTTON_HEADER = "Access Transfer";

        private IMenuItemBuilder _customButtonA;
        private IMenuItemBuilder _customButtonC;
        private IMenuItemBuilder _accessTransfer;

        private readonly IObjectModifier _objectModifier;
        private readonly IObjectsRepository _objectsRepository;
        private readonly IPilotDialogService _pilotDialogService;
        private readonly IFileProvider _fileProvider;

        private MainMenuDataGridVM _mainMenuDataGridVM;

        #region IMenu <ObjectsViewContext>

        [ImportingConstructor]
        public ObjectMenuButtonsBuilder(IObjectModifier modifier, IObjectsRepository objectsRepository, IPilotDialogService pilotDialogService, IFileProvider fileProvider)
        {
            _objectModifier = modifier;
            _objectsRepository = objectsRepository;
            _pilotDialogService = pilotDialogService;
            _fileProvider = fileProvider;
        }

        public void Build(IMenuBuilder builder, ObjectsViewContext context)
        {
            _customButtonA = builder.AddItem(CUSTOM_BUTTON_NAME, 0);
            _customButtonA.WithIcon(Properties.Resources.InfoSquare);
            _customButtonA.WithHeader(CUSTOM_BUTTON_HEADER);

            _customButtonC = builder.AddItem(COPY_BUTTON_NAME, 1);
            _customButtonC.WithIcon(Properties.Resources.InfoSquare);
            _customButtonC.WithHeader(COPY_BUTTON_HEADER);

            _accessTransfer = builder.AddItem(ACCESS_TRANSFER_BUTTON_NAME, 2);
            _accessTransfer.WithIcon(Properties.Resources.InfoSquare);
            _accessTransfer.WithHeader(ACCESS_TRANSFER_BUTTON_HEADER);
        }

        public void OnMenuItemClick(string name, ObjectsViewContext context)
        {
            try
            {
                if (name == CUSTOM_BUTTON_NAME)
                {
                    if (context.SelectedObjects.ToArray().Length > 1)
                    {
                        _mainMenuDataGridVM = new MainMenuDataGridVM(_objectModifier, _objectsRepository, _pilotDialogService, _fileProvider);
                        _mainMenuDataGridVM.OpenFormCommand.Execute(context.SelectedObjects.ToArray());
                    }
                    else if (context.SelectedObjects.ToArray().Length == 1)
                    {
                        CurrentObjectFormVM currentObjectFormVM = new CurrentObjectFormVM(_objectModifier, _objectsRepository, _fileProvider);
                        currentObjectFormVM.OpenCommand.Execute(context.SelectedObjects.ToArray()[0]);
                    }
                    
                }
                else if (name == COPY_BUTTON_NAME)
                {
                    _ = CreateCopeAsync(context.SelectedObjects.ToArray()[0]);
                }
                else if (name == ACCESS_TRANSFER_BUTTON_NAME)
                {
                    AccessTransferVM accessTransferVM = new AccessTransferVM(_objectsRepository, _objectModifier);
                    accessTransferVM.OpenWindowCommand.Execute(context.SelectedObjects.ToArray()[0].Id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private async Task CreateCopeAsync(IDataObject dataObject)
        {
            ObjectCreator objectCreator = new ObjectCreator(_objectsRepository, _objectModifier, _fileProvider);

            await objectCreator.CloneObject(dataObject);
        }
        #endregion
    }
}