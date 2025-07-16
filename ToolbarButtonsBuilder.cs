using Ascon.Pilot.SDK;
using Ascon.Pilot.SDK.Toolbar;
using MyIceLibrary.ViewModel;
using System.ComponentModel.Composition;

namespace MyIceLibrary
{
    [Export(typeof(IToolbar<ObjectsViewContext>))]
    public class ToolbarButtonsBuilder : IToolbar<ObjectsViewContext>
    {
        private const string CUSTOM_B_BUTTON_NAME = "AccessTree";
        private const string CUSTOM_B_BUTTON_HEADER = "Информация о базе";

        private readonly IObjectModifier _modifier;
        private readonly IObjectsRepository _objectsRepository;
        private readonly IPilotDialogService _pilotDialogService;
        private readonly IFileProvider _fileProvider;

        private IToolbarButtonItemBuilder _customButton;

        [ImportingConstructor]
        public ToolbarButtonsBuilder(IObjectModifier modifier, IObjectsRepository objectsRepository, IPilotDialogService pilotDialogService, IFileProvider fileProvider)
        {
            _modifier = modifier;
            _objectsRepository = objectsRepository;
            _pilotDialogService = pilotDialogService;
            _fileProvider = fileProvider;
        }

        public void Build(IToolbarBuilder builder, ObjectsViewContext context)
        {
            _customButton = builder.AddButtonItem(CUSTOM_B_BUTTON_NAME, 1);
            _customButton.WithIcon(Properties.Resources.InfoSquare);
            _customButton.WithHeader(CUSTOM_B_BUTTON_HEADER);
        }

        public void OnToolbarItemClick(string name, ObjectsViewContext context)
        {
            if (name == CUSTOM_B_BUTTON_NAME)
            {
                BaseStructureWindowVM access = new BaseStructureWindowVM(_objectsRepository);
                access.OpenDialogCommand.Execute(null);
            }
        }
    }
}