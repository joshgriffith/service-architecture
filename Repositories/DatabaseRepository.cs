using System;
using ServiceArchitecture.Core;

namespace ServiceArchitecture.Repositories {

    public interface IRepository<T> {
        IQueryable<T> Get();
        T? Find(string id);
        void Save(T entity);
        void Delete(T entity);
    }

    public class DatabaseRepository<T> : IRepository<T> where T : IEntity {
        protected readonly IDatabase Database;

        public DatabaseRepository(IDatabase database) {
            Database = database;
        }

        public virtual IQueryable<T> Get() {
            return Database.Get<T>();
        }

        public virtual T? Find(string id) {
            return Get().FirstOrDefault(each => each.Id == id);
        }

        public virtual void Save(T entity) {
            Database.Save(entity);
        }

        public virtual void Delete(T entity) {
            Database.Delete(entity);
        }
    }
}