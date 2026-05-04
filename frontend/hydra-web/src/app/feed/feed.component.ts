import { Component, OnInit, inject, signal, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BrowserProvider, ethers } from 'ethers';
import { HydraApiService } from '../services/hydra-api.service';
import { WalletAuthService } from '../services/wallet-auth.service';
import { BroadcastStrategyComponent, StoryStrategyComponent, FeedItem } from './feed-strategies';

@Component({
  selector: 'app-feed',
  standalone: true,
  imports: [CommonModule, FormsModule, BroadcastStrategyComponent, StoryStrategyComponent],
  templateUrl: './feed.component.html',
  styleUrls: ['./feed.component.css']
})
export class FeedComponent implements OnInit, OnDestroy {
  public api = inject(HydraApiService);
  public auth = inject(WalletAuthService);
  
  public feedItems = signal<FeedItem[]>([]);
  public newPostContent = signal<string>('');
  public debugLogs = signal<string[]>(['[System] Hydra Protocol Initialized']);
  
  private verifiedListener: any;

  ngOnInit() {
    this.fetchFeed();
    
    // Listen for SignalR broadcast verifications
    this.verifiedListener = (e: any) => {
      const hash = e.detail;
      this.feedItems.update(items => {
        return items.map(item => 
          item.contentHash === hash ? { ...item, status: 'Verified' } : item
        );
      });
      this.debugLogs.update(logs => [...logs, `[${new Date().toLocaleTimeString()}] Block Verified! Hash: ${hash.substring(0,10)}...`]);
    };
    window.addEventListener('broadcast-verified', this.verifiedListener);
  }

  ngOnDestroy() {
    window.removeEventListener('broadcast-verified', this.verifiedListener);
  }

  private fetchFeed() {
    this.api.get<FeedItem[]>('broadcast').subscribe({
      next: (items) => {
        const mapped = items.map((i: any) => ({
          ...i,
          type: 'broadcast',
          timestamp: i.timestamp || Date.now()
        }));
        this.feedItems.set(mapped);
      },
      error: (err) => console.error('Failed to fetch feed', err)
    });
  }

  public async broadcast() {
    const content = this.newPostContent().trim();
    if (!content) return;

    try {
      const provider = new BrowserProvider((window as any).ethereum);
      const signer = await provider.getSigner();
      
      const timestamp = Date.now();
      const contentHash = ethers.keccak256(ethers.toUtf8Bytes(content + timestamp));
      
      const signature = await signer.signMessage(contentHash);
      
      const payload = {
        authorWallet: this.auth.walletAddress() || '',
        content: content,
        contentHash: contentHash,
        signature: signature,
        timestamp: timestamp
      };

      // Optimistic UI
      const newItem: FeedItem = {
        id: contentHash,
        type: 'broadcast',
        authorWallet: payload.authorWallet,
        content: payload.content,
        contentHash: payload.contentHash,
        timestamp: payload.timestamp,
        status: 'Minting'
      };
      
      this.feedItems.update(items => [newItem, ...items]);
      this.newPostContent.set('');

      this.api.post('broadcast', payload).subscribe({
        next: () => {
          console.log('Broadcast submitted successfully');
          this.debugLogs.update(logs => [...logs, `[${new Date().toLocaleTimeString()}] Broadcast relayed to orchestrator`]);
        },
        error: (err) => console.error('Broadcast submission failed', err)
      });
      
    } catch (err) {
      console.error('Failed to sign broadcast', err);
    }
  }
}
