using Ascon.Pilot.SDK;
using Ascon.Pilot.SDK.Toolbar;
using MyIceLibrary.ViewModel;
using System.ComponentModel.Composition;

namespace MyIceLibrary
{
    [Export(typeof(IToolbar<ObjectsViewContext>))]
    public class ObjectToolbarButtonsBuilder : IToolbar<ObjectsViewContext>
    {
        private const string DATABASE_INFO_BUTTON_NAME = "DataBaseInfoButton";
        private const string DATABASE_INFO_BUTTON_HEADER = "Информация о базе";

        private readonly IObjectModifier _modifier;
        private readonly IObjectsRepository _objectsRepository;
        private readonly IPilotDialogService _pilotDialogService;
        private readonly IFileProvider _fileProvider;

        private IToolbarButtonItemBuilder _customButton;

        [ImportingConstructor]
        public ObjectToolbarButtonsBuilder(IObjectModifier modifier, IObjectsRepository objectsRepository, IPilotDialogService pilotDialogService, IFileProvider fileProvider)
        {
            _modifier = modifier;
            _objectsRepository = objectsRepository;
            _pilotDialogService = pilotDialogService;
            _fileProvider = fileProvider;
        }

        public void Build(IToolbarBuilder builder, ObjectsViewContext context)
        {
            _customButton = builder.AddButtonItem(DATABASE_INFO_BUTTON_NAME, 1);
            _customButton.WithIcon(Properties.Resources.DataBaseIcon);
            _customButton.WithHeader(DATABASE_INFO_BUTTON_HEADER);
        }

        public void OnToolbarItemClick(string name, ObjectsViewContext context)
        {
            if (name == DATABASE_INFO_BUTTON_NAME)
            {
                BaseStructureWindowVM access = new BaseStructureWindowVM(_objectsRepository);
                access.OpenDialogCommand.Execute(null);
            }
        }
    }
}