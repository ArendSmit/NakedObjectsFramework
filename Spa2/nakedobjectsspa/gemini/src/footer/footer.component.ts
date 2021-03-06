import { Component, OnInit, OnDestroy } from '@angular/core';
import { Location } from '@angular/common';
import { UrlManagerService } from '@nakedobjects/services';
import { ClickHandlerService } from '@nakedobjects/services';
import { ContextService } from '@nakedobjects/services';
import { ErrorService } from '@nakedobjects/services';
import { RepLoaderService } from '@nakedobjects/services';
import { IDraggableViewModel } from '@nakedobjects/view-models';
import * as Msg from '../user-messages';
import { ConfigService } from '@nakedobjects/services';
import { AuthService } from '@nakedobjects/services';
import { Pane } from '@nakedobjects/services';
import { SubscriptionLike as ISubscription } from 'rxjs';
import { safeUnsubscribe } from '../helpers-components';
import { HttpClient } from '@angular/common/http';
import { ErrorWrapper } from '@nakedobjects/services';
import { DragAndDropService } from '@nakedobjects/view-models';

@Component({
    selector: 'nof-footer',
    templateUrl: 'footer.component.html',
    styleUrls: ['footer.component.css']
})
export class FooterComponent implements OnInit, OnDestroy {

    constructor(
        private readonly authService: AuthService,
        private readonly urlManager: UrlManagerService,
        private readonly context: ContextService,
        private readonly clickHandler: ClickHandlerService,
        private readonly error: ErrorService,
        private readonly repLoader: RepLoaderService,
        private readonly location: Location,
        private readonly configService: ConfigService,
        private readonly http: HttpClient,
        private readonly dragAndDrop: DragAndDropService
    ) { }

    private warnSub: ISubscription;
    private messageSub: ISubscription;
    private cvmSub: ISubscription;
    private lcSub: ISubscription;

    loading: string;
    template: string;
    footerTemplate: string;
    userName: string;
    warnings: string[];
    messages: string[];
    copyViewModel: IDraggableViewModel;

    goHome = (right?: boolean) => {
        const newPane = this.clickHandler.pane(Pane.Pane1, right);

        if (this.configService.config.leftClickHomeAlwaysGoesToSinglePane && newPane === Pane.Pane1) {
            this.urlManager.setHomeSinglePane();
        } else {
            this.urlManager.setHome(newPane);
        }
    }

    goBack = () => {
        this.location.back();
    }

    goForward = () => {
        this.location.forward();
    }

    swapPanes = () => {
        if (!this.swapDisabled()) {
            this.context.swapCurrentObjects();
            this.urlManager.swapPanes();
        }
    }

    swapDisabled = () => {
        return this.urlManager.isMultiLineDialog() ? true : null;
    }

    singlePane = (right?: boolean) => {
        this.urlManager.singlePane(this.clickHandler.pane(Pane.Pane1, right));
    }

    logOff = () => this.urlManager.logoff();

    applicationProperties = () => this.urlManager.applicationProperties();

    recent = (right?: boolean) => {
        this.urlManager.setRecent(this.clickHandler.pane(Pane.Pane1, right));
    }

    cicero = () => {
        this.urlManager.singlePane(this.clickHandler.pane(Pane.Pane1));
        this.urlManager.cicero();
    }

    get currentCopyColor() {
        return this.copyViewModel.color;
    }

    get currentCopyTitle() {
        return this.copyViewModel.draggableTitle();
    }

    ngOnInit() {
        this.context.getUser().then(user => this.userName = user.userName()).catch((reject: ErrorWrapper) => this.error.handleError(reject));

        this.repLoader.loadingCount$.subscribe(count => this.loading = count > 0 ? Msg.loadingMessage : '');
        this.context.warning$.subscribe(ws => this.warnings = ws);
        this.context.messages$.subscribe(ms => this.messages = ms);
        this.dragAndDrop.copiedViewModel$.subscribe(cvm => this.copyViewModel = cvm);
    }

    ngOnDestroy() {
        safeUnsubscribe(this.warnSub);
        safeUnsubscribe(this.messageSub);
        safeUnsubscribe(this.cvmSub);
        safeUnsubscribe(this.lcSub);
    }
}
