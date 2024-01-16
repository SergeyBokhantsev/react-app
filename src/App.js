import './App.css';
import Login from './Login/Login.js';
import { useState } from 'react'
import Menu from './Menu.js';
import WorkspaceLoader from './WorkspaceLoader/WorkspaceLoader.js';
import ApiClient from './ApiClient.js';
import Message from './UI/Message.js';

const App = () => {

  console.log("enter App...");

  const [credentials, setCredentials] = useState(null);
  const [workspaces, setWorkspaces] = useState();
  const [message, setMessage] = useState({ closeButton: false });

  const loginHandler = (creds) => {
    setCredentials(creds);
  }

  const workspacesHandler = (ws) => {
    setWorkspaces(ws);
  }

  const messageHandler = (msg) => {
    setMessage({ 
        text: msg,
        closeButton: true  
      });
  }

  const modalScreenHandler = (msg) => {
    setMessage({ 
        text: msg,
        closeButton: false  
      });
  }


  const getApiClientOrNull = () => {
      return (credentials !== null && credentials.type  !== 'Anonymous') ? new ApiClient(credentials) : null;
  }

  const needToLoadWorkspaces = credentials !== null && credentials.type  !== 'Anonymous' && workspaces === undefined;

   const content = credentials === null 
   ? <Login onLoggedIn={loginHandler}/> 
   : needToLoadWorkspaces
   ? <WorkspaceLoader onWorkspaces={workspacesHandler} onMessage={messageHandler}/>
   : <Menu client={getApiClientOrNull()} workspaces={workspaces} onMessage={messageHandler} onModal={modalScreenHandler}/>

  const showMessage = message !== undefined && message !== null && message.text !== undefined && message.text !== null && message.text !== "";

  return(<div>
            {content}
            <Message isOpen={showMessage} showCloseButton={message.closeButton} closeModal={() => messageHandler(null)}>
              <h1>{message.text}</h1>
            </Message>
         </div>);
}

export default App;
