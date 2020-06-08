using BlitzkriegSoftware.Tenant.Libary;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace BlitzkriegSoftware.Tenant.MongoProvider
{
    /// <summary>
    /// Mongo: Tenant User Provider
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MongoTenantUserProfileProvider<T> : ITenantUserProfileProvider<T> where T : ITenantUserProfile, new()
    {
        #region "publics, etc."
        private readonly Models.MongoConfiguration _config;
        #endregion

        #region "ctor"
        [ExcludeFromCodeCoverage] 
        private MongoTenantUserProfileProvider() { }

        /// <summary>
        /// CTOR
        /// </summary>
        public MongoTenantUserProfileProvider(Models.MongoConfiguration config)
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

            if (!BsonClassMap.IsClassMapRegistered(typeof(TenantUserProfileBase)))
            {
                BsonClassMap.RegisterClassMap<TenantUserProfileBase>();
            }

            var db = client.GetDatabase(this._config.Database);
            var coll = db.GetCollection<T>(this._config.Collection);

            return coll;
        }

        #endregion

        /// <summary>
        /// Read a UserProfile
        /// </summary>
        /// <param name="uniqueUserId">User Id from AuthN</param>
        /// <returns>UserProfile of Type T or Null</returns>
        [ExcludeFromCodeCoverage]
        public T Read(string uniqueUserId)
        {
            var model = default(T);
            var coll = this.MongoConnection();
            model = coll.Find(f => f.UniqueUserId == uniqueUserId).FirstOrDefault();
            return model;
        }

        /// <summary>
        /// Read a UserProfile
        /// </summary>
        /// <param name="id">Key</param>
        /// <returns>UserProfile of Type T or Null</returns>
        [ExcludeFromCodeCoverage]
        public T Read(Guid id)
        {
            var model = default(T);
            var coll = this.MongoConnection();
            model = coll.Find(f => f._id == id).FirstOrDefault();
            return model;
        }

        /// <summary>
        /// Write UserProfile
        /// </summary>
        /// <param name="model">UserProfile of Type T</param>
        [ExcludeFromCodeCoverage]
        public void Write(T model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Tenants == null)
            {
                model.Tenants = new List<Guid>();
            }

            if (model.Settings == null)
            {
                model.Settings = new Dictionary<string, object>();
            }

            if (string.IsNullOrWhiteSpace(model.UniqueUserId))
            {
                throw new ArgumentException($"A model must contain a `UniqueUserId`", nameof(model));
            }

            var coll = this.MongoConnection();

            var replaceOneResult = coll.ReplaceOneAsync(
                f => f._id == model._id,
                model,
                new ReplaceOptions { IsUpsert = true }).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Does UserProfile Exist?
        /// </summary>
        /// <param name="uniqueUserId">User Id from AuthN</param>
        /// <returns>True if so</returns>
        [ExcludeFromCodeCoverage]
        public bool Exists(string uniqueUserId)
        {
            var coll = this.MongoConnection();
            T model = coll.Find(f => f.UniqueUserId == uniqueUserId).FirstOrDefault();
            return (model != null);
        }

        /// <summary>
        /// Does UserProfile Exist?
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
        /// Delete a UserProfile
        /// <para>Warning Destructive</para>
        /// </summary>
        /// <param name="uniqueUserId">Key</param>
        /// <returns>True if so</returns>
        [ExcludeFromCodeCoverage]
        public bool Delete(string uniqueUserId)
        {
            var coll = this.MongoConnection();
            var result = coll.DeleteOneAsync(f => f.UniqueUserId == uniqueUserId).GetAwaiter().GetResult();
            return result.IsAcknowledged && (result.DeletedCount > 0);
        }

        /// <summary>
        /// Delete a UserProfile
        /// <para>Warning Destructive</para>
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
    }
}
