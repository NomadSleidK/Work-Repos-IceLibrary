using Ascon.Pilot.SDK;
using Ascon.Pilot.SDK.Menu;
using MyIceLibrary.ViewModel;
using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace MyIceLibrary
{
    [Export(typeof(IMenu<TasksViewContext2>))]
    public class TaskToolbarButtonsBuilder : IMenu<TasksViewContext2>
    {
        private const string CUSTOM_BUTTON_NAME = "TaskInfoButton";
        private const string CUSTOM_BUTTON_HEADER = "Информация об объекте";

        private readonly IObjectModifier _modifier;
        private readonly IObjectsRepository _objectsRepository;
        private readonly IPilotDialogService _pilotDialogService;
        private readonly IFileProvider _fileProvider;

        [ImportingConstructor]
        public TaskToolbarButtonsBuilder(IObjectModifier modifier, IObjectsRepository objectsRepository, IPilotDialogService pilotDialogService, IFileProvider fileProvider)
        {
            _modifier = modifier;
            _objectsRepository = objectsRepository;
            _pilotDialogService = pilotDialogService;
            _fileProvider = fileProvider;
        }

        public void Build(IMenuBuilder builder, TasksViewContext2 context)
        {
            if (_objectsRepository.GetCurrentPerson().IsAdmin)
            {
                var infoButton = builder.AddItem(CUSTOM_BUTTON_NAME, 0);
                infoButton.WithIcon(Properties.Resources.InfoSquare);
                infoButton.WithHeader(CUSTOM_BUTTON_HEADER);
            }          
        }

        public void OnMenuItemClick(string name, TasksViewContext2 context)
        {
            try
            {
                if (name == CUSTOM_BUTTON_NAME && context.SelectedTasks.ToArray().Length > 1)
                {
                    CurrentObjectFormVM currentObjectFormVM = new CurrentObjectFormVM(_modifier, _objectsRepository, _fileProvider);
                    currentObjectFormVM.OpenCommand.Execute(context.SelectedTasks.ToArray()[0]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
