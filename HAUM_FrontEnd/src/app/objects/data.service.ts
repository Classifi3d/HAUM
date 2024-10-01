import { Injectable, inject } from '@angular/core';
import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
  HttpParams,
  HttpResponse,
} from '@angular/common/http';
import { Observable, catchError, map, throwError } from 'rxjs';
import { User } from './user.model';
import { Device } from './device.model';
import { Data } from './data.model';
import { SensorTypeEnum } from './sensor-type-enum';

@Injectable({
  providedIn: 'root',
})
export class DataService {
  private url: string;
  private headers: HttpHeaders;
  private logedInHeader: HttpHeaders;
  private paramsUsers: HttpParams;
  constructor(public http: HttpClient) {
    const ip = 'localhost';
    const port = 7073;
    this.headers = new HttpHeaders()
      .set('Content-Type', 'application/json')
      .set('Access-Control-Allow-Headers', 'Content-Type')
      .set('Access-Control-Allow-Origin', '*');
    this.logedInHeader = new HttpHeaders()
      .set('Content-Type', 'application/json')
      .set('Access-Control-Allow-Headers', 'Content-Type')
      .set('Access-Control-Allow-Origin', '*')
      .set('Authorization', `Bearer ${localStorage.getItem('token')}`);

    this.url = `https://${ip}:${port}`;
    this.paramsUsers = new HttpParams();
  }

  // ========== LOGIN ==========
  loginUser(user: User): Observable<string> {
    const userBody = {
      email: user.email,
      password: user.password,
    };
    return this.http
      .post<string>(`${this.url}/user/login`, userBody, {
        headers: this.headers,
      })
      .pipe(
        map((response) => {
          return response as string;
        })
      );
  }

  // ========== SIGN UP ==========
  public signUpUser(user: User): Observable<void> {
    const userBody = {
      email: user.email,
      password: user.password,
      username: user.username,
    };
    return this.http.post<void>(`${this.url}/user`, userBody, {
      headers: this.headers,
    });
  }

  // ========== HOME ==========
  public getDevices(): Observable<Device[]> {
    const userGUID = localStorage.getItem('guid');
    return this.http
      .get<Device[]>(`${this.url}/device/${userGUID}`, {
        headers: this.headers,
        observe: 'response',
      })
      .pipe(map((response) => response.body as Device[]));
  }
  public addDevice(device: Device): Observable<void> {
    const userGUID = localStorage.getItem('guid');
    const deviceBody = {
      name: device.name,
      ipaddress: device.ipAddress,
      port: device.port,
      description: device.description,
    };
    return this.http.post<void>(`${this.url}/device/${userGUID}`, deviceBody, {
      headers: this.headers,
    });
  }

  public deleteDevice(deviceId: string): Observable<boolean> {
    const userGUID = localStorage.getItem('guid');
    return this.http
      .delete<boolean>(`${this.url}/device/${userGUID}/${deviceId}`, {
        headers: this.headers,
        observe: 'response',
      })
      .pipe(map((response) => response.body as boolean));
  }

  public getUserByGUID(): Observable<User> {
    const userGUID = localStorage.getItem('guid');
    return this.http
      .get<User>(`${this.url}/user/${userGUID}`, {
        headers: this.headers,
        observe: 'response',
      })
      .pipe(map((response) => response.body as User));
  }

  // ========== DEVICE ==========
  public getAllData(): Observable<Data[]> {
    const deviceGUID = localStorage.getItem('deviceGUID');
    return this.http
      .get<Data[]>(`${this.url}/data/${deviceGUID}`, {
        headers: this.headers,
        observe: 'response',
      })
      .pipe(map((response) => response.body as Data[]));
  }

  public getLastData(sensorType: SensorTypeEnum): Observable<Data> {
    const deviceGUID = localStorage.getItem('deviceGUID');
    return this.http
      .get<Data>(`${this.url}/data/${deviceGUID}/type/${sensorType}/last`, {
        headers: this.headers,
        observe: 'response',
      })
      .pipe(map((response) => response.body as Data));
  }

  public getSensorTypeData(sensorType: SensorTypeEnum): Observable<Data[]> {
    const deviceGUID = localStorage.getItem('deviceGUID');
    return this.http
      .get<Data[]>(`${this.url}/data/${deviceGUID}/type/${sensorType}`, {
        headers: this.headers,
        observe: 'response',
      })
      .pipe(map((response) => response.body as Data[]));
  }

  public getSpecificDateData(
    sensorType: SensorTypeEnum,
    date: Date
  ): Observable<Data[]> {
    const deviceGUID = localStorage.getItem('deviceGUID');
    return this.http
      .get<Data[]>(
        `${
          this.url
        }/data/${deviceGUID}/type/${sensorType}/date/${date.toISOString()}`,
        {
          headers: this.headers,
          observe: 'response',
        }
      )
      .pipe(map((response) => response.body as Data[]));
  }

  public getDateRangeData(
    sensorType: SensorTypeEnum,
    dateStart: Date,
    dateEnd: Date
  ): Observable<Data[]> {
    const deviceGUID = localStorage.getItem('deviceGUID');
    return this.http
      .get<Data[]>(
        `${
          this.url
        }/data/${deviceGUID}/type/${sensorType}/date-range/${dateStart.toISOString()}/${dateEnd.toISOString()}`,
        {
          headers: this.headers,
          observe: 'response',
        }
      )
      .pipe(map((response) => response.body as Data[]));
  }
}
