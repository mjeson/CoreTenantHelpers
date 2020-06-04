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

    /// <summary>
    /// Mongo User Provider Test
    /// </summary>
    [TestClass]
    public class MUP_Test
    {
        private const int Test_Cases_Count = 10;

        private static MongoTenantUserProfileProvider<TenantUserProfileBase> mtup = null;

        #region "Boilerplate"

        private static TestContext Context;

        [ClassInitialize]
        public static void InitClass(TestContext testContext)
        {
            var config = new Models.MongoConfiguration()
            {
                Database = "TenantProvider",
                Collection = "Users"
            };

            mtup = new MongoTenantUserProfileProvider<TenantUserProfileBase>(config);

            Context = testContext;
            MakeUsers(Test_Cases_Count);
        }
        #endregion

        #region "Help"

        private readonly static List<string> ConfigKeys = new List<string>() { "FirstName", "LastName", "Email", "PersonalUrl" };

        static Guid[] Tenants = new Guid[] { Guid.NewGuid(), Guid.NewGuid() };

        private static List<Guid> _users;

        public static List<Guid> Users
        {
            get
            {
                if (MUP_Test._users == null)
                {
                    MUP_Test._users = new List<Guid>();
                }
                return MUP_Test._users;
            }
        }

        private static TenantUserProfileBase MakeUser(int index)
        {
            var id = Guid.NewGuid();

            var model = new TenantUserProfileBase() { _id = id, UniqueUserId = $"{id}@ey.net" };

            if (index % 2 == 0)
            {
                model.Tenants.Add(Tenants[0]);
            }
            else
            {
                model.Tenants.Add(Tenants[1]);
            }

            foreach (var k in ConfigKeys)
            {
                model.SettingsPut(k, Faker.Lorem.Word());
            }

            return model;
        }

        private static void MakeUsers(int count)
        {
            for(int i=0; i< count; i++)
            {
                var model = MakeUser(i);
                Users.Add(model._id);
                mtup.Write(model);
            }
        }

        #endregion

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_CTOR_1()
        {
            Assert.IsNotNull(mtup);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_Exists_1()
        {
            var g = Guid.NewGuid();
            var result = mtup.Exists(g);
            Assert.IsFalse(result);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_Exists_2()
        {
            var g = MUP_Test.Users[0];
            var result = mtup.Exists(g);
            Assert.IsTrue(result);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_Exists_3()
        {
            var result = mtup.Exists("fake");
            Assert.IsFalse(result);
        }


        [TestMethod]
        [TestCategory("Unit")]
        public void Test_Exists_4()
        {
            var g = MUP_Test.Users[0];
            var u = mtup.Read(g);
            var result = mtup.Exists(u.UniqueUserId);
            Assert.IsTrue(result);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_Read_Write_1()
        {
            var u = MUP_Test.MakeUser(0);
            mtup.Write(u);
            var m = mtup.Read(u._id);
            Assert.IsNotNull(m);
            Assert.IsTrue(u.Equals(m));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_Read_2()
        {
            var g = MUP_Test.Users[0];
            var m = mtup.Read(g);
            Assert.IsNotNull(m);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_Delete_2()
        {
            var u = MUP_Test.MakeUser(0);
            var m = mtup.Delete(u._id);
            Assert.IsFalse(m);
        }


        [TestMethod]
        [TestCategory("Unit")]
        public void Test_Delete_3()
        {
            var u = MUP_Test.MakeUser(0);
            mtup.Write(u);
            var m = mtup.Delete(u._id);
            Assert.IsTrue(m);
        }

    }
}
