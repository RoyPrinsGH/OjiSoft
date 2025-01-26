import { userManager } from "./OidcConfig";

async function login() {
    try {
        await userManager.signinRedirect();
    } catch (error) {
        console.error("Error during login:", error);
    }
}

async function logout() {
    try {
        await userManager.signoutRedirect();
    } catch (error) {
        console.error("Error during logout:", error);
    }
}

async function getUser() {
    try {
        const user = await userManager.getUser();
        if (user && !user.expired) {
            console.log("Authenticated user:", user);
            return user;
        } else {
            console.log("User not authenticated");
            return null;
        }
    } catch (error) {
        console.error("Error fetching user:", error);
        return null;
    }
}

async function renewToken() {
    try {
        const user = await userManager.signinSilent();
        console.log("Token renewed:", user);
        return user;
    } catch (error) {
        console.error("Token renewal error:", error);
    }
}

async function fetchWithToken(url: string, options: RequestInit = {}) {
    try {
      const user = await getUser();
      if (user && user.access_token) {
        options.headers = {
          ...options.headers,
          Authorization: `Bearer ${user.access_token}`,
        };
      }
      const response = await fetch(url, options);
      return response.json();
    } catch (error) {
      console.error("API call failed:", error);
    }
  }

export { login, logout, getUser, renewToken, fetchWithToken };