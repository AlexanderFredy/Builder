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
    mapId = "";
    async onCreate(options: any){
        console.log("Room create ",options);

        this.roomId = options.mapID;
        this.mapId = options.mapID.toLowerCase();
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

        this.onMessage("BlockSpawn", (client, data) => {
            this.broadcast("inbox_block_spawn",data.json,{except:client});
            this.sendSpawnBlock(client, data);
        });

        this.onMessage("BlockDestroy", (client, json) => {
            this.broadcast("inbox_block_destroy",json,{except:client});
            this.sendDestroyBlock(client, json);
        });
    }

    async sendSpawnBlock(client, data){     
        try{
            const response = await axios.post(Library.addBlock,{map: this.mapId, key: data.key, json: data.json});
            console.log(response.data);
        } catch(e){
            console.log('error by add block to DB: ' + e);
        }        
    }

    async sendDestroyBlock(client, json){
        try{
            const response = await axios.post(Library.removeBlock,{map: this.mapId, key: json});
            console.log(response.data);
        } catch(e){
            console.log('error by remove block to DB: ' + e);
        }   
    }

    onJoin(client: Client) {
        this.state.players.set(client.sessionId, new Player());
    }

    onLeave(client: Client, consented?: boolean) {
        this.state.players.delete(client.sessionId);
    }
}
