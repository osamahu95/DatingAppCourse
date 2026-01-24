import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';

@Component({
  selector: 'app-root',
  imports: [],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit{
  private http = inject(HttpClient); // Use Inject as per latest angular guide
  protected readonly title = signal('Dating App');
  
  // constructor(private http: HttpClient){} // old angular way - feels nostalgia. 
  
  ngOnInit(): void {
    this.http.get('https://localhost:5001/api/members').subscribe({
      next: response => console.log(response),
      error: error => console.log(error),
      complete: () => console.log("Completed the Http Request")
    })
  }
}
