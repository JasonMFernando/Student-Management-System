import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  private apiUrl = 'https://localhost:44390/api/Role/CreateRole';
  private statusUrl = 'https://localhost:44390/api/Status/GetAllStatuses';

  constructor(private http: HttpClient) {}

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token'); 
    return new HttpHeaders({ 'Authorization': `Bearer ${token}` });
  }

  getAllStatuses(): Observable<any[]> {
    return this.http.get<any[]>(this.statusUrl, { headers: this.getAuthHeaders() });
  }

  createRole(role: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, role, { headers: this.getAuthHeaders() });
  }
}
