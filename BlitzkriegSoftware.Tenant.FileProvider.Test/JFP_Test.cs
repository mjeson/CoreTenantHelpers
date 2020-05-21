using BlitzkriegSoftware.Tenant.Libary.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace BlitzkriegSoftware.Tenant.FileProvider.Test
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class JFP_Test
    {
        #region "Constants"
        private const string Test_Data_Folder = @"c:\temp\tenants";
        private const int Test_Cases_Count = 10;
        private readonly static List<string> ConfigKeys = new List<string>() { "SqlConnection", "TenantFeatures", "TenantLevel", "LogoFile" };
        #endregion

        #region "Boilerplate"

        private static TestContext Context;

        [ClassInitialize]
        public static void InitClass(TestContext testContext)
        {
            Context = testContext;

            if (Directory.Exists(Test_Data_Folder))
            {
                Directory.Delete(Test_Data_Folder, true);
            }
            Directory.CreateDirectory(Test_Data_Folder);
            MakeTenants(Test_Cases_Count);
        }
        #endregion

        #region "Helpers"

        private static void MakeTenants(int count)
        {
            var tp = new JsonFileProvider(new DirectoryInfo(Test_Data_Folder));

            for (int i = 0; i < count; i++)
            {
                var tenantId = Guid.NewGuid();
                var model = new TenantBase
                {
                    Contact = new ContactBase() { TenantId = tenantId, DisplayName = tenantId.ToString(), ContactEmail = $"{tenantId}@my.co", ContactName = $"", ContactPhone = "" }
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
                if (JFP_Test._tenants == null)
                {
                    JFP_Test._tenants = new List<Guid>();
                }
                return JFP_Test._tenants;
            }
        }

        #endregion

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_Data_Exists()
        {
            var di = new DirectoryInfo(Test_Data_Folder);
            var ct = di.GetFiles("*.json").Length;
            Assert.IsTrue(ct > 0);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_Ctor_1()
        {
            var tp = new JsonFileProvider(new DirectoryInfo(Test_Data_Folder));
            Assert.IsNotNull(tp);
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_TenantAddUpdate_1()
        {
            var tp = new JsonFileProvider(new DirectoryInfo(Test_Data_Folder));
            tp.TenantAddUpdate(null);
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_TenantAddUpdate_Bad_2()
        {
            var tp = new JsonFileProvider(new DirectoryInfo(Test_Data_Folder));
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
            var tp = new JsonFileProvider(new DirectoryInfo(Test_Data_Folder));
            var model = new TenantBase();
            tp.TenantAddUpdate(model);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_TenantExists_1()
        {
            var tp = new JsonFileProvider(new DirectoryInfo(Test_Data_Folder));
            var id = Tenants[0];
            Assert.IsTrue(tp.TenantExists(id));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_TenantGet_1()
        {
            var tp = new JsonFileProvider(new DirectoryInfo(Test_Data_Folder));
            var id = Tenants[0];
            var model = (TenantBase)tp.TenantGet(id);
            Assert.IsNotNull(model);
            Assert.AreEqual(id, model.Contact.TenantId);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_TenantGet_2()
        {
            var tp = new JsonFileProvider(new DirectoryInfo(Test_Data_Folder));
            var id = Tenants[0];
            var model = (TenantBase)tp.TenantGet(Guid.Empty);
            Assert.IsNull(model);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_TenantDelete_1()
        {
            var tp = new JsonFileProvider(new DirectoryInfo(Test_Data_Folder));
            var id = Tenants[Tenants.Count() - 2];
            var flag = tp.TenantDelete(id);
            Assert.IsTrue(flag);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_TenantDelete_2()
        {
            var tp = new JsonFileProvider(new DirectoryInfo(Test_Data_Folder));
            var id = Guid.NewGuid();
            var flag = tp.TenantDelete(id);
            Assert.IsFalse(flag);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_ConfigurationGet_1()
        {
            var tp = new JsonFileProvider(new DirectoryInfo(Test_Data_Folder));
            var id = Tenants[0];
            var model = tp.ConfigurationGet(id, ConfigKeys[0].Substring(0, 3));
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Count());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_ConfigurationGet_2()
        {
            var tp = new JsonFileProvider(new DirectoryInfo(Test_Data_Folder));
            var id = Tenants[0];
            var model = tp.ConfigurationGet(id, string.Empty);
            Assert.IsNotNull(model);
            Assert.AreEqual(4, model.Count());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_ConfigurationGet_3()
        {
            var tp = new JsonFileProvider(new DirectoryInfo(Test_Data_Folder));
            var id = Tenants[0];
            var keys = new List<string>() { "TenantLevel", "LogoFile" };
            var model = tp.ConfigurationGet(id, keys);
            Assert.IsNotNull(model);
            Assert.AreEqual( 2, model.Count());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_ConfigurationGet_4()
        {
            var tp = new JsonFileProvider(new DirectoryInfo(Test_Data_Folder));
            var id = Tenants[0];
            var keys = new List<string>();
            var model = tp.ConfigurationGet(id, keys);
            Assert.IsNotNull(model);
            Assert.AreEqual( 4, model.Count());
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_ConfigurationGet_Bad_1()
        {
            var tp = new JsonFileProvider(new DirectoryInfo(Test_Data_Folder));
            var id = Guid.Empty;
            var model = tp.ConfigurationGet(id, string.Empty);
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_ConfigurationGet_Bad_2()
        {
            var tp = new JsonFileProvider(new DirectoryInfo(Test_Data_Folder));
            var id = Guid.NewGuid();
            var model = tp.ConfigurationGet(id, string.Empty);
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_ConfigurationGet_Bad_3()
        {
            var keys = new List<string>();
            var tp = new JsonFileProvider(new DirectoryInfo(Test_Data_Folder));
            var id = Guid.Empty;
            var model = tp.ConfigurationGet(id, keys);
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_ConfigurationGet_Bad_4()
        {
            var keys = new List<string>();
            var tp = new JsonFileProvider(new DirectoryInfo(Test_Data_Folder));
            var id = Guid.NewGuid();
            var model = tp.ConfigurationGet(id, keys);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_ConfigurationUpdate_1()
        {
            var tp = new JsonFileProvider(new DirectoryInfo(Test_Data_Folder));
            var id = Tenants[0];
            var oldTenant = tp.TenantGet(id);
            var model = MakeConfigs();
            tp.ConfigurationUpdate(id, model);
            var model2 = tp.ConfigurationGet(id, string.Empty);
            foreach(var k in model)
            {
                if(!model2.Contains(k))
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
            var tp = new JsonFileProvider(new DirectoryInfo(Test_Data_Folder));
            var id = Tenants[0];
            tp.ConfigurationUpdate(id, null);
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_ConfigurationUpdate_Bad_1()
        {
            var tp = new JsonFileProvider(new DirectoryInfo(Test_Data_Folder));
            var id = Guid.Empty;
            var model = MakeConfigs();
            tp.ConfigurationUpdate(id, model);
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_ConfigurationUpdate_Bad_2()
        {
            var tp = new JsonFileProvider(new DirectoryInfo(Test_Data_Folder));
            var id = Guid.NewGuid();
            var model = tp.ConfigurationGet(id, string.Empty);
            tp.ConfigurationUpdate(id, model);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_ContactGet_1()
        {
            var tp = new JsonFileProvider(new DirectoryInfo(Test_Data_Folder));
            var id = Tenants[0];
            var model = tp.ContactGet(id);
            Assert.IsNotNull(model);
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_ContactGet_2()
        {
            var tp = new JsonFileProvider(new DirectoryInfo(Test_Data_Folder));
            var model = tp.ContactGet(Guid.Empty);
        }


        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_ContactGet_3()
        {
            var tp = new JsonFileProvider(new DirectoryInfo(Test_Data_Folder));
            var model = tp.ContactGet(Guid.NewGuid());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_ContactUpdate_1()
        {
            var tp = new JsonFileProvider(new DirectoryInfo(Test_Data_Folder));
            var id = Tenants[0];
            var model = tp.ContactGet(id);
            tp.ContactUpdate(id, model);
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_ContactUpdate_2()
        {
            var tp = new JsonFileProvider(new DirectoryInfo(Test_Data_Folder));
            var id = Guid.Empty;
            var model = new ContactBase() { TenantId = id };
            tp.ContactUpdate(id, model);
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_ContactUpdate_3()
        {
            var tp = new JsonFileProvider(new DirectoryInfo(Test_Data_Folder));
            var id = Guid.NewGuid();
            var model = new ContactBase() { TenantId = id };
            tp.ContactUpdate(id, model);
        }

        [TestMethod]
        [TestCategory("Unit")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_ContactUpdate_4()
        {
            var tp = new JsonFileProvider(new DirectoryInfo(Test_Data_Folder));
            var id = Guid.NewGuid();
            var model = tp.ContactGet(id);
        }

    }
}
