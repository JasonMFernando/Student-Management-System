import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';


export interface User {
  userID: number;
  firstName: string;
  lastName: string;
  email: string;
  dateOfBirth: string;
  role: {
    roleID: number;
    roleName: string;
  };
  status: {
    statusID: number;
    statusName: string;
  };
  createdAt: string;
  modifiedAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private apiUrl = 'https://localhost:44390/api/User/GetAllUsers';
  private baseUrl = 'https://localhost:44390/api/User';
  private roleUrl = 'https://localhost:44390/api/Role/GetAllRoles';
  private statusUrl = 'https://localhost:44390/api/Status/GetAllStatuses';

  constructor(private http: HttpClient) { }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token'); 
    return new HttpHeaders({ 'Authorization': `Bearer ${token}` });
  }

  getAllUsers(): Observable<any> {
    const token = localStorage.getItem('token');  
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`  
    });

    return this.http.get<any>(this.apiUrl, { headers }); 
  }
  getUserById(userID: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/GetUser/${userID}` , { headers: this.getAuthHeaders() });
  }

  getAllRoles(): Observable<any[]> {
    return this.http.get<any[]>(this.roleUrl, { headers: this.getAuthHeaders() });
  }

  getAllStatuses(): Observable<any[]> {
    return this.http.get<any[]>(this.statusUrl , { headers: this.getAuthHeaders() });
  }

  updateUser(user: any): Observable<any> {

    return this.http.put<any>(`${this.baseUrl}/UpdateUser/${user.userID}`, user, { headers: this.getAuthHeaders() });
  }

  deleteUser(userID: number): Observable<any> {
  
    return this.http.delete(`https://localhost:44390/api/User/DeleteUser/${userID}`, { headers: this.getAuthHeaders() });
  }
  
}
