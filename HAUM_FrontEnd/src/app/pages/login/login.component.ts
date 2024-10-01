import { BackgroundComponent } from '../../elements/background/background.component';
import {
  FormControl,
  Validators,
  FormsModule,
  ReactiveFormsModule,
  FormBuilder,
  Form,
  FormGroup,
} from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { LogoComponent } from '../../elements/logo/logo.component';
import { ActivatedRoute, Router } from '@angular/router';
import { DataService } from '../../objects/data.service';
import { Component } from '@angular/core';
import { User } from '../../objects/user.model';

@Component({
  selector: 'app-login',
  standalone: true,
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
  imports: [
    BackgroundComponent,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    FormsModule,
    ReactiveFormsModule,
    LogoComponent,
  ],
})
export class LoginComponent {
  public hide = true;
  public loginForm: FormGroup;
  public loginResponse: string | undefined;

  constructor(
    private dataService: DataService,
    private formBuilder: FormBuilder,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
    });
  }

  public ngOnInit(): void {
    localStorage.setItem('guid', '');
    localStorage.setItem('deviceGUID', '');
  }

  public onSubmit(): void {
    if (this.loginForm.valid) {
      const user = new User();
      user.email = this.loginForm.value.email;
      user.password = this.loginForm.value.password;
      this.dataService.loginUser(user).subscribe({
        next: (res) => {
          this.loginResponse = res;
          console.log('Login Status: ' + res);
          localStorage.setItem('guid', this.loginResponse);
          this.router.navigate(['/home']);
        },
        error: (error) => {
          console.log('Login Error!');
        },
      });
    }
  }

  public getEmailErrorMessage(): string {
    if (this.loginForm.get('email')?.hasError('required')) {
      return 'You must enter an email';
    }
    return this.loginForm.get('email')?.hasError('email')
      ? 'Not a valid email'
      : '';
  }
  public getPasswordErrorMessage(): string {
    if (this.loginForm.get('password')?.hasError('required')) {
      return 'You must enter a password';
    }
    return this.loginForm.get('password')?.hasError('minLength')
      ? ''
      : 'Password too short';
  }
}
