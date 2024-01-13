import "./Common.css";
import { useState } from 'react'
import BySyncJobId from "./Search/BySyncJobId";

const Menu = (props) => {

    const searchByJobIdHandler = () =>{
        setMenuContent(<BySyncJobId onBackClick={() => resetMenu()} onMessage={msg => props.onMessage(msg)} onModal={msg => props.onModal(msg)} client={props.client} workspaces={props.workspaces}/>);
    }

    const defaultContent = (
        <div>
            <button onClick={searchByJobIdHandler}>I have SyncJob Id</button>
        </div>
    );

    const resetMenu = () => {
        setMenuContent(defaultContent);
    }

    const [ menuContent, setMenuContent ] = useState(defaultContent);

    return menuContent;
}

export default Menu;