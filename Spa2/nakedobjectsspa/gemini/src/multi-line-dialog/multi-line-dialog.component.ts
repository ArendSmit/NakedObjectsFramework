import { Component, ViewChildren, QueryList, AfterViewInit, OnDestroy } from '@angular/core';
import { PaneComponent } from '../pane/pane';
import { ParametersComponent } from '../parameters/parameters.component';
import { ViewModelFactoryService } from '@nakedobjects/view-models';
import { ActivatedRoute } from '@angular/router';
import { UrlManagerService } from '@nakedobjects/services';
import { ContextService } from '@nakedobjects/services';
import { ErrorService } from '@nakedobjects/services';
import { PaneRouteData } from '@nakedobjects/services';
import { ActionViewModel } from '@nakedobjects/view-models';
import { CollectionViewModel } from '@nakedobjects/view-models';
import { MultiLineDialogViewModel } from '@nakedobjects/view-models';
import { DialogViewModel } from '@nakedobjects/view-models';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ParameterViewModel } from '@nakedobjects/view-models';
import { ConfigService } from '@nakedobjects/services';
import * as Msg from '../user-messages';
import * as Models from '@nakedobjects/restful-objects';
import { Dictionary } from 'lodash';
import find from 'lodash-es/find';
import forEach from 'lodash-es/forEach';
import map from 'lodash-es/map';
import some from 'lodash-es/some';
import each from 'lodash-es/each';
import { SubscriptionLike as ISubscription } from 'rxjs';
import { safeUnsubscribe, createForm } from '../helpers-components';
import { ErrorWrapper } from '@nakedobjects/services';

@Component({
    selector: 'nof-multi-line-dialog',
    templateUrl: 'multi-line-dialog.component.html',
    styleUrls: ['multi-line-dialog.component.css']
})
export class MultiLineDialogComponent extends PaneComponent implements AfterViewInit, OnDestroy {

    constructor(
        activatedRoute: ActivatedRoute,
        urlManager: UrlManagerService,
        private readonly viewModelFactory: ViewModelFactoryService,
        context: ContextService,
        private readonly error: ErrorService,
        private readonly formBuilder: FormBuilder,
        private readonly configService: ConfigService
    ) {
        super(activatedRoute, urlManager, context);
    }

    @ViewChildren(ParametersComponent)
    parmComponents: QueryList<ParametersComponent>;

    private sub: ISubscription;
    dialog: MultiLineDialogViewModel;

    rowData: { form: FormGroup, dialog: DialogViewModel, parms: Dictionary<ParameterViewModel>, sub: ISubscription }[];

    form = (i: number) => {
        const rowData = this.rowData[i];
        return rowData.form;
    }

    get objectFriendlyName() {
        return this.dialog.objectFriendlyName;
    }

    get objectTitle() {
        return this.dialog.objectTitle;
    }

    get dialogTitle() {
        return this.dialog.title;
    }

    get header() {
        return this.dialog.header();
    }

    get rows() {
        return this.dialog.dialogs;
    }

    parameters = (row: DialogViewModel) => row.parameters;

    rowSubmitted = (row: DialogViewModel) => row.submitted;

    rowTooltip = (row: DialogViewModel) => row.tooltip();

    rowMessage = (row: DialogViewModel) => {
        return row.submitted ? Msg.submittedMessage : row.getMessage();
    }

    rowDisabled = (row: DialogViewModel) => {
        return !row.clientValid() || row.submitted;
    }

    get count() {
        return Msg.submittedCount(this.dialog.submittedCount());
    }

    invokeAndAdd(index: number) {
        const parms = this.rowData[index].parms;

        forEach(parms,
            p => {
                const newValue = this.rowData[index].form.value[p.id];
                p.setValueFromControl(newValue);
            });

        const addedIndex = this.dialog.invokeAndAdd(index);

        if (addedIndex) {
            this.rowData.push(this.createForm(this.dialog.dialogs[addedIndex]));
        }
    }

    close = () => {
        this.urlManager.popUrlState();
    }

    private createForm(dialog: DialogViewModel) {
        return createForm(dialog, this.formBuilder);
    }

    setMultiLineDialog(holder: Models.MenuRepresentation | Models.DomainObjectRepresentation | CollectionViewModel,
        newDialogId: string,
        routeData: PaneRouteData,
        actionViewModel?: ActionViewModel) {

        const action = holder.actionMember(newDialogId)!;
        this.context.getInvokableAction(action).
            then(details => {

                if (actionViewModel) {
                    actionViewModel.makeInvokable(details);
                }

                this.dialog = this.viewModelFactory.multiLineDialogViewModel(routeData, details, holder);
                this.rowData = map(this.dialog.dialogs, d => this.createForm(d));
            }).
            catch((reject: ErrorWrapper) => this.error.handleError(reject));
    }

    protected setup(routeData: PaneRouteData) {
        if (routeData.menuId) {
            this.context.getMenu(routeData.menuId)
                .then((menu: Models.MenuRepresentation) => {
                    this.setMultiLineDialog(menu, routeData.dialogId, routeData);
                })
                .catch((reject: ErrorWrapper) => {
                    this.error.handleError(reject);
                });
        } else if (routeData.objectId) {
            const oid = Models.ObjectIdWrapper.fromObjectId(routeData.objectId, this.configService.config.keySeparator);
            this.context.getObject(routeData.paneId, oid, routeData.interactionMode).
                then((object: Models.DomainObjectRepresentation) => {

                    const ovm = this.viewModelFactory.domainObjectViewModel(object, routeData, false);
                    const newDialogId = routeData.dialogId;

                    const lcaCollection = find(ovm.collections, c => c.hasMatchingLocallyContributedAction(newDialogId));

                    if (lcaCollection) {
                        const actionViewModel = find(lcaCollection.actions, a => a.actionRep.actionId() === newDialogId);
                        this.setMultiLineDialog(lcaCollection, newDialogId, routeData, actionViewModel);
                    } else {
                        this.setMultiLineDialog(object, newDialogId, routeData);
                    }

                }).
                catch((reject: ErrorWrapper) => {
                    this.error.handleError(reject);
                });
        }
    }

    focus(parms: QueryList<ParametersComponent>) {
        if (parms && parms.length > 0) {
            some(parms.toArray(), p => p.focus());
        }
    }

    ngAfterViewInit(): void {
        this.sub = this.parmComponents.changes.subscribe(ql => this.focus(ql));
    }

    ngOnDestroy(): void {
        safeUnsubscribe(this.sub);
        each(this.rowData, rd => safeUnsubscribe(rd.sub));
        super.ngOnDestroy();
    }
}
