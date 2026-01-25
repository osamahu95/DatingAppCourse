import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { lastValueFrom } from 'rxjs';

@Component({
  selector: 'app-root',
  imports: [],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit{
  private http = inject(HttpClient); // Use Inject as per latest angular guide
  protected readonly title = signal('Dating App');
  protected members = signal<any>([]);
  
  // constructor(private http: HttpClient){} // old angular way - feels nostalgia. 
  
  async ngOnInit() {
    this.members.set(await this.getMembers());
  }

  async getMembers(){
    try{
      return lastValueFrom(this.http.get('https://localhost:5001/api/members'))
    }catch(error){
      console.log(error);
      throw error;
    }
  }
}
