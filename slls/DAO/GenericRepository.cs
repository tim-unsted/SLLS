using System;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using slls.Models;

namespace slls.DAO
{
    public class GenericRepository : IRepository
    {
        private readonly Type _entityType;
        private readonly DbEntities db;
        string _errorMessage = string.Empty;


        //Set the database context: This is the same as the connection string in th AppConfig file.
        public GenericRepository(Type entityType)
        {
            _entityType = entityType;
            db = new DbEntities();
        }


        //Get a list of all entity records ...
        public IQueryable<_entityType> GetAll<_entityType>() where _entityType : class
        {
            return db.Set<_entityType>();
        }


        //Insert a new entity record ...
        public void Insert<_entityType>(_entityType entity) where _entityType : class
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                db.Set<_entityType>().Add(entity);
                this.db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        _errorMessage += string.Format("Property: {0} Error: {1}",
                        validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;
                    }
                }
                throw new Exception(_errorMessage, dbEx);
            }
        }


        //Update an entity record ...
        public void Update<T>(T entity) where T : class
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        _errorMessage += Environment.NewLine + string.Format("Property: {0} Error: {1}",
                        validationError.PropertyName, validationError.ErrorMessage);
                    }
                }

                throw new Exception(_errorMessage, dbEx);
            }
        }


        //Delete an entity record ...
        public void Delete<T>(T entity) where T : class
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                db.Set<T>().Remove(entity);
                db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        _errorMessage += Environment.NewLine + string.Format("Property: {0} Error: {1}",
                        validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                throw new Exception(_errorMessage, dbEx);
            }
        }


        //Search for one or a set of entity records, returning an IQueryable list ...
        public IQueryable<T> SearchFor<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            throw new NotImplementedException();
        }

        //Return an entity by ID  (int) ...
        public T GetById<T>(int id) where T : class
        {
            return db.Set<T>().Find(id);
        }

        //Return an entity by ID (string) ...
        public T GetByString<T>(string id) where T : class
        {
            return db.Set<T>().Find(id);
        }

        //Clean up ...
        public void Dispose()
        {
            if (db != null) db.Dispose();
        }

    }
}
