import "./Common.css";
import { useState } from 'react'
import BySyncJobId from "./Search/BySyncJobId";

const Menu = (props) => {

    const searchByJobIdHandler = () =>{
        setMenuContent(<BySyncJobId onBackClick={() => resetMenu()} client={props.client}/>);
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