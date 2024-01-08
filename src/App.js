import './App.css';
import Login from './Login/Login.js';
import { useState } from 'react'
import Menu from './Menu.js';
import WorkspaceLoader from './WorkspaceLoader/WorkspaceLoader.js';
import ApiClient from './ApiClient.js';

const App = () => {

  const [credentials, setCredentials] = useState();
  const [workspaces, setWorkspaces] = useState();

  const loginHandler = (creds) => {
    console.log('logged in');
    console.log(creds);
    setCredentials(creds);
  }

  const workspacesHandler = (workspaces) => {
    console.log(workspaces);
    setWorkspaces(workspaces);
  }

   const content = credentials == null 
   ? <Login onLoggedIn={loginHandler}/> 
   : workspaces == null
   ? <WorkspaceLoader onWorkspaces={workspacesHandler}/> 
   : <Menu client={new ApiClient(credentials, workspaces)}/>

  return(content);
}

export default App;
