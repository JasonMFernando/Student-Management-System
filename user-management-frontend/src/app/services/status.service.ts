import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class StatusService {
  private baseUrl = 'https://localhost:44390/api/Status';

  constructor(private http: HttpClient) {}

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token'); 
    return new HttpHeaders({ 'Authorization': `Bearer ${token}` });
  }

  createStatus(status: { statusName: string }): Observable<any> {
    const token = localStorage.getItem('jwtToken'); 
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.post(`${this.baseUrl}/CreateStatus`, status, { headers: this.getAuthHeaders() });
  }

  getAllStatuses(): Observable<any[]> {
    const token = localStorage.getItem('jwtToken'); 
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.get<any[]>(`${this.baseUrl}/GetAllStatuses`, { headers });
  }
}
