import { Component, inject, Inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../../core/services/account-service';
import { Router, RouterLink, RouterLinkActive } from "@angular/router";

@Component({
  selector: 'app-nav',
  imports: [FormsModule, RouterLink, RouterLinkActive],
  templateUrl: './nav.html',
  styleUrl: './nav.css',
})
export class Nav {
  protected accountsService = inject(AccountService);
  protected router = inject(Router);
  protected creds: any = {};

  login(){
    this.accountsService.login(this.creds).subscribe({
      next: results => {
        this.router.navigateByUrl('/members');
        this.creds = {};
      },
      error: error => alert(error.message)
    });
  }

  logout(){
    this.accountsService.logout();
    this.router.navigateByUrl('/');
  }
}
