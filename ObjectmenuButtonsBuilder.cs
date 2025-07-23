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
        private const string INFO_BUTTON_NAME = "ObjectInfoButton";
        private const string INFO_BUTTON_HEADER = "Информация об объекте";

        private const string COPY_BUTTON_NAME = "CopyButton";
        private const string COPY_BUTTON_HEADER = "Копировать объект";

        private const string ACCESS_MODIFIER_BUTTON_NAME = "AccessModifierButton";
        private const string ACCESS_MODIFIER_BUTTON_HEADER = "Передать права наверх";

        private IMenuItemBuilder _customButtonA;
        private IMenuItemBuilder _customButtonC;
        private IMenuItemBuilder _accessTransfer;

        private readonly IObjectModifier _objectModifier;
        private readonly IObjectsRepository _objectsRepository;
        private readonly IPilotDialogService _pilotDialogService;
        private readonly IFileProvider _fileProvider;

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
            _customButtonA = builder.AddItem(INFO_BUTTON_NAME, builder.Count);
            _customButtonA.WithIcon(Properties.Resources.InfoIconB);
            _customButtonA.WithHeader(INFO_BUTTON_HEADER);

            _customButtonC = builder.AddItem(COPY_BUTTON_NAME, builder.Count);
            _customButtonC.WithIcon(Properties.Resources.CopyIcon);
            _customButtonC.WithHeader(COPY_BUTTON_HEADER);

            _accessTransfer = builder.AddItem(ACCESS_MODIFIER_BUTTON_NAME, builder.Count);
            _accessTransfer.WithIcon(Properties.Resources.AccessIcon);
            _accessTransfer.WithHeader(ACCESS_MODIFIER_BUTTON_HEADER);
        }

        public void OnMenuItemClick(string name, ObjectsViewContext context)
        {
            try
            {
                if (name == INFO_BUTTON_NAME && context.SelectedObjects.ToArray().Length > 0)
                {
                    CurrentObjectFormVM currentObjectFormVM = new CurrentObjectFormVM(_objectModifier, _objectsRepository, _fileProvider);
                    currentObjectFormVM.OpenCommand.Execute(context.SelectedObjects.ToArray()[0]);
                }
                else if (name == COPY_BUTTON_NAME)
                {
                    _ = CreateCopeAsync(context.SelectedObjects.ToArray()[0]);
                }
                else if (name == ACCESS_MODIFIER_BUTTON_NAME)
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