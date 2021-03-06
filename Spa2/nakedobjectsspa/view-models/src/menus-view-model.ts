﻿import { ViewModelFactoryService } from './view-model-factory.service';
import { LinkViewModel } from './link-view-model';
import * as Models from '@nakedobjects/restful-objects';
import map from 'lodash-es/map';

import { Pane } from '@nakedobjects/services';

export class MenusViewModel {
    constructor(
        private readonly viewModelFactory: ViewModelFactoryService,
        private readonly menusRep: Models.MenusRepresentation,
        onPaneId: Pane
    ) {
        this.items = map(this.menusRep.value(), link => this.viewModelFactory.linkViewModel(link, onPaneId));
    }

    readonly items: LinkViewModel[];
}
