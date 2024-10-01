export class Device {
  public id: string;
  public name: string;
  public description: string;
  public ipAddress: string;
  public port: number;
  constructor() {
    this.id = '';
    this.name = '';
    this.ipAddress = '';
    this.port = 0;
    this.description = '';
  }
}
