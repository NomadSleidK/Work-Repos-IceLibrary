using Ascon.Pilot.SDK;
using Ascon.Pilot.SDK.Menu;
using MyIceLibrary.View;
using MyIceLibrary.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace MyIceLibrary
{
    [Export(typeof(IMenu<ObjectsViewContext>))]
    public class App : IMenu<ObjectsViewContext>
    {
        private const string CUSTOM_BUTTON_NAME = "CustomProperties";
        private const string CUSTOM_BUTTON_HEADER = "My Object Info";

        private IMenuItemBuilder _customButton;
        private IObjectModifier _modifier;
        private IObjectsRepository _objectsRepository;

        private MainMenuDataGridVM _mainMenuDataGridVM;

        #region IMenu <ObjectsViewContext>

        [ImportingConstructor]
        public App(IObjectModifier modifier, IObjectsRepository objectsRepository)
        {
            _modifier = modifier;
            _objectsRepository = objectsRepository;
            _mainMenuDataGridVM = new MainMenuDataGridVM(modifier, objectsRepository);
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
                    _mainMenuDataGridVM.OpenMainInfoFormCommand.Execute(context.SelectedObjects.ToArray());

                    //OnInfoButtonClick(context);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        #endregion

        //#region Click Reactions
        //private void OnInfoButtonClick(ObjectsViewContext context)
        //{
        //    IDataObject[] objects = context.SelectedObjects.ToArray();

        //    OpenMainWindow(objects);
        //}

        //private void OpenMainWindow(IDataObject[] objects)
        //{
        //    IPerson person = _objectsRepository.GetCurrentPerson();

        //    MainForm mainWindow = new MainForm();

        //    if (person.IsAdmin)
        //    {
        //        mainWindow.OnDeleteButtonClick += OnDeleteByIdHandler;
        //        mainWindow.OnOpenNewWindow += OpenCurrentObjectForm;
        //    }

        //    mainWindow.ShowDialog();
        //}

        //private void OpenCurrentObjectForm(Guid objectGuid)
        //{
        //    Guid[] guids = new Guid[] { objectGuid };

        //    var dataObjects = _objectsRepository.SubscribeObjects(guids);
        //    ObserverFindedObjects observer = new ObserverFindedObjects(OnObjectsFind);          
        //    dataObjects.Subscribe(observer);
        //}

        //private void OnObjectsFind(IDataObject[] obj)
        //{
        //    CurrentObjectForm currentObjectForm = new CurrentObjectForm(obj);

        //    Guid[] guids = new Guid[] { obj[0].ParentId };

        //    var dataObjects = _objectsRepository.SubscribeObjects(guids);
        //    ObserverFindedObjects observer = new ObserverFindedObjects(currentObjectForm.SetParentObject);

        //    currentObjectForm.OnOpenNewWindow += OpenCurrentObjectForm;

        //    dataObjects.Subscribe(observer);

        //    currentObjectForm.ShowDialog();
        //}

        //private void OnDeleteByIdHandler(IEnumerable<Guid> selectedGuids)
        //{
        //    try
        //    {
        //        foreach (var obj in selectedGuids)
        //        {
        //            _modifier.DeleteById(obj);
        //            _modifier.Apply();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Error: " + ex.Message);
        //    }
        //    finally
        //    {
        //        System.Windows.MessageBox.Show($"{selectedGuids.Count()} Объекта успешно удалены ");
        //        //_mainWindow?.Close();
        //    }
        //}
        //#endregion
    }
}