import { useEffect } from 'react'
import ApiClient from "./../ApiClient.js"
import "./WorkspaceLoader.css"

const WorkspaceLoader = (props) => {

    useEffect(() => {
        new ApiClient().getWorkspaces()
                       .then(workspaces => props.onWorkspaces(workspaces))
                       .catch(err => props.onMessage(`Unable to load Workspaces: ${err.message}`))
    }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    , []);      

    return( 
        <div className='capt'>
            <label>Loading Workspaces...</label>
        </div>);
}

export default WorkspaceLoader;