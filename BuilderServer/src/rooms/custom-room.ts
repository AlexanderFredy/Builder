import {Schema, type} from "@colyseus/schema";
import {Client, Room, updateLobby} from "colyseus";
import { Library } from "../Library";
import axios from "axios";

export class RoomState extends Schema{
    @type("number") empty = 0; 
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

        this.onMessage("",(client,data) =>{         
        });
    }

    onJoin(client: Client, options?: any, auth?: any): void | Promise<any> {
        
    }
}
