import './App.css';
import Login from './Login/Login.js';
import { useState, useEffect } from 'react'
import Menu from './Menu.js';
import WorkspaceLoader from './WorkspaceLoader/WorkspaceLoader.js';
import ApiClient from './ApiClient.js';

const App = () => {

  console.log("enter App...");

  const [credentials, setCredentials] = useState();
  const [workspaces, setWorkspaces] = useState();

  useEffect(() => {
    console.log("App effect");
  }, []);

  const loginHandler = (creds) => {
    setCredentials(creds);
  }

  const workspacesHandler = (ws) => {
    setWorkspaces(ws);
  }

   const content = credentials == null 
   ? <Login onLoggedIn={loginHandler}/> 
   : workspaces == null
   ? <WorkspaceLoader onWorkspaces={workspacesHandler}/>
   : <Menu client={new ApiClient(credentials)} workspaces={workspaces}/>

  return(content);
}

export default App;
