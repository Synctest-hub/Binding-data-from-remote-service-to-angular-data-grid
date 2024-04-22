import { Component, ViewChild } from '@angular/core';
import { DataManager ,WebMethodAdaptor } from '@syncfusion/ej2-data';
import { GridComponent, EditSettingsModel, ToolbarItems } from '@syncfusion/ej2-angular-grids';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
})
export class AppComponent {
  public data?: DataManager;

  @ViewChild('grid')
  public grid?: GridComponent;
  public editSettings?: EditSettingsModel;
  public toolbar?: ToolbarItems[];

  ngOnInit(): void {
    this.data = new DataManager({
      url: 'https://localhost:7154/api/Grid', // Replace your hosted link
      insertUrl: 'https://localhost:7154/api/grid/Insert',
      updateUrl: 'https://localhost:7154/api/grid/Update',
      removeUrl: 'https://localhost:7154/api/grid/Remove',
      adaptor: new WebMethodAdaptor()
    });
    this.toolbar = ['Add', 'Update', 'Delete', 'Cancel', 'Search'];
    this.editSettings = { allowAdding: true, allowDeleting: true, allowEditing: true, mode: 'Normal' };
  }
}
