// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;

namespace NakedObjects.Core.Spec {
    public sealed class ServiceSpec : TypeSpec, IServiceSpec {
        public ServiceSpec(SpecFactory memberFactory, IMetamodelManager metamodelManager, INakedObjectManager nakedObjectManager, IServiceSpecImmutable innerSpec) :
            base(memberFactory, metamodelManager, nakedObjectManager, innerSpec) {}

        #region IServiceSpec Members

        public override IActionSpec[] GetActions() {
            return ObjectActions;
        }

        #endregion

        protected override PersistableType GetPersistable() {
            return PersistableType.ProgramPersistable;
        }
    }
}

// Copyright (c) Naked Objects Group Ltd.