using Ascon.Pilot.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyIceLibrary
{
    public class ObjectLoader
    {
        private IObjectsRepository _repository;

        public ObjectLoader(IObjectsRepository repository)
        {
            _repository = repository;
        }

        public async Task<IDataObject> Load(Guid id, long changesetId = 0)
        {
            var result = await Load(new List<Guid> { id }, changesetId);
            return result.First();
        }

        public Task<IEnumerable<IDataObject>> Load(IEnumerable<Guid> ids, long changesetId = 0)
        {
            return new NestedObjectLoader(_repository).Load(ids, changesetId);
        }

        private class NestedObjectLoader : IObserver<IDataObject>
        {
            private readonly IObjectsRepository _repository;
            private List<Guid> _toLoad;
            private List<IDataObject> _loaded = new List<IDataObject>();
            private TaskCompletionSource<IEnumerable<IDataObject>> _tcs;
            private IDisposable _subscription;
            private long _changesetId;

            public NestedObjectLoader(IObjectsRepository repository)
            {
                _repository = repository;
            }

            public Task<IEnumerable<IDataObject>> Load(IEnumerable<Guid> ids, long changesetId = 0)
            {
                _toLoad = ids.ToList();
                _changesetId = changesetId;
                _tcs = new TaskCompletionSource<IEnumerable<IDataObject>>();
                _subscription = _repository.SubscribeObjects(ids).Subscribe(this);
                return _tcs.Task;
            }
            public void OnNext(IDataObject value)
            {
                if (value.State != DataState.Loaded)
                    return;

                if (value.LastChange() < _changesetId)
                    return;

                _loaded.Add(value);
                _toLoad.Remove(value.Id);

                if (_toLoad.Any())
                    return;

                _subscription.Dispose();
                _tcs.TrySetResult(_loaded);
            }

            public void OnCompleted() { }

            public void OnError(Exception error) { }

        }
    }
}
