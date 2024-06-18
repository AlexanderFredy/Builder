import { Schema, type } from "@colyseus/schema";
import { Client, LobbyRoom } from "colyseus";

export class LobbyState extends Schema{
    @type("number") empty = 0; 
}

export class MyLobbyRoom extends LobbyRoom
{
    async onCreate(options: any) {
        await super.onCreate(options);

        this.setState(new LobbyState());
    }

    onJoin(client, options) {
        super.onJoin(client,options);
    }

    onDispose() {
        super.onDispose();
    }
}