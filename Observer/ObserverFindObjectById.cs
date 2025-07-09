using Ascon.Pilot.SDK;
using System;

namespace MyIceLibrary
{
    public class ObserverFindObjectById : IObserver<IDataObject>
    {
        private IDataObject _dataObject;

        public delegate void LoadObjectsHandler(IDataObject dataObjects);
        private LoadObjectsHandler _returnObjectDelegate;

        public ObserverFindObjectById(LoadObjectsHandler loadObject)
        {
            _returnObjectDelegate = loadObject;
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
            _dataObject = value;
            _returnObjectDelegate(_dataObject);
        }
    }
}
