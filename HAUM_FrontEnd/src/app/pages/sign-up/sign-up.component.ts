import { Component } from '@angular/core';
import { BackgroundComponent } from '../../elements/background/background.component';
import {
  FormControl,
  Validators,
  FormsModule,
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
} from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { LogoComponent } from '../../elements/logo/logo.component';
import { Router, ActivatedRoute } from '@angular/router';
import { DataService } from '../../objects/data.service';
import { User } from '../../objects/user.model';

@Component({
  selector: 'app-sign-up',
  standalone: true,
  templateUrl: './sign-up.component.html',
  styleUrl: './sign-up.component.scss',
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
export class SignUpComponent {
  hide = true;
  signUpForm: FormGroup;
  signUpResponse: string | undefined;

  constructor(
    private dataService: DataService,
    private formBuilder: FormBuilder,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.signUpForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      username: ['', [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(8)]],
    });
  }

  public onSubmit(): void {
    if (this.signUpForm.valid) {
      const user = new User();
      user.username = this.signUpForm.value.username;
      user.email = this.signUpForm.value.email;
      user.password = this.signUpForm.value.password;
      console.log(user);
      this.dataService.signUpUser(user).subscribe({
        next: (res) => {
          console.log('SignUp Status: ' + res);
          this.router.navigate(['/login']);
        },
        error: (error) => {
          console.log('SignUp Error!');
        },
      });
    }
  }

  public getUsernameErrorMessage(): string {
    if (this.signUpForm.get('username')?.hasError('required')) {
      return 'You must enter a username';
    }
    return this.signUpForm.get('username')?.hasError('required')
      ? 'Not a valid email'
      : '';
  }
  public getEmailErrorMessage(): string {
    if (this.signUpForm.get('email')?.hasError('required')) {
      return 'You must enter an email';
    }
    return this.signUpForm.get('email')?.hasError('email')
      ? 'Not a valid email'
      : '';
  }
  public getPasswordErrorMessage(): string {
    if (this.signUpForm.get('password')?.hasError('required')) {
      return 'You must enter a password';
    }
    return this.signUpForm.get('password')?.hasError('minLength')
      ? ''
      : 'Password too short';
  }
}
