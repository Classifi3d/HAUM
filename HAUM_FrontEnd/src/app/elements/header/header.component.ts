import { Component } from '@angular/core';
import { LogoComponent } from '../logo/logo.component';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { DataService } from '../../objects/data.service';
import { User } from '../../objects/user.model';

@Component({
  selector: 'app-header',
  standalone: true,
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
  imports: [LogoComponent, MatButtonModule, MatMenuModule, MatIconModule],
})
export class HeaderComponent {
  public user: User | undefined;
  constructor(private dataService: DataService) {}

  public ngOnInit(): void {
    this.dataService.getUserByGUID().subscribe({
      next: (res) => {
        this.user = res;
      },
      error: (error) => {
        console.log('User Credential Retrival Error!');
      },
    });
  }
}
