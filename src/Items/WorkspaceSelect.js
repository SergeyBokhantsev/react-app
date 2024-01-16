import { useState } from 'react'

const WorkspaceSelect = (props) => {

    const [workspaces, setWorkspaces] = useState(props.workspaces);

    const workspaceItemClickHandler = (event) => {
        const newSelectedState = event.target.className !== "w-item-selected";
        setWorkspaces(x => {
            const update = x.map(item => {
               return {
                ...item,
                selected: item.Id === event.target.id ? newSelectedState : item.selected
               };
            });

            props.onWorkspaceSelected(update.filter(x => x.selected === true).map(x => x.Id));

            return update;
        });
    }

    return (
        <div className="d-workspaces">
            {workspaces.map(x => <label key={x.Id} id={x.Id} className={x.selected ? "w-item-selected" : "w-item"} onClick={workspaceItemClickHandler}>{x.Name}</label>)}
        </div>
    );
}

export default WorkspaceSelect;