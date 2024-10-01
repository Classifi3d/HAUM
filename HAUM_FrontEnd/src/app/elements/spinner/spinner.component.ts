import { Component, Input } from '@angular/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSliderModule } from '@angular/material/slider';
import { FormsModule } from '@angular/forms';
import { MatRadioModule } from '@angular/material/radio';
import { MatCardModule } from '@angular/material/card';
import { CommonModule } from '@angular/common';
import { SensorTypeEnum } from '../../objects/sensor-type-enum';

@Component({
  selector: 'app-spinner',
  standalone: true,
  imports: [
    MatCardModule,
    MatRadioModule,
    FormsModule,
    MatSliderModule,
    MatProgressSpinnerModule,
    CommonModule,
  ],
  templateUrl: './spinner.component.html',
  styleUrl: './spinner.component.scss',
})
export class SpinnerComponent {
  @Input() value!: number;
  @Input() sensorType!: SensorTypeEnum;
  public valueType!: string;
  public spinnerColor!: string;
  public spinnerProgress!: number;

  public ngOnChanges(): void {
    if (this.sensorType == SensorTypeEnum.Temperature) {
      this.setProgressTemperature();
    }
    if (this.sensorType == SensorTypeEnum.Pressure) {
      this.setProgressPressure();
    }
    if (this.sensorType == SensorTypeEnum.Humidity) {
      this.setProgressHumidity();
    }
    if (this.sensorType == SensorTypeEnum.Illumination) {
      this.setProgressLightLevel();
    }
  }

  public setProgressTemperature(): void {
    this.valueType = '°C';
    this.spinnerProgress = (this.value + 20) * 1.25;
    this.spinnerColor = 'circle-dark-blue';
    if (this.value >= -20 && this.value < 0) {
      this.spinnerProgress = this.spinnerProgress + 0.5;
      this.spinnerColor = 'circle-dark-blue';
    } else if (this.value <= 10) {
      this.spinnerColor = 'circle-light-blue';
    } else if (this.value <= 20) {
      this.spinnerColor = 'circle-green';
    } else if (this.value <= 30) {
      this.spinnerColor = 'circle-yellow';
    } else if (this.value <= 40) {
      this.spinnerColor = 'circle-orange';
    } else if (this.value <= 60) {
      this.spinnerColor = 'circle-red';
    }
  }

  public setProgressPressure(): void {
    this.valueType = 'PSI';
    this.spinnerProgress = (this.value - 40) * 2.5;
    if (this.value >= 40 && this.value < 45) {
      this.spinnerProgress = this.spinnerProgress + 0.5;
      this.spinnerColor = 'circle-dark-blue';
    } else if (this.value < 50) {
      this.spinnerColor = 'circle-light-blue';
    } else if (this.value < 60) {
      this.spinnerColor = 'circle-green';
    } else if (this.value < 70) {
      this.spinnerColor = 'circle-yellow';
    } else if (this.value < 75) {
      this.spinnerColor = 'circle-orange';
    } else if (this.value <= 80) {
      this.spinnerColor = 'circle-red';
    }
  }

  public setProgressHumidity(): void {
    this.valueType = '%';
    this.spinnerProgress = this.value;
    if (this.value >= 0 && this.value < 15) {
      this.spinnerProgress = this.spinnerProgress + 0.5;
      this.spinnerColor = 'circle-red';
    } else if (this.value < 25) {
      this.spinnerColor = 'circle-orange';
    } else if (this.value < 30) {
      this.spinnerColor = 'circle-yellow';
    } else if (this.value <= 60) {
      this.spinnerColor = 'circle-green';
    } else if (this.value < 70) {
      this.spinnerColor = 'circle-yellow';
    } else if (this.value < 85) {
      this.spinnerColor = 'circle-orange';
    } else if (this.value <= 100) {
      this.spinnerColor = 'circle-red';
    }
  }

  public setProgressLightLevel(): void {
    this.valueType = 'LUX';
    this.spinnerProgress = this.value;
    if (this.value >= 0 && this.value <= 33) {
      this.spinnerProgress = this.spinnerProgress + 0.5;
      this.spinnerColor = 'circle-light-blue';
    } else if (this.value <= 66) {
      this.spinnerColor = 'circle-green';
    } else if (this.value <= 100) {
      this.spinnerColor = 'circle-orange';
    }
  }
}
