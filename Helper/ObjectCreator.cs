using Ascon.Pilot.SDK;
using MyIceLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyIceLibrary.Helper
{
    public class ObjectCreator
    {
        public struct GuidInfo
        {
            public Guid ParentId { get; set; }
            public Guid ChildId { get; set; }
        }

        private readonly IObjectsRepository _objectsRepository;
        private readonly IObjectModifier _objectModifier;
        private readonly IFileProvider _fileProvider;

        private List<GuidInfo> _parentToChildDictionary;

        public ObjectCreator(IObjectsRepository objectsRepository, IObjectModifier objectModifier, IFileProvider fileProvider)
        {
            _objectsRepository = objectsRepository;
            _objectModifier = objectModifier;
            _fileProvider = fileProvider;

            _parentToChildDictionary = new List<GuidInfo>();
        }

        //public async Task CopyObject(IDataObject dataObject, Guid parentId)
        //{
        //    IObjectBuilder objectBuilder = _objectModifier.Create(parentId, dataObject.Type);

        //    var attributes = dataObject.Attributes;
        //    var files = dataObject.Files.ToArray();

        //    foreach ( var attribute in attributes)
        //    {
        //        objectBuilder.SetAttributeAsObject(attribute.Key, attribute.Value);
        //    }

        //    _objectModifier.Apply();

        //    foreach (var file in files)
        //    {
        //        using (var fileStream = _fileProvider.OpenRead(file))
        //        {
        //            objectBuilder.AddFile(file.Name, fileStream, 
        //                System.DateTime.UtcNow, System.DateTime.UtcNow, System.DateTime.UtcNow);
        //        }
        //    }

        //    _objectModifier.Apply();

        //    if (dataObject.Children.Any())
        //    {
        //        var objectLoader = new ObjectLoader(_objectsRepository);
        //        var children = await objectLoader.Load(dataObject.Children.ToArray());

        //        foreach (var child in children)
        //        {
        //            await WaitUntilRunAsync(child);
        //            await WaitUntilRunAsync(objectBuilder.DataObject);

        //            await CopyObject(child, objectBuilder.DataObject.Id);


        //        }
            
        //    }

        //    await WaitUntilRunAsync(objectBuilder.DataObject);
        //}

        public async Task Loader(IDataObject currentDataObject)
        {
            await CreateDictionary(currentDataObject);
            await CloneObject(currentDataObject);
        }

        private async Task CreateDictionary(IDataObject currentDataObject)
        {
            var objectLoader = new ObjectLoader(_objectsRepository);
            var children = await objectLoader.Load(currentDataObject.Children.ToArray());

            DereAsync(currentDataObject);
        }

        private async void DereAsync(IDataObject currentDataObject)
        {
            var children = currentDataObject.Children.ToArray();

            foreach (var child in children)
            {
                _parentToChildDictionary.Add( new GuidInfo() { ParentId = currentDataObject.Id, ChildId = child });
                await Reload(child);
            }

            List<GuidInfo> parentToChildDictionary = _parentToChildDictionary.ToList();
        }

        private async Task Reload(Guid guid)
        {
            var objectLoader = new ObjectLoader(_objectsRepository);
            var children = await objectLoader.Load(guid);
            DereAsync(children);
        }

        private async Task CloneObject(IDataObject mainObject)
        {
            CreateCopy(mainObject, mainObject.ParentId);

            var objectLoader = new ObjectLoader(_objectsRepository);

            foreach(var parentKey in _parentToChildDictionary)
            {
                var dataObject = await objectLoader.Load(parentKey.ParentId);
                CreateCopy(dataObject, parentKey.ParentId);
            }
        }

        private void CreateCopy(IDataObject currentDataObject, Guid parentId)
        {
            IObjectBuilder objectBuilder = _objectModifier.Create(parentId, currentDataObject.Type);

            var attributes = currentDataObject.Attributes;
            var files = currentDataObject.Files.ToArray();

            foreach (var attribute in attributes)
            {
                objectBuilder.SetAttributeAsObject(attribute.Key, attribute.Value);
            }

            _objectModifier.Apply();

            foreach (var file in files)
            {
                using (var fileStream = _fileProvider.OpenRead(file))
                {
                    objectBuilder.AddFile(file.Name, fileStream,
                        System.DateTime.UtcNow, System.DateTime.UtcNow, System.DateTime.UtcNow);
                }
            }

            _objectModifier.Apply();
        }

        #region Wait Stoper
        public async Task WaitUntilRunAsync(IDataObject instance, CancellationToken cancellationToken = default)
        {
            while (instance.State != DataState.Loaded)
            {
                await Task.Delay(100, cancellationToken); // Асинхронная пауза
            }
        }

        public async Task WaitUntilSinhronAsync(IDataObject instance, CancellationToken cancellationToken = default)
        {
            while (instance.SynchronizationState != SynchronizationState.Synchronized)
            {
                await Task.Delay(100, cancellationToken); // Асинхронная пауза
            }
        }
        #endregion
    }
}
