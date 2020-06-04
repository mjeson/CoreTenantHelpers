using BlitzkriegSoftware.Tenant.Libary;
using BlitzkriegSoftware.Tenant.Libary.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace BlitzkriegSoftware.Tenant.MongoProvider
{
    /// <summary>
    /// Tenant Data Provider: MongoDB
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MongoTenantDataProvider<T> : ITenantDataProvider<T> where T : ITenantModel, new()
    {
        #region "publics, etc."
        private readonly Models.MongoConfiguration _config;
        #endregion

        #region "ctor"
        [ExcludeFromCodeCoverage] 
        private MongoTenantDataProvider() { }

        /// <summary>
        /// CTOR
        /// </summary>
        public MongoTenantDataProvider(Models.MongoConfiguration config)
        {
            this._config = config;
        }

        #endregion

        #region "Helpers"

        /// <summary>
        /// Mongo Root DB
        /// </summary>
        public const string Mongo_Root_DB = "admin";

        /// <summary>
        /// Get Tenant Collection
        /// </summary>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        public IMongoCollection<T> MongoConnection()
        {
            var settings = new MongoClientSettings();

            if (!string.IsNullOrWhiteSpace(this._config.Username))
            {

                var internalIdentity = new MongoInternalIdentity(Mongo_Root_DB, this._config.Username);

                var passwordEvidence = new PasswordEvidence(this._config.Password);

                var mongoCredential = new MongoCredential(
                             this._config.AuthMechanism,
                             internalIdentity,
                             passwordEvidence);

                settings.Credential = mongoCredential;
            }

            var address = new MongoServerAddress(this._config.Host);
            settings.Server = address;

            var client = new MongoClient(settings);

            if (!BsonClassMap.IsClassMapRegistered(typeof(ContactBase)))
            {
                BsonClassMap.RegisterClassMap<ContactBase>();
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(TenantBase)))
            {
                BsonClassMap.RegisterClassMap<TenantBase>();
            }

            var db = client.GetDatabase(this._config.Database);
            var coll = db.GetCollection<T>(this._config.Collection);

            return coll;
        }

        #endregion

        #region "Read/Write"

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="id">Key</param>
        /// <returns>Tenant of Type T</returns>
        [ExcludeFromCodeCoverage]
        public T Read(Guid id)
        {
            var model = default(T);
            var coll = this.MongoConnection();
            model = coll.Find(f => f._id == id).FirstOrDefault();
            return model;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="model">Tenant of Type T</param>
        [ExcludeFromCodeCoverage]
        public void Write(T model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Configuration == null)
            {
                model.Configuration = new List<KeyValuePair<string, string>>();
            }

            if (model.Contact == null)
            {
                model.Contact = new ContactBase();
            }

            if (model._id == Guid.Empty)
            {
                throw new ArgumentException($"A model must contain a `TenantId`", nameof(model));
            }

            var coll = this.MongoConnection();

            var replaceOneResult = coll.ReplaceOneAsync(
                f => f._id == model._id,
                model,
                new ReplaceOptions { IsUpsert = true }).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Exists
        /// </summary>
        /// <param name="id">Key</param>
        /// <returns>True if so</returns>
        [ExcludeFromCodeCoverage]
        public bool Exists(Guid id)
        {
            var coll = this.MongoConnection();
            T model = coll.Find(f => f._id == id).FirstOrDefault();
            return (model != null);
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id">Key</param>
        /// <returns>True if so</returns>
        [ExcludeFromCodeCoverage]
        public bool Delete(Guid id)
        {
            var coll = this.MongoConnection();
            var result = coll.DeleteOneAsync(f => f._id == id).GetAwaiter().GetResult();
            return result.IsAcknowledged && (result.DeletedCount > 0);
        }

        #endregion

    }
}
