import { Component, inject } from '@angular/core';
import { WalletAuthService } from '../services/wallet-auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  public authService = inject(WalletAuthService);

  connect() {
    this.authService.connectWallet();
  }
}
