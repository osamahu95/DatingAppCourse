import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../../core/services/account-service';
import { Router, RouterLink, RouterLinkActive } from "@angular/router";
import { ToastService } from '../../core/services/toast-service';
import { LoginCreds } from '../../types/user';

@Component({
  selector: 'app-nav',
  imports: [FormsModule, RouterLink, RouterLinkActive],
  templateUrl: './nav.html',
  styleUrl: './nav.css',
})
export class Nav {
  protected accountsService = inject(AccountService);
  protected router = inject(Router);
  private toast = inject(ToastService);
  protected creds: LoginCreds = {
    email: '',
    password: ''
  };

  login(){
    this.accountsService.login(this.creds).subscribe({
      next: () => {
        this.router.navigateByUrl('/members');
        this.toast.success('Logged In Successfully');
        this.creds = {
          email: '',
          password: ''
        };
      },
      error: error => {
        this.toast.error(error.error);
      }
    });
  }

  logout(){
    this.accountsService.logout();
    this.router.navigateByUrl('/');
  }
}
