import { Component, ViewEncapsulation } from '@angular/core';
import { HeaderComponent } from '../../elements/header/header.component';
import { MatTabsModule } from '@angular/material/tabs';
import { SpinnerComponent } from '../../elements/spinner/spinner.component';
import { SensorDataComponent } from '../../elements/sensor-data/sensor-data.component';
import { MatButtonModule } from '@angular/material/button';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { DataService } from '../../objects/data.service';
import { Data } from '../../objects/data.model';
import { CanvasJSAngularChartsModule } from '@canvasjs/angular-charts';
import { MatDividerModule } from '@angular/material/divider';
import { SensorTypeEnum } from '../../objects/sensor-type-enum';

@Component({
  selector: 'app-device',
  standalone: true,
  templateUrl: './device.component.html',
  styleUrl: './device.component.scss',
  encapsulation: ViewEncapsulation.None,
  imports: [
    HeaderComponent,
    MatTabsModule,
    SpinnerComponent,
    SensorDataComponent,
    MatCardModule,
    MatIconModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    CanvasJSAngularChartsModule,
    MatDividerModule,
  ],
})
export class DeviceComponent {
  public sensorType: SensorTypeEnum | undefined;
  public dataPoints: any[] = [];
  public timeout: any = null;
  public chart: any;
  public timeDataList: Date[] = [];
  public valueDataList: Number[] = [];
  constructor(private dataService: DataService) {}
}
