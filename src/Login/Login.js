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
    try{
  
      await this.publicClientApplication.initialize();
  
      await this.publicClientApplication.loginPopup(
        {
          scopes: msal_config.scopes,
          prompt: "select_account"
        }).then((response) => {
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
    this.customApSecret = event.target.value;
 }

 isEmpty(value) {
  return (value == null || (typeof value === "string" && value.trim().length === 0));
}

loginWithCustomCredentials() {

    const tenantId =  this.isEmpty(this.customTenantId) ? process.env.REACT_APP_DEF_TENANT_ID : this.customTenantId;
    const appId = this.isEmpty(this.customTenantId)  ? process.env.REACT_APP_DEF_CLIENT_ID : this.customTenantId;
    const appSecret = this.isEmpty(this.customTenantId)  ? process.env.REACT_APP_DEF_CLIENT_SECRET : this.customTenantId;

    const basicCreds = tenantId + ':' +appId + ":" + appSecret;

    this.props.onLoggedIn({ 
      type: 'Basic',
      name: this.customAppId,
      accessToken: btoa(basicCreds)
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