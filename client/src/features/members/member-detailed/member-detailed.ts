import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { filter } from 'rxjs';
import { Member } from '../../../types/member';
import { AgePipe } from '../../../core/pipes/age-pipe';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-member-detailed',
  imports: [
    RouterLink, 
    RouterLinkActive, 
    RouterOutlet,
    AgePipe
  ],
  templateUrl: './member-detailed.html',
  styleUrl: './member-detailed.css',
})
export class MemberDetailed implements OnInit{
  private activatedRoute = inject(ActivatedRoute);
  private router = inject(Router);
  protected member = signal<Member | undefined>(undefined);
  protected title = signal<string | undefined>('Profile');
  private destroyRef = inject(DestroyRef);
  
  ngOnInit(): void {
    this.activatedRoute.data.subscribe({
      next: data => this.member.set(data['member'])
    });

    this.title.set(this.activatedRoute.firstChild?.snapshot?.title);
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: () => {
        this.title.set(this.activatedRoute.firstChild?.snapshot?.title);
      }
    });
  }
}
