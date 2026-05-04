import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SignalRService } from '../services/signalr.service';
import { WalletAuthService } from '../services/wallet-auth.service';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent {
  public signalR = inject(SignalRService);
  public auth = inject(WalletAuthService);
  
  public currentConversationId = signal<string>('global-chat-room');
  public messageInput = signal<string>('');

  public async sendMessage() {
    const content = this.messageInput();
    if (!content.trim()) return;

    // A nonce to prevent replay attacks on message
    const nonce = Date.now().toString();

    await this.signalR.sendMessage(this.currentConversationId(), content, nonce);
    this.messageInput.set('');
  }
}
