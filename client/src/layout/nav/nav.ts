import { Component, inject, Inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../../core/services/account-service';
import { RouterLink, RouterLinkActive } from "@angular/router";

@Component({
  selector: 'app-nav',
  imports: [FormsModule, RouterLink, RouterLinkActive],
  templateUrl: './nav.html',
  styleUrl: './nav.css',
})
export class Nav {
  protected accountsService = inject(AccountService)
  protected creds: any = {};

  login(){
    this.accountsService.login(this.creds).subscribe({
      next: results => {
        console.log(results);
        this.creds = {};
      },
      error: error => alert(error.message)
    });
  }

  logout(){
    this.accountsService.logout();
  }
}
