import { ChangeDetectorRef, Component } from '@angular/core';
import { HeaderComponent } from '../../elements/header/header.component';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import {
  FormControl,
  Validators,
  FormsModule,
  ReactiveFormsModule,
  FormBuilder,
  Form,
  FormGroup,
} from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { Device } from '../../objects/device.model';
import { Router, ActivatedRoute } from '@angular/router';
import { DataService } from '../../objects/data.service';
import { NgFor, NgIf } from '@angular/common';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatMenuModule } from '@angular/material/menu';
import { MatTooltipModule } from '@angular/material/tooltip';
@Component({
  selector: 'app-home',
  standalone: true,
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
  imports: [
    HeaderComponent,
    MatCardModule,
    MatIconModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatMenuModule,
    MatSelectModule,
    MatButtonModule,
    MatTooltipModule,
    NgFor,
    NgIf,
  ],
})
export class HomeComponent {
  public deviceName!: string;
  public selectedValue!: string;
  public deviceList: Device[] = [];
  public deviceForm: FormGroup;
  public isDisabled = true;
  public deviceIsSelected = false;
  public isLastDeleted = false;
  public isInEditMode = false;
  public lastSelectedDevice!: Device;
  constructor(
    private dataService: DataService,
    private formBuilder: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private _snackBar: MatSnackBar,
    private cdRef: ChangeDetectorRef
  ) {
    this.deviceForm = this.formBuilder.group({
      name: [{ value: '', disabled: true }, [Validators.required]],
      ipAddress: [{ value: '', disabled: true }, [Validators.required]],
      port: [{ value: '', disabled: true }, [Validators.required]],
      description: [{ value: '', disabled: true }, [Validators.required]],
    });
  }

  public ngOnInit(): void {
    this.getCurrentDevices();
    this.disableDeviceFields();
  }

  public saveDevice(): void {
    const device = new Device();
    device.name = this.deviceForm.value.name;
    device.ipAddress = this.deviceForm.value.ipAddress;
    device.port = this.deviceForm.value.port;
    device.description = this.deviceForm.value.description;
    this.dataService.addDevice(device).subscribe({
      next: (res) => {
        console.log(res);
        this.openSnackBar('New Device Added!');
        this.getCurrentDevices();
        this.disableDeviceFields();
      },
      error: () => {
        console.log('Error When Adding Device!');
        this.openSnackBar('Error When Adding Device!');
      },
    });
  }

  public selectDevice(device: Device): void {
    this.deviceForm.get('name')?.setValue(device.name);
    this.deviceForm.get('ipAddress')?.setValue(device.ipAddress);
    this.deviceForm.get('port')?.setValue(device.port);
    this.deviceForm.get('description')?.setValue(device.description);
    this.disableDeviceFields();
    this.deviceIsSelected = true;
    this.lastSelectedDevice = device;
    this.isLastDeleted = false;
    this.isInEditMode = false;
  }

  public accessSelectedDevice(): void {
    localStorage.setItem('deviceGUID', this.lastSelectedDevice.id);
    this.router.navigate(['/device']);
  }

  public addNewDevice(): void {
    this.deviceForm.get('name')?.setValue('');
    this.deviceForm.get('ipAddress')?.setValue('');
    this.deviceForm.get('port')?.setValue('');
    this.deviceForm.get('description')?.setValue('');
    this.enableDeviceFields();
  }

  public async deleteDevice(): Promise<void> {
    this.dataService.deleteDevice(this.lastSelectedDevice.id).subscribe({
      next: () => {
        this.openSnackBar('Deleted Device!');
        this.getCurrentDevices();
        this.cdRef.detectChanges();
        this.isInEditMode = false;
      },
      error: () => {
        console.log('Error When Deleting Device!');
        this.openSnackBar('Error When Deleting Device!');
      },
    });
    setTimeout(() => {
      this.getCurrentDevices();
      this.deviceForm.get('name')?.setValue('');
      this.deviceForm.get('ipAddress')?.setValue('');
      this.deviceForm.get('port')?.setValue('');
      this.deviceForm.get('description')?.setValue('');
      this.isDisabled = true;
    }, 100);
    this.cdRef.detectChanges();
    this.isLastDeleted = true;
  }

  public async getCurrentDevices(): Promise<void> {
    this.dataService.getDevices().subscribe({
      next: (devices: Device[]) => {
        this.deviceList = devices;
      },
    });
    this.cdRef.detectChanges();
  }

  public toggleEditMode(): void {
    if (this.isInEditMode) {
      this.isInEditMode = false;
    } else {
      this.isInEditMode = true;
    }
  }

  public enableDeviceFields(): void {
    if (this.deviceForm.disabled) {
      this.isDisabled = false;
      this.deviceForm?.enable();
    }
  }

  public disableDeviceFields(): void {
    if (!this.deviceForm.disabled) {
      this.isDisabled = true;
      this.deviceForm?.disable();
    }
  }

  public openSnackBar(message: string) {
    this._snackBar.open(message, 'Close', {
      duration: 3000,
    });
  }
}
