using Ascon.Pilot.SDK;
using System;
using System.Collections.Generic;

namespace MyIceLibrary
{
    public class ObserverFindedObjects : IObserver<IDataObject>
    {
        private List<IDataObject> _dataObjects;

        public delegate void LoadObjectsHandler(IDataObject[] dataObjects);
        private LoadObjectsHandler _loadObjects;

        public ObserverFindedObjects(LoadObjectsHandler revert)
        {
            _dataObjects = new List<IDataObject>();
            _loadObjects = revert;
        }

        public void OnCompleted()
        {

        }

        public void OnError(Exception error)
        {
            Console.WriteLine("Error: " + error.Message);
        }

        public void OnNext(IDataObject value)
        {
            _dataObjects.Add(value);
            _loadObjects(_dataObjects.ToArray());
        }

        public IEnumerable<IDataObject> GetDataObjects()
        {
            return _dataObjects;
        }
    }
}
