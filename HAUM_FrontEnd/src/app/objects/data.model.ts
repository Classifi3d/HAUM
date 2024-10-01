import { SensorTypeEnum } from './sensor-type-enum';

export class Data {
  public time: Date;
  public type: SensorTypeEnum;
  public dataValue: number;
  constructor() {
    this.time = new Date();
    this.type = SensorTypeEnum.Temperature;
    this.dataValue = 0;
  }
}
