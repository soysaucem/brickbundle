import * as Cookie from "./Cookie.js";

//const apiUrl = "https://localhost:5001/api/";
const apiUrl = "https://brickbundle-api.azurewebsites.net/api/";

/**
 * Class to requests to the API.
 * Handles JWT session tokens and credential storage.
 */
export class ApiIO {
    constructor() { }

    /** JWT token is valid for the session. */
    private get Token(): string | null {
        return Cookie.GetCookie("token");
    }
    private set Token(value: string | null) {
        if (value != null) {
            Cookie.SetCookie("token", value);
        }
    }

    private RemoveToken() {
        Cookie.EraseCookie("token");
    }

    /**
     * Saves user credentials to a cookie.
     * @param credentials { Username: string, Password: string }
     * This will need to be switched to a more secure token based system.
     */
    private SaveCredentials(credentials: string) {
        Cookie.SetCookie("credentials", credentials, 365);
    }

    private RemoveCredentials() {
        Cookie.EraseCookie("credentials");
    }

    HasSavedCredentials(): boolean {
        return Cookie.GetCookie("credentials") != null;
    }

    IsLoggedIn(): boolean {
        return Cookie.GetCookie("token") != null;
    }

    Logout() {
        this.RemoveToken();
        this.RemoveCredentials();
    }

    /**
     * Fetches a session token if provided credentials are correct. 
     * @param username Username or email address
     * @param password User's password
     * @param rememberMe Saves credential to cookie if true
     */
    async Login(username: string, password: string, rememberMe: boolean = false): Promise<boolean> {
        this.Logout();
        if (navigator.onLine) {
            let data = JSON.stringify({ Username: username, Password: password });
            let response = await fetch(`${apiUrl}authenticate`, {
                method: "post",
                cache: "no-store",
                headers: {
                    "Accept": "application/json",
                    "Content-Type": "application/json",
                },
                body: data
            });
            if (response.status == 201) {
                this.Token = (await response.json()).token;
                if (rememberMe) {
                    this.SaveCredentials(data);
                }
                return true;
            }
            else if (response.status == 400) {
                return false;
            }
            else {
                throw new DOMException(`${response.status}`);
            }
        }
        else {
            throw new DOMException("Offline", "NetworkError");
        }
    }

    /** Logs in user with saved credentials. */
    private async AutoLogin(): Promise<boolean> {
        this.RemoveToken();
        let credentials = Cookie.GetCookie("credentials");
        if (credentials != null) {
            let response = await fetch(`${apiUrl}authenticate`, {
                method: "post",
                cache: "no-store",
                headers: {
                    "Accept": "application/json",
                    "Content-Type": "application/json",
                },
                body: credentials
            });
            if (response.status == 201) {
                this.Token = (await response.json()).token;
                return true;
            }
            else if (response.status == 400) {
                this.RemoveCredentials();
            }
            throw new DOMException("login failed", "UnknownError");
        }
        return false;
    }

    /**
     * Send HTTP POST request to API.
     * @param url Relative url for the post method
     * @param jsonData Data to post in json string format
     * @param requiresToken Whether the request requires the session token for authentication
     */
    async Post(url: string, jsonData: any, requiresToken: boolean = true): Promise<Response> {
        if (navigator.onLine) {
            let headers: Record<string, string> | undefined;
            if (requiresToken) {
                if (this.Token == null) {
                    if (!(await this.AutoLogin())) {
                        throw new DOMException("not logged in", "NotAllowedError");
                    }
                }
                headers = {
                    "Accept": "application/json",
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${this.Token}`
                };
            }
            else {
                headers = {
                    "Accept": "application/json",
                    "Content-Type": "application/json"
                };
            }

            let response = await fetch(`${apiUrl}${url}`, {
                method: "post",
                cache: "no-store",
                headers: headers,
                body: jsonData
            });
            return response;
        }
        else {
            throw new DOMException("Offline", "NetworkError");
        }
    }

    /**
     * Send HTTP POST request to API.
     * @param url Relative url for the post method
     * @param image Bytes from jpeg image
     * @param requiresToken Whether the request requires the session token for authentication
     */
    async PostImage(url: string, image: any, requiresToken: boolean = true): Promise<Response> {
        if (navigator.onLine) {
            let headers: Record<string, string> | undefined;
            if (requiresToken) {
                if (this.Token == null) {
                    if (!(await this.AutoLogin())) {
                        throw new DOMException("not logged in", "NotAllowedError");
                    }
                }
                headers = {
                    "Accept": "application/json",
                    "Content-Type": "image/jpeg",
                    "Authorization": `Bearer ${this.Token}`
                };
            }
            else {
                headers = {
                    "Accept": "application/json",
                    "Content-Type": "image/jpeg"
                };
            }

            let response = await fetch(`${apiUrl}${url}`, {
                method: "post",
                cache: "no-store",
                headers: headers,
                body: image
            });
            return response;
        }
        else {
            throw new DOMException("Offline", "NetworkError");
        }
    }

    /**
    * Send HTTP GET request to API.
    * @param url Relative url for the get method
    * @param requiresToken Whether the request requires the session token for authentication
    */
    async Get(url: string, requiresToken: boolean = true): Promise<Response> {
        if (navigator.onLine) {
            let headers: Record<string, string> | undefined;
            if (requiresToken) {
                if (this.Token == null) {
                    if (!(await this.AutoLogin())) {
                        throw new DOMException("not logged in", "NotAllowedError");
                    }
                }
                headers = {
                    "Accept": "application/json",
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${this.Token}`
                };
            }
            else {
                headers = {
                    "Accept": "application/json",
                    "Content-Type": "application/json"
                };
            }

            let response = await fetch(`${apiUrl}${url}`, {
                method: "get",
                cache: "reload",
                headers: headers
            });
            return response;
        }
        else {
            if (this.HasSavedCredentials()) {
                let response = await fetch(`${apiUrl}${url}`, {
                    method: "get",
                    cache: "force-cache",
                    headers:
                    {
                        "Accept": "application/json",
                        "Content-Type": "application/json"
                    }
                });
                return response;
            }
            throw new DOMException("Offline", "NetworkError");
        }
    }
}
