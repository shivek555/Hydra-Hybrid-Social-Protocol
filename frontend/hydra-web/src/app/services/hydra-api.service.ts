import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { SignalRService } from './signalr.service';

@Injectable({
  providedIn: 'root'
})
export class HydraApiService {
  private http = inject(HttpClient);
  private signalR = inject(SignalRService);
  private baseUrl = 'https://localhost:7198/api'; // Adjust port as needed

  public initializeRealtimeConnection() {
    const token = localStorage.getItem('hydra_token');
    if (token) {
      this.signalR.startConnection(token);
    }
  }

  public terminateRealtimeConnection() {
    this.signalR.stopConnection();
  }

  public post<T>(endpoint: string, body: any): Observable<T> {
    return this.http.post<T>(`${this.baseUrl}/${endpoint}`, body, this.getHeaders())
      .pipe(catchError(this.handleError));
  }

  public get<T>(endpoint: string): Observable<T> {
    return this.http.get<T>(`${this.baseUrl}/${endpoint}`, this.getHeaders())
      .pipe(catchError(this.handleError));
  }

  private getHeaders() {
    const token = localStorage.getItem('hydra_token');
    return {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Authorization: token ? `Bearer ${token}` : ''
      })
    };
  }

  private handleError(error: any) {
    console.error('API Error:', error);
    return throwError(() => new Error('An error occurred during the API request.'));
  }
}
