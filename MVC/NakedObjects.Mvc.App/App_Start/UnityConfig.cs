// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Security.Principal;
using System.Web;
using Microsoft.Practices.Unity;
using NakedObjects;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Menu;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Authentication;
using NakedObjects.Core.Component;
using NakedObjects.Core.Configuration;
using NakedObjects.Core.Container;
using NakedObjects.Core.Spec;
using NakedObjects.Meta;
using NakedObjects.Meta.Audit;
using NakedObjects.Meta.Menu;
using NakedObjects.Persistor.Entity;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Reflect;
using NakedObjects.Reflect.FacetFactory;
using NakedObjects.Reflect.TypeFacetFactory;
using NakedObjects.Service;
using NakedObjects.Surface;
using NakedObjects.Surface.Nof4.Implementation;
using NakedObjects.Surface.Nof4.Utility;

namespace NakedObjects.Mvc.App {
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig {
        #region Framework Configuration

        protected static void RegisterFacetFactories(IUnityContainer container) {
            int order = 0;
            container.RegisterType<IFacetFactory, FallbackFacetFactory>("FallbackFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, IteratorFilteringFacetFactory>("IteratorFilteringFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, UnsupportedParameterTypesMethodFilteringFactory>("UnsupportedParameterTypesMethodFilteringFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, RemoveSuperclassMethodsFacetFactory>("RemoveSuperclassMethodsFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, RemoveInitMethodFacetFactory>("RemoveInitMethodFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, RemoveDynamicProxyMethodsFacetFactory>("RemoveDynamicProxyMethodsFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, RemoveEventHandlerMethodsFacetFactory>("RemoveEventHandlerMethodsFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, TypeMarkerFacetFactory>("TypeMarkerFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            // must be before any other FacetFactories that install MandatoryFacet.class facets
            container.RegisterType<IFacetFactory, MandatoryDefaultFacetFactory>("MandatoryDefaultFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, PropertyValidateDefaultFacetFactory>("PropertyValidateDefaultFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ComplementaryMethodsFilteringFacetFactory>("ComplementaryMethodsFilteringFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ActionMethodsFacetFactory>("ActionMethodsFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, CollectionFieldMethodsFacetFactory>("CollectionFieldMethodsFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, PropertyMethodsFacetFactory>("PropertyMethodsFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, IconMethodFacetFactory>("IconMethodFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, CallbackMethodsFacetFactory>("CallbackMethodsFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, TitleMethodFacetFactory>("TitleMethodFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ValidateObjectFacetFactory>("ValidateObjectFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ComplexTypeAnnotationFacetFactory>("ComplexTypeAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ViewModelFacetFactory>("ViewModelFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, BoundedAnnotationFacetFactory>("BoundedAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, EnumFacetFactory>("EnumFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ActionDefaultAnnotationFacetFactory>("ActionDefaultAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, PropertyDefaultAnnotationFacetFactory>("PropertyDefaultAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, DescribedAsAnnotationFacetFactory>("DescribedAsAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, DisabledAnnotationFacetFactory>("DisabledAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, PasswordAnnotationFacetFactory>("PasswordAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ExecutedAnnotationFacetFactory>("ExecutedAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, PotencyAnnotationFacetFactory>("PotencyAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, PageSizeAnnotationFacetFactory>("PageSizeAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, HiddenAnnotationFacetFactory>("HiddenAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, HiddenDefaultMethodFacetFactory>("HiddenDefaultMethodFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, DisableDefaultMethodFacetFactory>("DisableDefaultMethodFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, AuthorizeAnnotationFacetFactory>("AuthorizeAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ValidateProgrammaticUpdatesAnnotationFacetFactory>("ValidateProgrammaticUpdatesAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ImmutableAnnotationFacetFactory>("ImmutableAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, MaxLengthAnnotationFacetFactory>("MaxLengthAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, RangeAnnotationFacetFactory>("RangeAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, MemberOrderAnnotationFacetFactory>("MemberOrderAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, MultiLineAnnotationFacetFactory>("MultiLineAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, NamedAnnotationFacetFactory>("NamedAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, NotPersistedAnnotationFacetFactory>("NotPersistedAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ProgramPersistableOnlyAnnotationFacetFactory>("ProgramPersistableOnlyAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, OptionalAnnotationFacetFactory>("OptionalAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, RequiredAnnotationFacetFactory>("RequiredAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, PluralAnnotationFacetFactory>("PluralAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, DefaultNamingFacetFactory>("DefaultNamingFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++)); // must come after Named and Plural factories
            container.RegisterType<IFacetFactory, ConcurrencyCheckAnnotationFacetFactory>("ConcurrencyCheckAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ContributedActionAnnotationFacetFactory>("ContributedActionAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ExcludeFromFindMenuAnnotationFacetFactory>("ExcludeFromFindMenuAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            // must come after any facets that install titles
            container.RegisterType<IFacetFactory, MaskAnnotationFacetFactory>("MaskAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            // must come after any facets that install titles, and after mask
            // if takes precedence over mask.
            container.RegisterType<IFacetFactory, RegExAnnotationFacetFactory>("RegExAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, TypeOfAnnotationFacetFactory>("TypeOfAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, TableViewAnnotationFacetFactory>("TableViewAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, TypicalLengthDerivedFromTypeFacetFactory>("TypicalLengthDerivedFromTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, TypicalLengthAnnotationFacetFactory>("TypicalLengthAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, EagerlyAnnotationFacetFactory>("EagerlyAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, PresentationHintAnnotationFacetFactory>("PresentationHintAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, BooleanValueTypeFacetFactory>("BooleanValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ByteValueTypeFacetFactory>("ByteValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, SbyteValueTypeFacetFactory>("SbyteValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ShortValueTypeFacetFactory>("ShortValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, IntValueTypeFacetFactory>("IntValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, LongValueTypeFacetFactory>("LongValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, UShortValueTypeFacetFactory>("UShortValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, UIntValueTypeFacetFactory>("UIntValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ULongValueTypeFacetFactory>("ULongValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, FloatValueTypeFacetFactory>("FloatValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, DoubleValueTypeFacetFactory>("DoubleValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, DecimalValueTypeFacetFactory>("DecimalValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, CharValueTypeFacetFactory>("CharValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, DateTimeValueTypeFacetFactory>("DateTimeValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, TimeValueTypeFacetFactory>("TimeValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, StringValueTypeFacetFactory>("StringValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, GuidValueTypeFacetFactory>("GuidValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, EnumValueTypeFacetFactory>("EnumValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, FileAttachmentValueTypeFacetFactory>("FileAttachmentValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ImageValueTypeFacetFactory>("ImageValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ArrayValueTypeFacetFactory<byte>>("ArrayValueTypeFacetFactory<byte>", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, CollectionFacetFactory>("CollectionFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++)); // written to not trample over TypeOf if already installed
            container.RegisterType<IFacetFactory, MenuFacetFactory>("MenuFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
        }

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container) {
            RegisterFacetFactories(container);

            // config
            container.RegisterInstance<IReflectorConfiguration>(NakedObjectsRunSettings.ReflectorConfig(), (new ContainerControlledLifetimeManager()));
            container.RegisterInstance<IEntityObjectStoreConfiguration>(NakedObjectsRunSettings.EntityObjectStoreConfig(), new ContainerControlledLifetimeManager());
           
            // in architecture
            container.RegisterType<IClassStrategy, DefaultClassStrategy>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISpecificationCache, ImmutableInMemorySpecCache>(new ContainerControlledLifetimeManager(), new InjectionConstructor());
            container.RegisterType<IReflector, Reflector>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMetamodel, Metamodel>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMetamodelBuilder, Metamodel>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMenuFactory, MenuFactory>(new ContainerControlledLifetimeManager());

            container.RegisterType<IPocoAdapterMap, PocoAdapterHashMap>(new PerRequestLifetimeManager(), new InjectionConstructor(10));
            container.RegisterType<IIdentityAdapterMap, IdentityAdapterHashMap>(new PerRequestLifetimeManager(), new InjectionConstructor(10));
            container.RegisterType<IContainerInjector, DomainObjectContainerInjector>(new PerRequestLifetimeManager());
            container.RegisterType<IOidGenerator, EntityOidGenerator>(new PerRequestLifetimeManager());
            container.RegisterType<IPersistAlgorithm, EntityPersistAlgorithm>(new PerRequestLifetimeManager());
            container.RegisterType<IObjectStore, EntityObjectStore>(new PerRequestLifetimeManager());
            container.RegisterType<IIdentityMap, IdentityMapImpl>(new PerRequestLifetimeManager());
            container.RegisterType<ITransactionManager, TransactionManager>(new PerRequestLifetimeManager());
            container.RegisterType<INakedObjectManager, NakedObjectManager>(new PerRequestLifetimeManager());
            container.RegisterType<IObjectPersistor, ObjectPersistor>(new PerRequestLifetimeManager());
            container.RegisterType<IServicesManager, ServicesManager>(new PerRequestLifetimeManager());
            container.RegisterType<ILifecycleManager, LifeCycleManager>(new PerRequestLifetimeManager());
            container.RegisterType<IMetamodelManager, MetamodelManager>(new PerResolveLifetimeManager());
            container.RegisterType<ISession, WindowsSession>(new PerRequestLifetimeManager());
            container.RegisterType<IMessageBroker, MessageBroker>(new PerRequestLifetimeManager());
            container.RegisterType<INakedObjectsFramework, NakedObjectsFramework>(new PerRequestLifetimeManager());

            // surface
            container.RegisterType<IOidStrategy, ExternalOid>(new PerRequestLifetimeManager());
            container.RegisterType<INakedObjectsSurface, NakedObjectsSurface>(new PerRequestLifetimeManager());

            //Temporary scaffolding
            container.RegisterType<NakedObjectFactory, NakedObjectFactory>(new PerRequestLifetimeManager());
            container.RegisterType<SpecFactory, SpecFactory>(new PerRequestLifetimeManager());

            //Externals
            container.RegisterType<IPrincipal>(new InjectionFactory(c => HttpContext.Current.User));

            //DI
            container.RegisterType<IFrameworkResolver, UnityFrameworkResolver>(new PerRequestLifetimeManager());

            // Facet decorators 
            if (NakedObjectsRunSettings.AuditConfig() != null) {
                container.RegisterType(typeof(IFacetDecorator), typeof(AuditManager), "AuditManager", new ContainerControlledLifetimeManager());
                container.RegisterInstance(typeof(IAuditConfiguration), NakedObjectsRunSettings.AuditConfig(), new ContainerControlledLifetimeManager());
            }
        }

        #endregion

        #region Unity Container

        private static readonly Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() => {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer() {
            return container.Value;
        }

        #endregion
    }
}