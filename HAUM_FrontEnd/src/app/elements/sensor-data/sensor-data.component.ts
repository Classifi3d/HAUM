import { Component, Input, ViewEncapsulation } from '@angular/core';
import { HeaderComponent } from '../../elements/header/header.component';
import { MatTabsModule } from '@angular/material/tabs';
import { SpinnerComponent } from '../../elements/spinner/spinner.component';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { DataService } from '../../objects/data.service';
import { SensorTypeEnum } from '../../objects/sensor-type-enum';
import { Data } from '../../objects/data.model';
import { CanvasJSAngularChartsModule } from '@canvasjs/angular-charts';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatNativeDateModule } from '@angular/material/core';
import { NgIf, JsonPipe } from '@angular/common';
import { MatCheckboxModule } from '@angular/material/checkbox';

import {
  FormGroup,
  FormControl,
  FormBuilder,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';

@Component({
  selector: 'app-sensor-data',
  standalone: true,
  templateUrl: './sensor-data.component.html',
  styleUrl: './sensor-data.component.scss',
  encapsulation: ViewEncapsulation.None,
  imports: [
    HeaderComponent,
    MatTabsModule,
    SpinnerComponent,
    SensorDataComponent,
    HeaderComponent,
    MatCardModule,
    MatIconModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    CanvasJSAngularChartsModule,
    MatFormFieldModule,
    MatDatepickerModule,
    FormsModule,
    ReactiveFormsModule,
    NgIf,
    JsonPipe,
    MatNativeDateModule,
    MatCheckboxModule,
  ],
})
export class SensorDataComponent {
  @Input() public sensorType: SensorTypeEnum = SensorTypeEnum.Temperature;
  public dataPoints: any[] = [];
  public timeout: any = null;
  public chart: any;
  public timeDataList: Date[] = [];
  public valueDataList: Number[] = [];
  public isDisabled = true;
  public currentDataValue!: number;
  constructor(private dataService: DataService, private form: FormBuilder) {}

  public range = new FormGroup({
    start: new FormControl({ value: null, disabled: true }),
    end: new FormControl({ value: null, disabled: true }),
  });

  public chartOptions = {
    animationEnabled: true,

    theme: 'light2',
    data: [
      {
        type: 'column',
        dataPoints: this.dataPoints,
        color: '#3364af',
      },
    ],
  };

  public ngOnInit(): void {
    this.dataService.getLastData(this.sensorType).subscribe((data: Data) => {
      this.currentDataValue = data.dataValue;
    });
  }

  public ngOnDestroy(): void {
    clearTimeout(this.timeout);
  }

  public toggleDateRange(): void {
    if (this.isDisabled) {
      this.isDisabled = false;
    } else {
      this.isDisabled = true;
    }
  }

  public customDateSearch(): void {
    console.log(this.range);
    if (this.range.value.start) {
      if (this.range.value.end) {
        console.log('date-range');
        this.dataService
          .getDateRangeData(
            this.sensorType,
            this.range.value.start,
            this.range.value.end
          )
          .subscribe((datas: Data[]) => {
            this.dataPoints.length = 0;
            datas.forEach((data) => {
              this.timeDataList.push(new Date(data.time));
              this.valueDataList.push(data.dataValue);
              this.dataPoints.push({
                x: new Date(data.time),
                y: data.dataValue,
              });
            });
            this.chart.render();
            console.log(this.dataPoints);
          });
      } else {
        console.log('specific-date');
        this.dataService
          .getSpecificDateData(this.sensorType, this.range.value.start)
          .subscribe((datas: Data[]) => {
            this.dataPoints.length = 0;
            datas.forEach((data) => {
              this.timeDataList.push(new Date(data.time));
              this.valueDataList.push(data.dataValue);
              this.dataPoints.push({
                x: new Date(data.time),
                y: data.dataValue,
              });
            });
            this.chart.render();
            console.log(this.dataPoints);
          });
      }
    }
  }
  public getChartInstance(chart: object): void {
    setTimeout(() => {});
    this.chart = chart;
    this.chart.options = this.chartOptions;
    this.updateData();
  }

  public updateData = () => {
    this.dataService
      .getSensorTypeData(this.sensorType)
      .subscribe((datas: Data[]) => {
        this.dataPoints.length = 0;
        datas.forEach((data) => {
          this.timeDataList.push(new Date(data.time));
          this.valueDataList.push(data.dataValue);
          this.dataPoints.push({
            x: new Date(data.time),
            // x: dataIndex,
            y: data.dataValue,
          });
        });
        console.log(this.dataPoints);
        this.chart.render();
      });
    this.timeout = setTimeout(this.updateData, 60000);
  };
}
