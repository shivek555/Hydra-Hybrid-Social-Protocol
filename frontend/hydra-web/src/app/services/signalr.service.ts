import { Injectable, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';

export interface ChatMessage {
  id: string;
  conversationId: string;
  senderWallet: string;
  encryptedBody: string;
  nonce: string;
  timestamp: string;
}

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection: signalR.HubConnection | null = null;
  public connectionState = signal<signalR.HubConnectionState>(signalR.HubConnectionState.Disconnected);
  public messages = signal<ChatMessage[]>([]);
  public onlineUsers = signal<Map<string, boolean>>(new Map());

  public startConnection(token: string) {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7198/hubs/chat', {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.onreconnecting(() => this.connectionState.set(signalR.HubConnectionState.Reconnecting));
    this.hubConnection.onreconnected(() => this.connectionState.set(signalR.HubConnectionState.Connected));
    this.hubConnection.onclose(() => this.connectionState.set(signalR.HubConnectionState.Disconnected));

    this.hubConnection.on('ReceiveMessage', (message: ChatMessage) => {
      this.messages.update(msgs => [...msgs, message]);
    });

    this.hubConnection.on('UserPresenceChanged', (walletAddress: string, status: string) => {
      const isOnline = status === 'Online';
      this.onlineUsers.update(map => {
        const newMap = new Map(map);
        newMap.set(walletAddress, isOnline);
        return newMap;
      });
    });

    this.hubConnection.on('BroadcastVerified', (contentHash: string) => {
      // Create a dedicated signal for verified hashes so components can react
      const event = new CustomEvent('broadcast-verified', { detail: contentHash });
      window.dispatchEvent(event);
    });

    this.hubConnection
      .start()
      .then(() => {
        this.connectionState.set(signalR.HubConnectionState.Connected);
        console.log('SignalR Connection started');
      })
      .catch(err => console.error('Error while starting connection: ' + err));
  }

  public stopConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop().then(() => {
        this.connectionState.set(signalR.HubConnectionState.Disconnected);
      });
    }
  }

  public async sendMessage(conversationId: string, content: string, nonce: string) {
    if (this.hubConnection && this.connectionState() === signalR.HubConnectionState.Connected) {
      await this.hubConnection.invoke('SendMessage', conversationId, content, nonce);
    } else {
      console.error('Cannot send message, SignalR is not connected');
    }
  }
}
