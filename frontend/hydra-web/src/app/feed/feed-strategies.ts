import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface FeedItem {
  id: string;
  type: 'broadcast' | 'story';
  authorWallet: string;
  content: string;
  contentHash: string;
  timestamp: number;
  status: string; // 'Minting' | 'Verified'
}

export interface IFeedItemStrategy {
  render(): any;
}

@Component({
  selector: 'app-broadcast-item',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="feed-item broadcast">
      <div class="feed-header">
        <span class="author">{{ item.authorWallet | slice:0:6 }}...{{ item.authorWallet | slice:-4 }}</span>
        <span class="badge" [ngClass]="item.status.toLowerCase()">
          {{ item.status === 'Verified' ? '✓ Verified on Ledger' : '⏳ Minting...' }}
        </span>
      </div>
      <div class="feed-content">{{ item.content }}</div>
      <div class="feed-footer">
        <span class="hash">Hash: {{ item.contentHash | slice:0:16 }}...</span>
        <span class="time">{{ item.timestamp | date:'short' }}</span>
      </div>
    </div>
  `,
  styleUrls: ['./feed-item.css']
})
export class BroadcastStrategyComponent implements IFeedItemStrategy {
  @Input() item!: FeedItem;
  render() {}
}

@Component({
  selector: 'app-story-item',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="feed-item story">
      <div class="feed-header">
        <span class="author">{{ item.authorWallet | slice:0:6 }}...{{ item.authorWallet | slice:-4 }}</span>
      </div>
      <div class="story-content">
        <em>{{ item.content }}</em>
      </div>
    </div>
  `,
  styleUrls: ['./feed-item.css']
})
export class StoryStrategyComponent implements IFeedItemStrategy {
  @Input() item!: FeedItem;
  render() {}
}
