"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var signalr_client_1 = require("@aspnet/signalr-client");
var AppComponent = /** @class */ (function () {
    function AppComponent() {
        this.message = '';
        this.messages = [];
    }
    AppComponent.prototype.sendMessage = function () {
        var data = this.name + ": " + this.message;
        this._hubConnection.invoke('Send', data);
        this.messages.push(data);
    };
    AppComponent.prototype.ngOnInit = function () {
        var _this = this;
        this._hubConnection = new signalr_client_1.HubConnection('ChatHub');
        this._hubConnection.on('Send', function (data) {
            var received = "Received: " + data;
            _this.messages.push(received);
        });
        this._hubConnection.start()
            .then(function () {
            console.log('Hub connection started');
        })
            .catch(function () {
            console.log('Error while establishing connection');
        });
    };
    AppComponent = __decorate([
        core_1.Component({
            selector: 'app',
            templateUrl: "/app/app.component.html"
        }),
        __metadata("design:paramtypes", [])
    ], AppComponent);
    return AppComponent;
}());
exports.AppComponent = AppComponent;
//# sourceMappingURL=app.component.js.map