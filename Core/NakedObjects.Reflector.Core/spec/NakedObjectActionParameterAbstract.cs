// Copyright � Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actcoll.Typeof;
using NakedObjects.Architecture.Facets.Actions.Choices;
using NakedObjects.Architecture.Facets.Actions.Defaults;
using NakedObjects.Architecture.Facets.AutoComplete;
using NakedObjects.Architecture.Facets.Naming.DescribedAs;
using NakedObjects.Architecture.Facets.Naming.Named;
using NakedObjects.Architecture.Facets.Properties.Enums;
using NakedObjects.Architecture.Facets.Propparam.Modify;
using NakedObjects.Architecture.Facets.Propparam.Validate.Mandatory;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Context;
using NakedObjects.Core.Persist;
using NakedObjects.Reflector.Peer;

namespace NakedObjects.Reflector.Spec {
    public abstract class NakedObjectActionParameterAbstract : INakedObjectActionParameter {
        private readonly int number;
        private readonly INakedObjectAction parentAction;
        private readonly INakedObjectActionParamPeer peer;

        protected internal NakedObjectActionParameterAbstract(int number, INakedObjectAction nakedObjectAction, INakedObjectActionParamPeer peer) {
            this.number = number;
            parentAction = nakedObjectAction;
            this.peer = peer;
        }

        public bool IsAutoCompleteEnabled {
            get { return ContainsFacet<IAutoCompleteFacet>(); }
        }

        public bool IsChoicesEnabled {
            get { return !IsMultipleChoicesEnabled && (Specification.IsBoundedSet() || ContainsFacet<IActionChoicesFacet>() || ContainsFacet<IEnumFacet>()); }
        }

        public bool IsMultipleChoicesEnabled {
            get { return Specification.IsCollectionOfBoundedSet() || Specification.IsCollectionOfEnum() || (ContainsFacet<IActionChoicesFacet>() && GetFacet<IActionChoicesFacet>().IsMultiple); }
        }

        #region INakedObjectActionParameter Members

        /// <summary>
        ///     Subclasses should override either <see cref="IsObject" /> or <see cref="IsCollection" />
        /// </summary>
        public virtual bool IsObject {
            get { return false; }
        }

        /// <summary>
        ///     Subclasses should override either <see cref="IsObject" /> or <see cref="IsCollection" />
        /// </summary>
        public virtual bool IsCollection {
            get { return false; }
        }

        public virtual int Number {
            get { return number; }
        }

        public virtual INakedObjectAction Action {
            get { return parentAction; }
        }

        public virtual INakedObjectSpecification Specification {
            get { return peer.Specification; }
        }

        public virtual string Name {
            get {
                var facet = GetFacet<INamedFacet>();
                string name = facet == null ? null : facet.Value;
                return name ?? peer.Specification.SingularName;
            }
        }

        public virtual string Description {
            get { return GetFacet<IDescribedAsFacet>().Value ?? ""; }
        }

        public virtual bool IsMandatory {
            get {
                var mandatoryFacet = GetFacet<IMandatoryFacet>();
                return mandatoryFacet.IsMandatory;
            }
        }

        public virtual Type[] FacetTypes {
            get { return peer != null ? peer.FacetTypes : new Type[] {}; }
        }

        public virtual IIdentifier Identifier {
            get { return parentAction.Identifier; }
        }

        public virtual bool ContainsFacet(Type facetType) {
            return peer != null && peer.ContainsFacet(facetType);
        }

        public virtual bool ContainsFacet<T>() where T : IFacet {
            return peer != null && peer.ContainsFacet<T>();
        }

        public virtual IFacet GetFacet(Type type) {
            return peer != null ? peer.GetFacet(type) : null;
        }

        public virtual T GetFacet<T>() where T : IFacet {
            return peer != null ? peer.GetFacet<T>() : default(T);
        }

        public virtual IFacet[] GetFacets(IFacetFilter filter) {
            return peer != null ? peer.GetFacets(filter) : new IFacet[] {};
        }

        public virtual void AddFacet(IFacet facet) {
            if (peer != null) {
                peer.AddFacet(facet);
            }
        }

        public virtual void AddFacet(IMultiTypedFacet facet) {
            if (peer != null) {
                peer.AddFacet(facet);
            }
        }

        public virtual void RemoveFacet(IFacet facet) {
            if (peer != null) {
                peer.RemoveFacet(facet);
            }
        }

        public virtual void RemoveFacet(Type facetType) {
            if (peer != null) {
                peer.RemoveFacet(facetType);
            }
        }

        public IConsent IsValid(INakedObject nakedObject, INakedObject proposedValue) {
            if (proposedValue != null && !proposedValue.Specification.IsOfType(Specification)) {
                return GetConsent("Not a suitable type; must be a " + Specification.SingularName);
            }

            var buf = new InteractionBuffer();
            InteractionContext ic = InteractionContext.ModifyingPropParam(NakedObjectsContext.Session, false, parentAction.RealTarget(nakedObject), Identifier, proposedValue);
            InteractionUtils.IsValid(this, ic, buf);
            return InteractionUtils.IsValid(buf);
        }

        public virtual IConsent IsUsable(ISession session, INakedObject target) {
            return Allow.Default;
        }

        public bool IsNullable {
            get { return ContainsFacet(typeof (INullableFacet)); }
        }

        public Tuple<string, INakedObjectSpecification>[] GetChoicesParameters() {
            var choicesFacet = GetFacet<IActionChoicesFacet>();
            return choicesFacet != null ? choicesFacet.ParameterNamesAndTypes : new Tuple<string, INakedObjectSpecification>[]{};
        }

        public INakedObject[] GetChoices(INakedObject nakedObject, IDictionary<string, INakedObject> parameterNameValues) {
            var choicesFacet = GetFacet<IActionChoicesFacet>();
            var enumFacet = GetFacet<IEnumFacet>();

            if (choicesFacet != null) {
                object[] options = choicesFacet.GetChoices(parentAction.RealTarget(nakedObject), parameterNameValues);
                if (enumFacet == null) {
                    return NakedObjectsContext.ObjectPersistor.GetCollectionOfAdaptedObjects(options).ToArray();
                }

                return NakedObjectsContext.ObjectPersistor.GetCollectionOfAdaptedObjects(enumFacet.GetChoices(parentAction.RealTarget(nakedObject), options)).ToArray();
            }


            if (enumFacet != null) {
                return NakedObjectsContext.ObjectPersistor.GetCollectionOfAdaptedObjects(enumFacet.GetChoices(parentAction.RealTarget(nakedObject))).ToArray();
            }

            if (Specification.IsBoundedSet()) {
                return NakedObjectsContext.ObjectPersistor.GetCollectionOfAdaptedObjects(NakedObjectsContext.ObjectPersistor.Instances(Specification)).ToArray();
            }

            if (Specification.IsCollectionOfBoundedSet() || Specification.IsCollectionOfEnum()) {
                INakedObjectSpecification instanceSpec = Specification.GetFacet<ITypeOfFacet>().ValueSpec;

                var instanceEnumFacet = instanceSpec.GetFacet<IEnumFacet>();

                if (instanceEnumFacet != null) {
                    return NakedObjectsContext.ObjectPersistor.GetCollectionOfAdaptedObjects(instanceEnumFacet.GetChoices(parentAction.RealTarget(nakedObject))).ToArray();
                }

                return NakedObjectsContext.ObjectPersistor.GetCollectionOfAdaptedObjects(NakedObjectsContext.ObjectPersistor.Instances(instanceSpec)).ToArray();
            }

            return null;
        }


        public INakedObject[] GetCompletions(INakedObject nakedObject, string autoCompleteParm) {
            var autoCompleteFacet = GetFacet<IAutoCompleteFacet>();
            return autoCompleteFacet == null ? null : NakedObjectsContext.ObjectPersistor.GetCollectionOfAdaptedObjects(autoCompleteFacet.GetCompletions(parentAction.RealTarget(nakedObject), autoCompleteParm)).ToArray();
        }

        public INakedObject GetDefault(INakedObject nakedObject) {
            return GetDefaultValueAndType(nakedObject).Item1;
        }

        public TypeOfDefaultValue GetDefaultType(INakedObject nakedObject) {
            return GetDefaultValueAndType(nakedObject).Item2;
        }


        public string Id {
            get { return Identifier.MemberParameterNames[Number]; }
        }

        private Tuple<INakedObject, TypeOfDefaultValue> GetDefaultValueAndType(INakedObject nakedObject) {
            if (parentAction.IsContributedMethod && nakedObject != null) {
                IEnumerable<INakedObjectActionParameter> matchingParms = parentAction.Parameters.Where(p => nakedObject.Specification.IsOfType(p.Specification));

                if (matchingParms.Any() && matchingParms.First() == this) {
                    return new Tuple<INakedObject, TypeOfDefaultValue>(nakedObject, TypeOfDefaultValue.Explicit);
                }
            }
            var defaultsFacet = GetFacet<IActionDefaultsFacet>();
            if (defaultsFacet != null) {
                Tuple<object, TypeOfDefaultValue> defaultvalue = defaultsFacet.GetDefault(parentAction.RealTarget(nakedObject));
                return new Tuple<INakedObject, TypeOfDefaultValue>(NakedObjectsContext.ObjectPersistor.CreateAdapter(defaultvalue.Item1, null, null), defaultvalue.Item2);
            }
            return new Tuple<INakedObject, TypeOfDefaultValue>(null, TypeOfDefaultValue.Implicit);
        }

        #endregion

        protected internal virtual IConsent GetConsent(string message) {
            return message == null ? (IConsent) Allow.Default : new Veto(message);
        }
    }
}