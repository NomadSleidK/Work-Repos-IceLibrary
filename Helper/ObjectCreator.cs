using Ascon.Pilot.SDK;
using MyIceLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyIceLibrary.Helper
{
    public class ObjectCreator
    {
        private readonly IObjectsRepository _objectsRepository;
        private readonly IObjectModifier _objectModifier;
        private readonly IFileProvider _fileProvider;

        public ObjectCreator(IObjectsRepository objectsRepository, IObjectModifier objectModifier, IFileProvider fileProvider)
        {
            _objectsRepository = objectsRepository;
            _objectModifier = objectModifier;
            _fileProvider = fileProvider;
        }

        public async Task CloneObject(IDataObject clonedObject)
        {
            ObjectLoader objectLoader = new ObjectLoader(_objectsRepository);
            var children = await objectLoader.Load(clonedObject.Children);

            List<ChildParentRatio> ChildParentRatios = await CreateChildParentRatiosList(clonedObject, children.ToArray());
            
            await CopyObjectByRatiosList(clonedObject, ChildParentRatios);
        }

        #region Create Children List
        private async Task<List<ChildParentRatio>> CreateChildParentRatiosList(IDataObject parentObject, IDataObject[] children)
        {
            List<ChildParentRatio> resultList = new List<ChildParentRatio>();

            foreach (var child in children)
            {
                resultList.Add(new ChildParentRatio()
                {
                    ParentId = parentObject.Id,
                    ChildId = child.Id,
                });

                if (child.Children.Any())
                {
                    var nextChildren = await FindChildren(child);
                    var nextList = await CreateChildParentRatiosList(child, nextChildren);
                    resultList.AddRange(nextList);
                }
            }

            return resultList;
        }

        private async Task<IDataObject[]> FindChildren(IDataObject dataObject)
        {
            ObjectLoader objectLoader = new ObjectLoader(_objectsRepository);
            var children = await objectLoader.Load(dataObject.Children);

            return children.ToArray();
        }
        #endregion

        #region Copy Children By List
        private async Task CopyObjectByRatiosList(IDataObject mainObject, List<ChildParentRatio> guidsInfo)
        {
            ObjectLoader objectLoader = new ObjectLoader(_objectsRepository);

            var newMain = await CreateObjectCopy(mainObject, mainObject.ParentId);
            guidsInfo = await ReplaceParentGuidToNewGuid(mainObject.Id, newMain.Id, guidsInfo);

            for (int i = 0; i < guidsInfo.Count; i++)
            {
                var refObject = await objectLoader.Load(guidsInfo[i].ChildId);

                var newObject = await CreateObjectCopy(refObject, guidsInfo[i].ParentId);  
                
                guidsInfo = await ReplaceParentGuidToNewGuid(refObject.Id, newObject.Id, guidsInfo);
            }
        }

        private async Task<List<ChildParentRatio>> ReplaceParentGuidToNewGuid(Guid targetGuid, Guid newGuid, List<ChildParentRatio> guidsInfo)
        {
            for (int i = 0; i < guidsInfo.Count; i++)
            {
                if (guidsInfo[i].ParentId == targetGuid)
                {
                    guidsInfo[i].ParentId = newGuid;
                }
            }

            return guidsInfo;
        }

        private async Task<IDataObject> CreateObjectCopy(IDataObject clonedObject, Guid parentId)
        {
            IObjectBuilder objectBuilder = _objectModifier.Create(parentId, clonedObject.Type);

            var attributes = clonedObject.Attributes;
            var files = clonedObject.Files.ToArray();

            foreach (var attribute in attributes)
            {
                objectBuilder.SetAttributeAsObject(attribute.Key, attribute.Value);
            }

            foreach (var file in files)
            {
                using (var fileStream = _fileProvider.OpenRead(file))
                {
                    objectBuilder.AddFile(file.Name, fileStream,
                        DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow);
                }
            }

            _objectModifier.Apply();

            return objectBuilder.DataObject;
        }
        #endregion
    }
}
