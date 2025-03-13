import { Component, ViewEncapsulation  } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
  encapsulation: ViewEncapsulation.None,
})
export class LoginComponent {
  email: string = '';
  password: string = '';
  errorMessage: string = '';

  constructor(private http: HttpClient, private router: Router) {}

  onSubmit() {
    const loginData = { email: this.email, password: this.password };
    this.http.post<{ token: string }>('https://localhost:44390/api/User/Login', loginData)
      .subscribe({
        next: (response) => {
          console.log("JWT Token:", response.token)
          localStorage.setItem('token', response.token);
          this.router.navigate(['/user-management']);
        },
        error: () => {
          this.errorMessage = 'Invalid credentials';
        }
      });
  }
}
