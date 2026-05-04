import { Injectable, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { BrowserProvider, ethers } from 'ethers';
import { HydraApiService } from './hydra-api.service';

declare global {
  interface Window {
    ethereum?: any;
  }
}

@Injectable({
  providedIn: 'root'
})
export class WalletAuthService {
  private api = inject(HydraApiService);
  
  public walletAddress = signal<string | null>(null);
  public isAuthenticated = signal<boolean>(false);

  public async connectWallet(): Promise<void> {
    if (typeof window.ethereum === 'undefined') {
      alert('MetaMask is not installed!');
      return;
    }

    try {
      const provider = new BrowserProvider(window.ethereum);
      await provider.send('eth_requestAccounts', []);
      const signer = await provider.getSigner();
      const address = await signer.getAddress();
      
      this.walletAddress.set(address);
      await this.authenticate(signer, address);
    } catch (error) {
      console.error('Wallet connection failed', error);
    }
  }

  private router = inject(Router);

  private async authenticate(signer: ethers.JsonRpcSigner, address: string) {
    const message = `Welcome to Project Hydra.\n\nPlease sign this message to authenticate your wallet.\nNonce: ${Date.now()}`;
    try {
      const signature = await signer.signMessage(message);
      
      this.api.post<{ token: string }>('auth/login', {
        walletAddress: address,
        message: message,
        signature: signature
      }).subscribe({
        next: (response: { token: string }) => {
          localStorage.setItem('hydra_token', response.token);
          this.api.initializeRealtimeConnection();
          this.isAuthenticated.set(true);
          this.router.navigate(['/feed']);
        },
        error: (err: any) => console.error('Login API failed', err)
      });
    } catch (error) {
      console.error('Signature failed', error);
    }
  }
}
