import "./BySyncJobId.css"
import { useState } from 'react'
import ApiClient from './../ApiClient.js'

const BySyncJobId = (props) =>{

    const [workspaces, setWorkspaces] = useState(props.client.workspaces);

    console.log(workspaces);

    const workspaceItemClickHandler = (event) => {
        const newSelectedState = event.target.className !== "w-item-selected";
        setWorkspaces(x => {
            return x.map(item => {
               return {
                ...item,
                selected: item.id === event.target.id ? newSelectedState : item.selected
               };
            });
        });
    }

    const searchClickHandler = () => {
        //props.client.ge
    }

    return (
        <div className="root">
            <div className="d0">
                <div className="d1">
                    <button className="btn-back" onClick={() => props.onBackClick()}>Back</button>
                    <div className="d2">
                        <p><input type="search" className="i1" placeholder="Job Id"/></p>
                        <p><button onClick={searchClickHandler} className="b2">Search</button></p>
                    </div>
                    <div className="d2">
                        <p><input type="search" className="i1" placeholder="Job Id"/></p>
                        <p><button onClick={searchClickHandler} className="b2">Search</button></p>
                    </div>
                </div>
                <div className="d3">
                    {workspaces.map(x => <p key={x.id} id={x.id} className={x.selected ? "w-item-selected" : "w-item"} onClick={workspaceItemClickHandler}>{x.name}</p>)}
                </div>
            </div>
          
        </div>);
}

export default BySyncJobId;