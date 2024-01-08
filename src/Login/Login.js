import "./Login.css"
import "../Common.css"
import { msal_config } from "../config.js"
import { PublicClientApplication } from '@azure/msal-browser';
import { Component } from 'react';

class Login extends Component {

  customTenantId = null;
  customAppId = null;
  customAppSecret = null;

  constructor(props){
    super(props);

    this.login = this.login.bind(this)

    this.publicClientApplication = new PublicClientApplication({
      auth: {
        clientId: msal_config.appId,
        redirectUrl: msal_config.redirectUrl,
        authority: msal_config.authority
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
          scopes: msal_config.scopes,
          prompt: "select_account"
        }).then((response) => {
          console.log(response);

          this.props.onLoggedIn({ 
                  type: 'Bearer',
                  name: response.account.name,
                  accessToken: response.accessToken
              });
        });
    }
    catch(err){
      console.log(err);  
    } 
  }
  
  logout() {
    this.publicClientApplication.logout();
  }
  
  customTenantIdChangeHandler(event) {
    this.customTenantId = event.target.value;
  }

  customAppIdChangeHandler(event) {
     this.customAppId = event.target.value;
  }

  customAppSecretChangeHandler(event) {
    this.customAppSecret = event.target.value;
 }

loginWithCustomCredentials() {
    this.props.onLoggedIn({ 
      type: 'Basic',
      name: this.customAppId,
      accessToken: btoa(this.customTenantId + ':' +this.customAppId + ":" + this.customAppSecret)
  })
}

  render() {
    return(
      <div className='login'>
        <div className="main-block">
          <button onClick={() => this.login()}>Log-in with RevenueGrid Account</button>
        </div>
        <div className="main-block">
          <p>or use your custom app credentials</p>
          <div className="hor">
            <div className="login-with-secrets-labels">
              <p>Tenant Id:</p>
              <p>Client Id:</p>
              <p>App Secret:</p>
            </div>
            <div className="login-with-secrets-inputs">
              <p><input onChange={(event) => this.customTenantIdChangeHandler(event)}></input></p>
              <p><input onChange={(event) => this.customAppIdChangeHandler(event)}></input></p>
              <p><input onChange={(event) => this.customAppSecretChangeHandler(event)} type="password"></input></p>
            </div>
          </div>        
        </div>
        <button onClick={() => this.loginWithCustomCredentials()} className="button-small">Apply</button>
      </div>
  );
  }
  
  }

export default Login;