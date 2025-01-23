import { UserManager, WebStorageStateStore, Log } from "oidc-client-ts";

const oidcConfig = {
  authority: 'https://localhost:7241',   // The URL of your OpenIddict server
  client_id: "ojiaccport",          // The client_id registered in OpenIddict
  redirect_uri: "http://localhost:5173/handleloginredirect", // Where to return after login
  post_logout_redirect_uri: "http://localhost:5173/",     // Where to return after logout
  response_type: "code",                // Use the authorization code flow
  scope: "openid profile offline_access", // Requested scopes
  // PKCE is automatically enabled for `response_type=code` in oidc-client-ts
  
  // Where to store session data; "sessionStorage" or "localStorage" can be used
  userStore: new WebStorageStateStore({ store: window.localStorage }),
};

// Optional logging (for debugging):
// You can set Log.setLogger(console) or Log.setLevel(Log.DEBUG)
// depending on how verbose you want the logs.
Log.setLogger(console);
Log.setLevel(Log.DEBUG);

const userManager = new UserManager(oidcConfig);

userManager.events.addSilentRenewError((error) => {
  console.error("Silent renew error:", error);
});

userManager.events.addUserLoaded((user) => {
  console.log("User loaded:", user);
});

userManager.events.addUserUnloaded(() => {
  console.log("User session ended");
});

export { userManager };