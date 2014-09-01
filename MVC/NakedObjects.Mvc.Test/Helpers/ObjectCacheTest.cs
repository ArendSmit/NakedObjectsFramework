﻿// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Expenses.ExpenseClaims;
using Expenses.ExpenseClaims.Items;
using Expenses.Fixtures;
using Expenses.RecordedActions;
using Expenses.Services;
using MvcTestApp.Tests.Util;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;
using NakedObjects.Boot;
using NakedObjects.Core.Context;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Core.Persist;
using NakedObjects.Persistor.Objectstore.Inmemory;
using NakedObjects.Web.Mvc;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Xat;
using NUnit.Framework;

namespace MvcTestApp.Tests.Helpers {
    [TestFixture]
    public class ObjectCacheTest : AcceptanceTestCase {
        #region Setup/Teardown

        [TestFixtureSetUp]
        public void SetupTest() {
            InitializeNakedObjectsFramework();

        }

        [TestFixtureTearDown]
        public void TearDownTest() {
            CleanupNakedObjectsFramework();
        }

        [SetUp]
        public void StartTest() {
            SetUser("sven");
            Fixtures.InstallFixtures(NakedObjectsContext.ObjectPersistor, null);
        }

        [TearDown]
        public void EndTest() {
            MemoryObjectStore.DiscardObjects();
        }

        #endregion

        protected override IServicesInstaller MenuServices {
            get { return new ServicesInstaller(DemoServicesSet.ServicesSet()); }
        }

        protected override IServicesInstaller ContributedActions {
            get { return new ServicesInstaller(new object[] { new RecordedActionContributedActions() }); }
        }

        protected override IFixturesInstaller Fixtures {
            get { return new FixturesInstaller(DemoFixtureSet.FixtureSet()); }
        }

        private class DummyController : Controller {}

        private readonly Controller controller = new DummyController();

        [Test]
        public void AddNakedObjectToCache() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsContext.GetServices();

            INakedObject claim = NakedObjectsContext.GetNakedObject(NakedObjectsContext.ObjectPersistor.Instances<Claim>().First());
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsContext, claim);

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Contains(claim.Object));
        }

        [Test]
        public void AddToCache() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsContext.GetServices();

            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsContext, claim);

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Contains(claim));
        }

        [Test]
        public void AllCachedObjects() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsContext.GetServices();

            Claim claim1 = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            Claim claim2 = NakedObjectsContext.ObjectPersistor.Instances<Claim>().Last();

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsContext, claim1);
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsContext, claim2);

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Contains(claim1));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Contains(claim2));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Count() == 2);
        }

        [Test]
        public void CacheLimit() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsContext.GetServices();

            for (int i = 0; i < 200; i++) {
                INakedObject claim = GetTestService("Claims").GetAction("Create New Claim", typeof (string)).InvokeReturnObject(i.ToString()).NakedObject;
                mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsContext, claim);
            }

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Count() == ObjectCache.CacheSize);
        }

        [Test]
        public void CachedObjectsOfBaseType() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsContext.GetServices();

            GeneralExpense item1 = NakedObjectsContext.ObjectPersistor.Instances<GeneralExpense>().First();
            GeneralExpense item2 = NakedObjectsContext.ObjectPersistor.Instances<GeneralExpense>().Last();

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsContext, item1);
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsContext, item2);

            INakedObjectSpecification spec = NakedObjectsContext.Reflector.LoadSpecification(typeof (AbstractExpenseItem));

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(NakedObjectsContext, spec).Contains(item1));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(NakedObjectsContext, spec).Contains(item2));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(NakedObjectsContext, spec).Count() == 2);
        }

        [Test]
        public void CachedObjectsOfDifferentType() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsContext.GetServices();

            GeneralExpense item1 = NakedObjectsContext.ObjectPersistor.Instances<GeneralExpense>().First();
            GeneralExpense item2 = NakedObjectsContext.ObjectPersistor.Instances<GeneralExpense>().Last();

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsContext, item1);
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsContext, item2);

            INakedObjectSpecification spec = NakedObjectsContext.Reflector.LoadSpecification(typeof (Claim));

            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(NakedObjectsContext, spec).Contains(item1));
            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(NakedObjectsContext, spec).Contains(item2));
            Assert.IsTrue(!mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(NakedObjectsContext, spec).Any());
        }

        [Test]
        public void CachedObjectsOfType() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsContext.GetServices();

            Claim claim1 = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            Claim claim2 = NakedObjectsContext.ObjectPersistor.Instances<Claim>().Last();

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsContext, claim1);
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsContext, claim2);

            INakedObjectSpecification spec = NakedObjectsContext.Reflector.LoadSpecification(typeof (Claim));

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(NakedObjectsContext, spec).Contains(claim1));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(NakedObjectsContext, spec).Contains(claim2));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(NakedObjectsContext, spec).Count() == 2);
        }

        [Test]
        public void DoNotAddDuplicates() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsContext.GetServices();

            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsContext, claim);
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsContext, claim);

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Contains(claim));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Count() == 1);
        }

        [Test]
        public void AddTransient() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsContext.GetServices();

            var claim = NakedObjectsContext.ObjectPersistor.CreateInstance(NakedObjectsContext.Reflector.LoadSpecification(typeof (Claim))).GetDomainObject<Claim>();

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsContext, claim);

            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Contains(claim));
            Assert.IsTrue(!mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Any());
        }

        [Test, Ignore] // temp ignore pending proper tests 
        public void AddCollection() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsContext.GetServices();

            var claim = NakedObjectsContext.ObjectPersistor.CreateInstance(NakedObjectsContext.Reflector.LoadSpecification(typeof(Claim))).GetDomainObject<Claim>();
            var claims = new List<Claim> {claim};
            var claimAdapter = NakedObjectsContext.GetNakedObject(claim);
            var claimsAdapter = NakedObjectsContext.GetNakedObject(claims);

            var mockOid = new CollectionMemento(NakedObjectsContext.ObjectPersistor, NakedObjectsContext.Reflector, NakedObjectsContext.Session, claimAdapter, claimAdapter.GetActionLeafNode("ApproveItems"), new INakedObject[] { });

            claimsAdapter.SetATransientOid(mockOid);

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsContext, claimsAdapter);

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Contains(claims));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Count() == 1);
        }


        [Test]
        public void PurgesOldest() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsContext.GetServices();

            var testObjects = new List<INakedObject>();

            for (int i = 0; i <= ObjectCache.CacheSize; i++) {
                INakedObject claim = GetTestService("Claims").GetAction("Create New Claim", typeof (string)).InvokeReturnObject(i.ToString()).NakedObject;
                testObjects.Add(claim);
            }

            testObjects.ForEach(o => mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsContext, o));

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Count() == ObjectCache.CacheSize);
            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Contains(testObjects[0].Object));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Contains(testObjects[ObjectCache.CacheSize].Object));
        }

        [Test]
        public void RemoveFromCache() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsContext.GetServices();

            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsContext, claim);
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Contains(claim));

            mocks.HtmlHelper.ViewContext.HttpContext.Session.RemoveFromCache(NakedObjectsContext, claim);
            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Contains(claim));
        }

        [Test]
        public void RemoveOthersFromCache() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsContext.GetServices();

            Claim claim1 = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            Claim claim2 = NakedObjectsContext.ObjectPersistor.Instances<Claim>().Last();

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsContext, claim1);
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsContext, claim2);

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Contains(claim1));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Contains(claim2));

            mocks.HtmlHelper.ViewContext.HttpContext.Session.RemoveOthersFromCache(NakedObjectsContext, claim1);

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Contains(claim1));
            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Contains(claim2));
        }


        [Test]
        public void RemoveFromCacheNotThere() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsContext.GetServices();

            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();

            mocks.HtmlHelper.ViewContext.HttpContext.Session.RemoveFromCache(NakedObjectsContext, claim);
            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Contains(claim));
            Assert.IsTrue(!mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Any());
        }

        [Test]
        public void ClearDisposedFromCache() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsContext.GetServices();

            INakedObject claim = NakedObjectsContext.GetNakedObject(NakedObjectsContext.ObjectPersistor.Instances<Claim>().Last());
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsContext, claim);
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Contains(claim.Object));

            NakedObjectsContext.ObjectPersistor.StartTransaction();
            NakedObjectsContext.ObjectPersistor.DestroyObject(claim);
            NakedObjectsContext.ObjectPersistor.EndTransaction();

            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Contains(claim.Object));
        }

        [Test]
        public void ClearNotExistentFromCache() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsContext.GetServices();

            INakedObject claim = NakedObjectsContext.ObjectPersistor.CreateInstance( NakedObjectsContext.Reflector.LoadSpecification(typeof(Claim)));

            // mangle oid 
            new SimpleOidGenerator(NakedObjectsContext.Reflector, 100).ConvertTransientToPersistentOid(claim.Oid);

            mocks.HtmlHelper.ViewContext.HttpContext.Session.TestAddToCache(NakedObjectsContext, claim);
                    
            // object not found 

            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Contains(claim.Object));
        }


        [Test]
        public void RemoveNakedObjectFromCache() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsContext.GetServices();

            INakedObject claim = NakedObjectsContext.GetNakedObject(NakedObjectsContext.ObjectPersistor.Instances<Claim>().First());
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsContext, claim);
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Contains(claim.Object));

            mocks.HtmlHelper.ViewContext.HttpContext.Session.RemoveFromCache(NakedObjectsContext, claim);
            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Contains(claim.Object));
        }

        [Test]
        public void ClearCache() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsContext.GetServices();

            INakedObject claim = NakedObjectsContext.GetNakedObject(NakedObjectsContext.ObjectPersistor.Instances<Claim>().First());
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsContext, claim);
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Contains(claim.Object));

            mocks.HtmlHelper.ViewContext.HttpContext.Session.ClearCachedObjects();
            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Contains(claim.Object));
        }


        [Test]
        public void RemoveNakedObjectFromCacheNotThere() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsContext.GetServices();

            INakedObject claim = NakedObjectsContext.GetNakedObject(NakedObjectsContext.ObjectPersistor.Instances<Claim>().First());

            mocks.HtmlHelper.ViewContext.HttpContext.Session.RemoveFromCache(NakedObjectsContext, claim);
            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Contains(claim.Object));
            Assert.IsTrue(!mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Any());
        }


        [Test]
        public void SeperateCaches() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsContext.GetServices();

            INakedObject claim1 = NakedObjectsContext.GetNakedObject(NakedObjectsContext.ObjectPersistor.Instances<Claim>().First());
            INakedObject claim2 = NakedObjectsContext.GetNakedObject(NakedObjectsContext.ObjectPersistor.Instances<Claim>().Last());
            Assert.AreNotSame(claim1, claim2);

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsContext, claim1);
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsContext, claim2, ObjectCache.ObjectFlag.BreadCrumb);

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Contains(claim1.Object));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext, ObjectCache.ObjectFlag.BreadCrumb).Contains(claim2.Object));

            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Contains(claim2.Object));
            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext, ObjectCache.ObjectFlag.BreadCrumb).Contains(claim1.Object));

            mocks.HtmlHelper.ViewContext.HttpContext.Session.ClearCachedObjects();

            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext).Contains(claim1.Object));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext, ObjectCache.ObjectFlag.BreadCrumb).Contains(claim2.Object));

            mocks.HtmlHelper.ViewContext.HttpContext.Session.ClearCachedObjects(ObjectCache.ObjectFlag.BreadCrumb);

            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsContext, ObjectCache.ObjectFlag.BreadCrumb).Contains(claim2.Object));
        }

    }
}