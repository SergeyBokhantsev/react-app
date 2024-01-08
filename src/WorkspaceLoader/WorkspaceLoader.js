import { useEffect } from 'react'
import ApiClient from "./../ApiClient.js"
import "./WorkspaceLoader.css"

const WorkspaceLoader = (props) => {

    useEffect(() => {
        new ApiClient().getWorkspaces().then((result) => {
            props.onWorkspaces(result);
        });
      })

    return( 
        <div className='capt'>
            <label>Loading Workspaces...</label>
        </div>);
}

export default WorkspaceLoader;