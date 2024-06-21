import {Schema, type, MapSchema} from "@colyseus/schema";
import {Client, Room, updateLobby} from "colyseus";
import { Library } from "../Library";
import axios from "axios";

export class Player extends Schema{
    //@type("number")
    //speed = 0;

    @type("number")
    pX = 0;

    @type("number")
    pY = 0;

    @type("number")
    pZ = 0;

    @type("number")
    vX = 0;

    @type("number")
    vY = 0;

    @type("number")
    vZ = 0;

    @type("number")
    rX = 0;

    @type("number")
    rY = 0;
}

export class RoomState extends Schema{
    @type({map: Player}) players = new MapSchema<Player>(); 

    movePlayer (sessionId: string, data: any) {
        const player = this.players.get(sessionId);
        player.pX = data.pX;
        player.pY = data.pY;
        player.pZ = data.pZ;
        player.vX = data.vX;
        player.vY = data.vY;
        player.vZ = data.vZ;
        player.rX = data.rX;
        player.rY = data.rY;
    }
}

export class CustomRoom extends Room<RoomState>{

    async onCreate(options: any){
        console.log("Room create ",options);

        this.roomId = options.mapID;
        this.setPrivate(options.isPrivate);

        try{
            const response = await axios.post(Library.getMap,{roomid: options.mapID, userid: options.userid})
            console.log(response.data);
        } catch(e){
            console.log('error by connect to DB: ' + e);
        }
        
        this.setState(new RoomState());

        this.onMessage("move", (client, data) => {
            this.state.movePlayer(client.sessionId, data);
        });
    }

    onJoin(client: Client) {
        this.state.players.set(client.sessionId, new Player());
    }

    onLeave(client: Client, consented?: boolean) {
        this.state.players.delete(client.sessionId);
    }
}
