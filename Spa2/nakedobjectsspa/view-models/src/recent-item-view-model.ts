﻿import { ContextService } from '@nakedobjects/services';
import { ColorService } from '@nakedobjects/services';
import { ErrorService } from '@nakedobjects/services';
import { UrlManagerService } from '@nakedobjects/services';
import { ClickHandlerService } from '@nakedobjects/services';
import { ViewModelFactoryService } from './view-model-factory.service';
import { ItemViewModel } from './item-view-model';
import * as Models from '@nakedobjects/restful-objects';
import { ConfigService } from '@nakedobjects/services';
import { Pane } from '@nakedobjects/services';

export class RecentItemViewModel extends ItemViewModel {

    constructor(
        context: ContextService,
        colorService: ColorService,
        error: ErrorService,
        urlManager: UrlManagerService,
        configService: ConfigService,
        link: Models.Link,
        paneId: Pane,
        clickHandler: ClickHandlerService,
        viewModelFactory: ViewModelFactoryService,
        index: number,
        isSelected: boolean,
        public readonly friendlyName: string
    ) {
        super(context, colorService, error, urlManager, configService, link, paneId, clickHandler, viewModelFactory, index, isSelected, '');
    }
}
