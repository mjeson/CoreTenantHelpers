using BlitzkriegSoftware.Tenant.Libary;
using BlitzkriegSoftware.Tenant.Libary.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlitzkriegSoftware.Tenant.MongoProvider.Test
{
    [TestClass]
    public class MTP_Test
    {
        #region "Constants and Tenant Providers"
        private const int Test_Cases_Count = 10;
        private readonly static List<string> ConfigKeys = new List<string>() { "MongoConnection", "TenantFeatures", "TenantLevel", "LogoFile" };

        private static TenantProvider<TenantBase> tp = null;
        #endregion

        #region "Boilerplate"

        private static TestContext Context;

        [ClassInitialize]
        public static void InitClass(TestContext testContext)
        {
            var config = new Models.MongoConfiguration()
            {
                Database = "TenantProvider",
                Collection = "Tenants"
            };

            var tdp = new MongoTenantDataProvider<TenantBase>(config);
            tp = new TenantProvider<TenantBase>(tdp);

            Context = testContext;
            MakeTenants(Test_Cases_Count);
        }
        #endregion

        #region "Helpers"

        private static void MakeTenants(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var tenantId = Guid.NewGuid();
                var model = new TenantBase
                {
                    _id = tenantId,
                    Contact = new ContactBase() { DisplayName = tenantId.ToString(), ContactEmail = $"{tenantId}@my.co", ContactName = $"", ContactPhone = "" }
                };
                var config = new List<KeyValuePair<string, string>>();
                foreach (var k in ConfigKeys)
                {
                    var kv = new KeyValuePair<string, string>(k, Faker.Lorem.Word());
                    config.Add(kv);
                }
                model.Configuration = MakeConfigs();
                Tenants.Add(tenantId);
                tp.TenantAddUpdate(model);
            }
        }

        private static List<KeyValuePair<string, string>> MakeConfigs()
        {
            var config = new List<KeyValuePair<string, string>>();
            foreach (var k in ConfigKeys)
            {
                var kv = new KeyValuePair<string, string>(k, Faker.Lorem.Word());
                config.Add(kv);
            }
            return config;
        }


        private static List<Guid> _tenants;

        public static List<Guid> Tenants
        {
            get
            {
                if (MTP_Test._tenants == null)
                {
                    MTP_Test._tenants = new List<Guid>();
                }
                return MTP_Test._tenants;
            }
        }

        #endregion

        #region "Tests"

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_Ctor_1()
        {
            Assert.IsNotNull(tp);
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_TenantAddUpdate_1()
        {
            tp.TenantAddUpdate(null);
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_TenantAddUpdate_Bad_2()
        {
            var model = new TenantBase();
            var id = Guid.Empty;
            model.Contact = new ContactBase() { TenantId = id, DisplayName = id.ToString() };
            tp.TenantAddUpdate(model);
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_TenantAddUpdate_Bad_3()
        {
            var model = new TenantBase();
            tp.TenantAddUpdate(model);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_TenantExists_1()
        {
            var id = Tenants[0];
            Assert.IsTrue(tp.TenantExists(id));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_TenantExists_Bad_1()
        {
            var id = Guid.Empty;
            Assert.IsFalse(tp.TenantExists(id));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_TenantExists_Bad_2()
        {
            var id = Guid.NewGuid();
            Assert.IsFalse(tp.TenantExists(id));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_TenantGet_1()
        {
            var id = Tenants[0];
            var model = (TenantBase)tp.TenantGet(id);
            Assert.IsNotNull(model);
            Assert.AreEqual(id, model._id);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_TenantGet_2()
        {
            var model = (TenantBase)tp.TenantGet(Guid.Empty);
            Assert.IsNull(model);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_TenantDelete_1()
        {
            var id = Tenants[Tenants.Count() - 2];
            var flag = tp.TenantDelete(id);
            Assert.IsTrue(flag);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_TenantDelete_2()
        {
            var id = Guid.NewGuid();
            var flag = tp.TenantDelete(id);
            Assert.IsFalse(flag);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_ConfigurationGet_1()
        {
            var id = Tenants[0];
            var model = tp.ConfigurationGet(id, ConfigKeys[0].Substring(0, 3));
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Count());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_ConfigurationGet_2()
        {
            var id = Tenants[0];
            var model = tp.ConfigurationGet(id, string.Empty);
            Assert.IsNotNull(model);
            Assert.AreEqual(4, model.Count());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_ConfigurationGet_3()
        {
            var id = Tenants[0];
            var keys = new List<string>() { "TenantLevel", "LogoFile" };
            var model = tp.ConfigurationGet(id, keys);
            Assert.IsNotNull(model);
            Assert.AreEqual(2, model.Count());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_ConfigurationGet_4()
        {
            var id = Tenants[0];
            var keys = new List<string>();
            var model = tp.ConfigurationGet(id, keys);
            Assert.IsNotNull(model);
            Assert.AreEqual(4, model.Count());
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_ConfigurationGet_Bad_1()
        {
            var id = Guid.Empty;
            _ = tp.ConfigurationGet(id, string.Empty);
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_ConfigurationGet_Bad_2()
        {
            var id = Guid.NewGuid();
            _ = tp.ConfigurationGet(id, string.Empty);
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_ConfigurationGet_Bad_3()
        {
            var keys = new List<string>();
            var id = Guid.Empty;
            _ = tp.ConfigurationGet(id, keys);
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_ConfigurationGet_Bad_4()
        {
            var keys = new List<string>();
            var id = Guid.NewGuid();
            _ = tp.ConfigurationGet(id, keys);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_ConfigurationUpdate_1()
        {
            var id = Tenants[0];
            var model = MakeConfigs();
            tp.ConfigurationUpdate(id, model);
            var model2 = tp.ConfigurationGet(id, string.Empty);
            foreach (var k in model)
            {
                if (!model2.Contains(k))
                {
                    Assert.Fail($"Missing Key {k}");
                }
            }
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_ConfigurationUpdate_Bad_0()
        {
            var id = Tenants[0];
            tp.ConfigurationUpdate(id, null);
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_ConfigurationUpdate_Bad_1()
        {
            var id = Guid.Empty;
            var model = MakeConfigs();
            tp.ConfigurationUpdate(id, model);
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_ConfigurationUpdate_Bad_2()
        {
            var id = Guid.NewGuid();
            var model = tp.ConfigurationGet(id, string.Empty);
            tp.ConfigurationUpdate(id, model);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_ContactGet_1()
        {
            var id = Tenants[0];
            var model = tp.ContactGet(id);
            Assert.IsNotNull(model);
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_ContactGet_2()
        {
            _ = tp.ContactGet(Guid.Empty);
        }


        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_ContactGet_3()
        {
            _ = tp.ContactGet(Guid.NewGuid());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_ContactUpdate_1()
        {
            var id = Tenants[0];
            var model = tp.ContactGet(id);
            tp.ContactUpdate(id, model);
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_ContactUpdate_2()
        {
            var id = Guid.Empty;
            var model = new ContactBase() { TenantId = id };
            tp.ContactUpdate(id, model);
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_ContactUpdate_3()
        {
            var id = Guid.NewGuid();
            var model = new ContactBase() { TenantId = id };
            tp.ContactUpdate(id, model);
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_ContactUpdate_4()
        {
            var id = Guid.NewGuid();
            _ = tp.ContactGet(id);
        }

        #endregion

    }
}
