import './App.css';
import { config } from "./config.js"
import { PublicClientApplication } from '@azure/msal-browser';
import { Component } from 'react';

class App extends Component {

constructor(props){
  super(props);
  this.state = {
    error: null,
    isAuthenticated: false,
    authResponse: {}
  };
  this.login = this.login.bind(this)
  this.publicClientApplication = new PublicClientApplication({
    auth: {
      clientId: config.appId,
      redirectUrl: config.redirectUrl,
      authority: config.authority
    },
    cache: {
      cacheLocation: "sessionStorage",
      storeAuthStateInCookie: true
    }
  })
}

async login(){
  console.log("begin login");
  try{

    await this.publicClientApplication.initialize();

    await this.publicClientApplication.loginPopup(
      {
        scopes: config.scopes,
        prompt: "select_account"
      }).then((response) => {
        console.log(response);
        this.setState(
          {
            isAuthenticated: true,
            authResponse: response
          });
      });

      
  }
  catch(err){
    console.log(err);
    this.setState({
      isAuthenticated: false,
      authResponse: {},
      error: err
    });
  } 
}

logout() {
  this.publicClientApplication.logout();
}

render() {
  return(
    <div className='App'>
      <header className='App-header'>
        { this.state.isAuthenticated ? <p>LOGGED IN as {this.state.authResponse.account.name}</p> 
        : <p><button onClick={() => this.login()}>Log-in</button></p>
        }
      </header>
    </div>
  );
}

}

export default App;
