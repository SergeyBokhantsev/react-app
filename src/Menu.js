import "./Common.css";
import { useState } from 'react'
import BySyncJobId from "./Search/BySyncJobId";
import ByEmployeeId from "./Search/ByEmployeeId";
import BySyncDumpUrl from "./Search/BySyncDumpUrl";

const Menu = (props) => {

    const searchByJobIdHandler = () =>{
        setMenuContent(<BySyncJobId onBackClick={() => resetMenu()} onMessage={msg => props.onMessage(msg)} onModal={msg => props.onModal(msg)} client={props.client} workspaces={props.workspaces}/>);
    }

    const searchByEmployeeIdHandler = () => {
        setMenuContent(<ByEmployeeId onBackClick={() => resetMenu()} onMessage={msg => props.onMessage(msg)} onModal={msg => props.onModal(msg)} client={props.client} workspaces={props.workspaces}/>);
    }

    const bySyncJobUrlHandler = () => {
        setMenuContent(<BySyncDumpUrl onBackClick={() => resetMenu()} onMessage={msg => props.onMessage(msg)} onModal={msg => props.onModal(msg)}/>);
    }

    const defaultContent = (
        <div>
            <button onClick={bySyncJobUrlHandler}>I have SyncJob Download Url</button>
            { props.client !== null && <button onClick={searchByEmployeeIdHandler}>I have Employee Id</button> }
            { props.client !== null && <button onClick={searchByJobIdHandler}>I have SyncJob Id</button> }
        </div>
    );

    const resetMenu = () => {
        setMenuContent(defaultContent);
    }

    const [ menuContent, setMenuContent ] = useState(defaultContent);

    return menuContent;
}

export default Menu;