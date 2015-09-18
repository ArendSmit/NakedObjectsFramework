﻿//Copyright 2014 Stef Cascarini, Dan Haywood, Richard Pawson
//Licensed under the Apache License, Version 2.0(the
//"License"); you may not use this file except in compliance
//with the License.You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
//Unless required by applicable law or agreed to in writing,
//software distributed under the License is distributed on an
//"AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
//KIND, either express or implied.See the License for the
//specific language governing permissions and limitations
//under the License.

/// <reference path="../../Scripts/typings/karma-jasmine/karma-jasmine.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular-mocks.d.ts" />
/// <reference path="../../Scripts/nakedobjects.models.ts" />
/// <reference path="../../Scripts/nakedobjects.angular.services.color.ts" />
/// <reference path="../../Scripts/nakedobjects.gemini.viewmodels.ts" />
/// <reference path="helpers.ts" />

describe("viewModelFactory Service", () => {

    beforeEach(angular.mock.module("app"));

    describe("create errorViewModel", () => {

        let resultVm: NakedObjects.Angular.Gemini.ErrorViewModel;
        const rawError = { message: "a message", stackTrace: ["line1", "line2"] };
        const emptyError = {};

        describe("from populated rep", () => {

            beforeEach(inject((viewModelFactory: NakedObjects.Angular.Gemini.IViewModelFactory) => {
                resultVm = viewModelFactory.errorViewModel(new NakedObjects.ErrorRepresentation(rawError));
            }));

            it("creates a error view model", () => {
                expect(resultVm.message).toBe("a message");
                expect(resultVm.stackTrace.length).toBe(2);
                expect(resultVm.stackTrace.pop()).toBe("line2");
                expect(resultVm.stackTrace.pop()).toBe("line1");
            });
        });

        describe("from empty rep", () => {

            beforeEach(inject((viewModelFactory: NakedObjects.Angular.Gemini.IViewModelFactory) => {
                resultVm = viewModelFactory.errorViewModel(new NakedObjects.ErrorRepresentation(emptyError));
            }));

            it("creates a error view model", () => {
                expect(resultVm.message).toBe("An Error occurred");
                expect(resultVm.stackTrace.length).toBe(1);
                expect(resultVm.stackTrace.pop()).toBe("Empty");
            });
        });
    });

    describe("create linkViewModel", () => {

        let resultVm: NakedObjects.Angular.Gemini.LinkViewModel;
        const rawLink = {
            title: "a title",
            href: "http://objects/AdventureWorksModel.Product/1",
            rel: 'urn: org.restfulobjects:rels/details;action="anAction"'
        };

        describe("from populated rep", () => {

            let setMenu: jasmine.Spy;

            beforeEach(inject((viewModelFactory: NakedObjects.Angular.Gemini.IViewModelFactory, urlManager) => {
                resultVm = viewModelFactory.linkViewModel(new NakedObjects.Link(rawLink));
                setMenu = spyOn(urlManager, "setMenu");
            }));

            it("creates a link view model", () => {
                expect(resultVm.title).toBe("a title");
                expect(resultVm.color).toBe("bg-color-orangeDark");
                resultVm.doClick();
                expect(setMenu).toHaveBeenCalledWith("anAction");
            });
        });
    });

    describe("create itemViewModel", () => {

        let resultVm: NakedObjects.Angular.Gemini.ItemViewModel;
        let setItem: jasmine.Spy;
        const rawLink = { title: "a title", href: "http://objects/AdventureWorksModel.Product/1" };


        describe("from populated rep", () => {
            const link = new NakedObjects.Link(rawLink);
            beforeEach(inject((viewModelFactory: NakedObjects.Angular.Gemini.IViewModelFactory, urlManager) => {
                resultVm = viewModelFactory.itemViewModel(link);
                setItem = spyOn(urlManager, "setItem");
            }));

            it("creates an item view model", () => {
                expect(resultVm.title).toBe("a title");
                expect(resultVm.color).toBe("bg-color-orangeDark");
                resultVm.doClick();
                expect(setItem).toHaveBeenCalledWith(link);
                expect(resultVm.target).toBeUndefined();
            });
        });
    });

    describe("create actionViewModel", () => {

        let resultVm: NakedObjects.Angular.Gemini.ActionViewModel;
        const rawdetailsLink = {
            rel: "urn:org.restfulobjects:rels/details",
            href: "http://objects/AdventureWorksModel.Product/1/actions/anaction"
        }
        const rawAction = {
            extensions: {
                friendlyName: "a title",
                "x-ro-nof-menuPath": "a path"
            },
            links: [rawdetailsLink]
        };
        const rawActionParms = _.set(_.cloneDeep(rawAction), "extensions.hasParams", true);

        describe("from populated rep with no parms", () => {

            let invokeAction: jasmine.Spy;
            const am = new NakedObjects.ActionMember(rawAction, {}, "anid");

            beforeEach(inject((viewModelFactory: NakedObjects.Angular.Gemini.IViewModelFactory, context) => {
                resultVm = viewModelFactory.actionViewModel(am);
                invokeAction = spyOn(context, "invokeAction");
            }));

            it("creates an action view model", () => {
                expect(resultVm.title).toBe("a title");
                expect(resultVm.menuPath).toBe("a path");
                resultVm.doInvoke();
                expect(invokeAction).toHaveBeenCalledWith(am);
            });
        });

        describe("from populated rep with parms", () => {

            let setDialog: jasmine.Spy;

            beforeEach(inject((viewModelFactory: NakedObjects.Angular.Gemini.IViewModelFactory, urlManager) => {
                resultVm = viewModelFactory.actionViewModel(new NakedObjects.ActionMember(rawActionParms, {}, "anid"));
                setDialog = spyOn(urlManager, "setDialog");
            }));

            it("creates an action view model", () => {
                expect(resultVm.title).toBe("a title");
                expect(resultVm.menuPath).toBe("a path");
                resultVm.doInvoke();
                expect(setDialog).toHaveBeenCalledWith("anid");
            });
        });
    });

    describe("create dialogViewModel", () => {

        let resultVm: NakedObjects.Angular.Gemini.DialogViewModel;

        const rawInvokeLink = {
            rel: "urn:org.restfulobjects:rels/invoke",
            href: "http://objects/AdventureWorksModel.Product/1/actions/anaction"
        };
        const rawUpLink = {
            rel: "urn:org.restfulobjects:rels/up",
            href: "http://objects/AdventureWorksModel.Product/1"
        };
        const rawAction = {
            extensions: { friendlyName: "a title" },
            links: [rawInvokeLink, rawUpLink]
        };

        describe("from simple rep", () => {

            let invokeAction: jasmine.Spy;
            let closeDialog: jasmine.Spy;
            const am = new NakedObjects.ActionMember(rawAction, {}, "anid");

            beforeEach(inject((viewModelFactory: NakedObjects.Angular.Gemini.IViewModelFactory, context, urlManager) => {
                invokeAction = spyOn(context, "invokeAction");
                closeDialog = spyOn(urlManager, "closeDialog");
                resultVm = viewModelFactory.dialogViewModel(am);
            }));

            it("creates a dialog view model", () => {
                expect(resultVm.title).toBe("a title");
                expect(resultVm.isQuery).toBe(false);
                expect(resultVm.message).toBe("");
                expect(resultVm.parameters.length).toBe(0);

                resultVm.doInvoke();
                expect(invokeAction).toHaveBeenCalledWith(am, resultVm);

                resultVm.doClose();
                expect(closeDialog).toHaveBeenCalled();
            });
        });

    });

    describe("create collectionViewModel", () => {

        let resultVm: NakedObjects.Angular.Gemini.CollectionViewModel;

        const rawLink1 = {     
            type : "application/json;profile=\"urn:org.resfulobjects:repr-types/object\"",  
            href: "http://objects/AdventureWorksModel.Product/1"
        };

        const rawLink2 = {     
            type: "application/json;profile=\"urn:org.resfulobjects:repr-types/object\"",  
            href: "http://objects/AdventureWorksModel.Product/2"
        };


        const rawDetailsLink = {
            rel: "urn:org.restfulobjects:rels/details",
            href: "http://objects/AdventureWorksModel.Product/1/collections/acollection"
        };


        const rawSelfLink = {
            rel: "urn:org.restfulobjects:rels/self",
            href: "http://objects/AdventureWorksModel.Product/1/collections/acollection"
        };

        const rawEmptyCollection = {
            size: 0,
            extensions: {
                friendlyName: "a title",
                pluralName: "somethings",
                elementType: "AdventureWorksModel.Product"
            },
            links: [rawDetailsLink]
        };

        const rawEmptyList = {
            value: [],
            links: [rawSelfLink]
        };

        const rawCollection = {
            size: 2,
            extensions: {
                friendlyName: "a title",
                pluralName: "somethings",
                elementType: "AdventureWorksModel.Product"
            },
            links: [rawDetailsLink],
            value : [rawLink1, rawLink2]
        };

        const rawList = {
            value: [rawLink1, rawLink2],
            links: [rawSelfLink]
        };


        describe("from empty collection member rep", () => {

            let setCollectionState: jasmine.Spy;
            const cm = new NakedObjects.CollectionMember(rawEmptyCollection, {}, "");

            beforeEach(inject((viewModelFactory, urlManager) => {
                resultVm = viewModelFactory.collectionViewModel(cm, NakedObjects.Angular.Gemini.CollectionViewState.List);

                setCollectionState = spyOn(urlManager, "setCollectionState");
            }));

            it("creates a dialog view model", () => {
                expect(resultVm.title).toBe("a title");
                expect(resultVm.size).toBe(0);
                expect(resultVm.color).toBe("bg-color-orangeDark");
                expect(resultVm.items.length).toBe(0);
                expect(resultVm.pluralName).toBe("somethings");

                resultVm.doSummary();
                expect(setCollectionState).toHaveBeenCalledWith(cm, NakedObjects.Angular.Gemini.CollectionViewState.Summary);
                resultVm.doList();
                expect(setCollectionState).toHaveBeenCalledWith(cm, NakedObjects.Angular.Gemini.CollectionViewState.List);
                resultVm.doTable();
                expect(setCollectionState).toHaveBeenCalledWith(cm, NakedObjects.Angular.Gemini.CollectionViewState.Table);
            });
        });

        describe("from non empty collection member rep", () => {

            let setCollectionState: jasmine.Spy;
            let itemViewModel: jasmine.Spy;
            let populate: jasmine.Spy;

            const cm = new NakedObjects.CollectionMember(rawCollection, {}, "");
            let vmf: NakedObjects.Angular.Gemini.IViewModelFactory;
          
            beforeEach(inject((viewModelFactory, urlManager, repLoader, $q) => {
                setCollectionState = spyOn(urlManager, "setCollectionState");
                itemViewModel = spyOn(viewModelFactory, "itemViewModel");
                populate = spyOn(repLoader, "populate");
                populate.and.returnValue($q.when());
                vmf = viewModelFactory;                 
            }));

            it("creates a dialog view model with items", () => {  
                resultVm = vmf.collectionViewModel(cm, NakedObjects.Angular.Gemini.CollectionViewState.List);                    
                expect(resultVm.items.length).toBe(2);
                expect(itemViewModel.calls.count()).toBe(2);
                expect(populate).not.toHaveBeenCalled();
            });

            it("it populates table items", () => {
                resultVm = vmf.collectionViewModel(cm, NakedObjects.Angular.Gemini.CollectionViewState.Table);
                expect(resultVm.items.length).toBe(2);
                expect(itemViewModel.calls.count()).toBe(2);
                expect(populate.calls.count()).toBe(2);
            });
        });

        describe("from empty list rep", () => {

            let setCollectionState: jasmine.Spy;
            const lr = new NakedObjects.ListRepresentation(rawEmptyList);

            beforeEach(inject((viewModelFactory, urlManager) => {
                setCollectionState = spyOn(urlManager, "setCollectionState");
                resultVm = viewModelFactory.collectionViewModel(lr, NakedObjects.Angular.Gemini.CollectionViewState.Summary);
            }));

            it("creates a dialog view model", () => {
                expect(resultVm.title).toBeUndefined();
                expect(resultVm.size).toBe(0);
                expect(resultVm.color).toBeUndefined();
                expect(resultVm.items.length).toBe(0);
                expect(resultVm.pluralName).toBe("Objects");

                resultVm.doSummary();
                expect(setCollectionState).toHaveBeenCalledWith(lr, NakedObjects.Angular.Gemini.CollectionViewState.Summary);
                resultVm.doList();
                expect(setCollectionState).toHaveBeenCalledWith(lr, NakedObjects.Angular.Gemini.CollectionViewState.List);
                resultVm.doTable();
                expect(setCollectionState).toHaveBeenCalledWith(lr, NakedObjects.Angular.Gemini.CollectionViewState.Table);
            });
        });

        describe("from non empty list rep", () => {

            let setCollectionState: jasmine.Spy;
            let itemViewModel: jasmine.Spy;
            let populate: jasmine.Spy;
            const lr = new NakedObjects.ListRepresentation(rawList);
            let vmf: NakedObjects.Angular.Gemini.IViewModelFactory;
         

            beforeEach(inject((viewModelFactory, urlManager, repLoader, $q) => {
                setCollectionState = spyOn(urlManager, "setCollectionState");
                itemViewModel = spyOn(viewModelFactory, "itemViewModel");
                populate = spyOn(repLoader, "populate");
                populate.and.returnValue($q.when());
                vmf = viewModelFactory;
            }));

            it("creates a dialog view model with items", () => {
                resultVm = vmf.collectionViewModel(lr, NakedObjects.Angular.Gemini.CollectionViewState.List);
                expect(resultVm.items.length).toBe(2);
                expect(itemViewModel.calls.count()).toBe(2);
                expect(populate).not.toHaveBeenCalled();
            });

            it("it populates table items", () => {
                resultVm = vmf.collectionViewModel(lr, NakedObjects.Angular.Gemini.CollectionViewState.Table);
                expect(resultVm.items.length).toBe(2);
                expect(itemViewModel.calls.count()).toBe(2);
                expect(populate.calls.count()).toBe(2);
            });
        });
    });

    describe("create parameter view model", () => {
        let resultVm: NakedObjects.Angular.Gemini.ParameterViewModel;

        const rawParameter: any = { extensions: { friendlyName: "a parm" }, links: [] };
        const rawAction = {};

        describe("from populated rep", () => {

            beforeEach(inject((viewModelFactory: NakedObjects.Angular.Gemini.IViewModelFactory) => {
                resultVm = viewModelFactory.parameterViewModel(new NakedObjects.Parameter(rawParameter, new NakedObjects.ActionRepresentation(rawAction), "anId"), "pv");
            }));

            it("creates a parameter view model", () => {

                expect(resultVm.type).toBe("ref");
                expect(resultVm.title).toBe("a parm");
                expect(resultVm.dflt).toBe("");
                expect(resultVm.message).toBe("");
                expect(resultVm.mask).toBeUndefined();
                expect(resultVm.id).toBe("anId");
                expect(resultVm.argId).toBe("anid");
                expect(resultVm.returnType).toBeUndefined();
                expect(resultVm.format).toBeUndefined();
                expect(resultVm.reference).toBe("");

                expect(resultVm.choices.length).toBe(0);
                expect(resultVm.hasChoices).toBe(false);
                expect(resultVm.hasPrompt).toBe(false);
                expect(resultVm.hasConditionalChoices).toBe(false);
                expect(resultVm.isMultipleChoices).toBe(false);
                expect(resultVm.value).toBe("pv");

            });
        });

        describe("from populated rep with scalar choices", () => {

            beforeEach(inject((viewModelFactory: NakedObjects.Angular.Gemini.IViewModelFactory) => {

                rawParameter.choices = [1, 2, 3];
                rawParameter.default = 1;


                resultVm = viewModelFactory.parameterViewModel(new NakedObjects.Parameter(rawParameter, new NakedObjects.ActionRepresentation(rawAction), "anid"), "");
            }));

            it("creates a parameter view model with choices", () => {


                expect(resultVm.choices.length).toBe(3);
                expect(resultVm.hasChoices).toBe(true);
                expect(resultVm.hasPrompt).toBe(false);
                expect(resultVm.hasConditionalChoices).toBe(false);
                expect(resultVm.isMultipleChoices).toBe(false);
                expect(resultVm.choice.value).toBe("1");
                expect(resultVm.value).toBeUndefined();

            });
        });

        describe("from populated rep with prompt autocomplete", () => {

            beforeEach(inject((viewModelFactory: NakedObjects.Angular.Gemini.IViewModelFactory) => {
                const rawPromptLink = {
                    rel: "urn:org.restfulobjects:rels/prompt",
                    href: "http://services/AdventureWorksModel.ProductRepository/prompt",
                    arguments: { "x-ro-searchTerm": { value: null } },
                    extensions: { minLength: 0 },
                    type: "application/json; profile = \"urn:org.restfulobjects:repr-types/prompt\""
                };

                rawParameter.choices = null;
                rawParameter.default = 1;
                rawParameter.links.push(rawPromptLink);

                resultVm = viewModelFactory.parameterViewModel(new NakedObjects.Parameter(rawParameter, new NakedObjects.ActionRepresentation(rawAction), "anid"), "");
            }));

            it("creates a parameter view model with prompt", () => {


                expect(resultVm.choices.length).toBe(0);
                expect(resultVm.hasChoices).toBe(false);
                expect(resultVm.hasPrompt).toBe(true);
                expect(resultVm.hasConditionalChoices).toBe(false);
                expect(resultVm.isMultipleChoices).toBe(false);
                expect(resultVm.choice.value).toBe("1");
                expect(resultVm.value).toBeUndefined();

            });
        });

        describe("from populated rep with prompt conditional choices", () => {

            beforeEach(inject((viewModelFactory: NakedObjects.Angular.Gemini.IViewModelFactory) => {
                const rawPromptLink = {
                    rel: "urn:org.restfulobjects:rels/prompt",
                    href: "http://services/AdventureWorksModel.ProductRepository/prompt",
                    arguments: { "parm": { value: null } },
                    extensions: { minLength: 0 },
                    type: "application/json; profile = \"urn:org.restfulobjects:repr-types/prompt\""
                };

                rawParameter.choices = null;
                rawParameter.default = 1;
                rawParameter.links.pop();
                rawParameter.links.push(rawPromptLink);

                resultVm = viewModelFactory.parameterViewModel(new NakedObjects.Parameter(rawParameter, new NakedObjects.ActionRepresentation(rawAction), "anid"), "");
            }));

            it("creates a parameter view model with prompt", () => {


                expect(resultVm.choices.length).toBe(0);
                expect(resultVm.hasChoices).toBe(false);
                expect(resultVm.hasPrompt).toBe(false);
                expect(resultVm.hasConditionalChoices).toBe(true);
                expect(resultVm.isMultipleChoices).toBe(false);
                expect(resultVm.choice.value).toBe("1");
                expect(resultVm.value).toBeUndefined();

            });
        });

    });
  
    describe("create object view model", () => {
        let resultVm: NakedObjects.Angular.Gemini.DomainObjectViewModel;
        const rawSelfLink = { rel: "urn:org.restfulobjects:rels/self", href: "http://objects/AdventureWorksModel.Product/1" };

        const rawObject = { domainType: "an object", links: [rawSelfLink], title: "a title", extensions: { friendlyName: "a name" } };

        describe("from populated rep", () => {

            beforeEach(inject((viewModelFactory: NakedObjects.Angular.Gemini.IViewModelFactory) => {
                resultVm = viewModelFactory.domainObjectViewModel(new NakedObjects.DomainObjectRepresentation(rawObject), {});
            }));

            it("creates a object view model", () => {
                expect(resultVm.domainType).toBe("an object");
                expect(resultVm.title).toBe("a title");
                expect(resultVm.actions.length).toBe(0);
                expect(resultVm.properties.length).toBe(0);
                expect(resultVm.collections.length).toBe(0);
                expect(resultVm.color).toBe("bg-color-red");
            });
        });

        describe("from transient populated rep", () => {

            beforeEach(inject((viewModelFactory: NakedObjects.Angular.Gemini.IViewModelFactory) => {
                const rawPersistLink = { rel: "urn:org.restfulobjects:rels/persist", href: "http://objects/AdventureWorksModel.Product" };
                rawObject.links.pop();
                rawObject.links.push(rawPersistLink);
                const doRep = new NakedObjects.DomainObjectRepresentation(rawObject);
                doRep.hateoasUrl = "http://objects/AdventureWorksModel.Product";

                resultVm = viewModelFactory.domainObjectViewModel(doRep, {});
            }));

            it("creates a object view model", () => {
                expect(resultVm.domainType).toBe("an object");
                expect(resultVm.title).toBe("Unsaved a name");
                expect(resultVm.actions.length).toBe(0);
                expect(resultVm.properties.length).toBe(0);
                expect(resultVm.collections.length).toBe(0);
                expect(resultVm.color).toBe("bg-color-red");

            });
        });
    });

}); 
