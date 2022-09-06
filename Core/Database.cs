namespace ServiceArchitecture.Core {

    public interface IDatabase {
        IQueryable<T> Get<T>();
        void Save(object entity);
        void Delete(object entity);
    }

    public class Database : IDatabase {
        private readonly Dictionary<Type, List<object>> _data = new();

        public IQueryable<T> Get<T>() {
            if (_data.ContainsKey(typeof(T)))
                return _data[typeof(T)].Cast<T>().AsQueryable();

            return new List<T>().AsQueryable();
        }

        public void Save(object entity) {
            if (!_data.ContainsKey(entity.GetType()))
                _data.Add(entity.GetType(), new List<object>());

            _data[entity.GetType()].Add(entity);
        }

        public void Delete(object entity) {
            if (!_data.ContainsKey(entity.GetType()))
                _data.Add(entity.GetType(), new List<object>());

            _data[entity.GetType()].Remove(entity);
        }

        public void Transaction(Action action) {

        }
    }
}