import { Component, OnInit } from '@angular/core';
import { HubConnection } from '@aspnet/signalr-client';

@Component({
    selector: 'app',
    templateUrl: `/app/app.component.html`
})
export class AppComponent  implements OnInit   {
    private _hubConnection: HubConnection;
    public async: any;
    name: string;
    group: string;
    message = '';
    messages: string[] = [];

    constructor() {
    }

    public sendMessage(): void {
        const data = `${this.name}: ${this.message }`;
        this._hubConnection.invoke('Send', data);
        this.messages.push(data);
    }
    
    ngOnInit() {
        this._hubConnection = new HubConnection('ChatHub');

        this._hubConnection.on('Send', (data: any) => {
            const received = `Received: ${data}`;
            this.messages.push(received);
        });

        this._hubConnection.start()
            .then(() => {
                console.log('Hub connection started')
            })
            .catch(() => {
                console.log('Error while establishing connection')
            }); 
    }

}
