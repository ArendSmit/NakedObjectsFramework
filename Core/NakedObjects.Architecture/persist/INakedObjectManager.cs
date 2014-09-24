// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Services;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Persist {
    /// <summary>
    ///     Broadly speaking, keeps track of the oid/adapter/domain object tuple
    /// </summary>
    public interface INakedObjectManager {
        INakedObject CreateInstance(INakedObjectSpecification specification);

        INakedObject CreateViewModel(INakedObjectSpecification specification);

        INakedObject RecreateInstance(IOid oid, INakedObjectSpecification specification);  

        void RemoveAdapter(INakedObject objectToDispose);

        INakedObject GetAdapterFor(object obj);

        INakedObject GetAdapterFor(IOid oid);

        INakedObject CreateAdapter(object domainObject, IOid oid, IVersion version);

        void ReplacePoco(INakedObject nakedObject, object newDomainObject);
     
        INakedObject GetViewModel(IOid oid);

        INakedObject CreateAggregatedAdapter(INakedObject parent, string fieldId, object obj);

        INakedObject NewAdapterForKnownObject(object domainObject, IOid transientOid, ILifecycleManager persistor);

    }


    // Copyright (c) Naked Objects Group Ltd.
}