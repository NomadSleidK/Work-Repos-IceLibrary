using Ascon.Pilot.SDK;
using Ascon.Pilot.SDK.Menu;
using MyIceLibrary.Helper;
using MyIceLibrary.ViewModel;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace MyIceLibrary
{
    [Export(typeof(IMenu<ObjectsViewContext>))]
    internal class ObjectmenuButtonsBuilder : IMenu<ObjectsViewContext>
    {
        private const string CUSTOM_BUTTON_NAME = "CustomProperties";
        private const string CUSTOM_BUTTON_HEADER = "My Object Info";

        private const string COPY_BUTTON_NAME = "CopyButton";
        private const string COPY_BUTTON_HEADER = "Copy Ice";

        private IMenuItemBuilder _customButtonA;
        private IMenuItemBuilder _customButtonC;

        private readonly IObjectModifier _modifier;
        private readonly IObjectsRepository _objectsRepository;
        private readonly IPilotDialogService _pilotDialogService;
        private readonly IFileProvider _fileProvider;

        private MainMenuDataGridVM _mainMenuDataGridVM;

        #region IMenu <ObjectsViewContext>

        [ImportingConstructor]
        public ObjectmenuButtonsBuilder(IObjectModifier modifier, IObjectsRepository objectsRepository, IPilotDialogService pilotDialogService, IFileProvider fileProvider)
        {
            _modifier = modifier;
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
                else if (name == COPY_BUTTON_NAME)
                {
                    _ = CreateCopeAsync(context.SelectedObjects.ToArray()[0]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private async Task CreateCopeAsync(IDataObject dataObject)
        {
            ObjectCreator objectCreator = new ObjectCreator(_objectsRepository, _modifier, _fileProvider);

            await objectCreator.CloneObject(dataObject);
        }
        #endregion
    }
}