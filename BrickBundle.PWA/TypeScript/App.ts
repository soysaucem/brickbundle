import { PageHandler } from "./PageHandler.js"
import { pages } from "./PageHandler.js"
import { ApiIO } from "./ApiIO.js";
import * as Validation from "./Validation.js";

let Page: PageHandler;
let IO: ApiIO;
/** Used to prompt user to install app. */
//let deferredPrompt: any | null = null;

window.onload = (function () {
    Init();
    Start();
});

/** Initializes app classes and event listners. */
function Init() {
    // Register the service worker
    if ("serviceWorker" in navigator) {
        if (navigator.serviceWorker.controller) {
            // active service worker found, no need to register
        } else {
            navigator.serviceWorker
                .register("sw.js", {
                    scope: "./"
                })
                .then(function (reg) {
                    // Service worker has been registered
                });
        }
    }

    IO = new ApiIO();
    Page = new PageHandler(IO);

    // Install events
    //window.addEventListener("beforeinstallprompt", (e) => { deferredPrompt = e; });

    // History events
    window.onpopstate = (e: PopStateEvent) => { Page.HandlePopState(e); }

    // Scroll events
    document.getElementById("page10")!.onscroll = (e) => { Page.ScrollAddBricks(); };

    // Click events
    // Menubar
    document.getElementById("menubar")!.onclick = (e) => { e.preventDefault(); MenuBarClick(e); };
    // Toolbar
    document.getElementById("toolbarbtnBack")!.onclick = (e) => { e.preventDefault(); window.history.back() };
    document.getElementById("toolbarbtnAccount")!.onclick = (e) => { e.preventDefault(); Page.GoTo(pages.Account); };
    document.getElementById("toolbarbtnAddParts")!.onclick = (e) => { e.preventDefault(); Page.GoTo(pages.AddParts); };
    document.getElementById("toolbarbtnTick")!.onclick = (e) => { e.preventDefault(); Page.AddPart(); };
    // Gotos
    document.getElementById("gotoLogin")!.onclick = (e) => { e.preventDefault(); Page.GoTo(pages.Login); };
    document.getElementById("gotoRegister")!.onclick = (e) => { e.preventDefault(); Page.GoTo(pages.Register) };
    document.getElementById("gotoForgotPassword")!.onclick = (e) => { e.preventDefault(); Page.GoTo(pages.ResetPassword); };
    document.getElementById("btnChooseCategory")!.onclick = (e) => { e.preventDefault(); Page.GoTo(pages.Categories); };
    // Functional buttons
    document.getElementById("btnRegister")!.onclick = (e) => { e.preventDefault(); Register(); };
    document.getElementById("btnLogin")!.onclick = (e) => { e.preventDefault(); Login(); };
    document.getElementById("btnSendResetPassword")!.onclick = (e) => { e.preventDefault(); SendResetPassword(); };
    document.getElementById("btnResetPassword")!.onclick = (e) => { e.preventDefault(); ResetPassword(); };
    document.getElementById("btnSendVerifyEmail")!.onclick = (e) => { e.preventDefault(); SendVerificationEmail(); };
    document.getElementById("btnVerifyEmail")!.onclick = (e) => { e.preventDefault(); VerifyEmail() };
    document.getElementById("btnLogOut")!.onclick = (e) => { e.preventDefault(); Logout(); };
    document.getElementById("btnSearchMyParts")!.onclick = (e) => { e.preventDefault(); Page.SearchMyParts(); };
    document.getElementById("btnSearchAddParts")!.onclick = (e) => { e.preventDefault(); Page.SearchAddBricks() };
    document.getElementById("btnRemoveCategory")!.onclick = (e) => { e.preventDefault(); Page.RemoveAddCategory(); };
    // Container clicks
    document.getElementById("addparts-container")!.onclick = (e) => { e.preventDefault(); Page.PartClick(e); };
    document.getElementById("category-container")!.onclick = (e) => { e.preventDefault(); Page.CategoryClick(e) };

    // Input events
    document.getElementById("captureParts")!.addEventListener("change", e => { UploadImage(); });
    document.getElementById("searchMyParts")!.addEventListener("keypress", e => { if (e.which == 13 || e.keyCode == 13) { Page.SearchMyParts(); } });
    document.getElementById("searchAddParts")!.addEventListener("keypress", e => { if (e.which == 13 || e.keyCode == 13) { Page.SearchAddBricks(); } });
}

/** Navigates to the specified url. */
function Start() {
    Page.GoToUrl(window.location.pathname);
}

/** Prompts user to install app. Currently not supported on iOS. */
//async function Install() {
//    if (deferredPrompt != null) {
//        deferredPrompt.prompt();
//        deferredPrompt.userChoice.then(function (choiceResult: any) {
//            deferredPrompt = null;
//        });
//    }
//}

/** Registers user. */
async function Register() {
    let registerForm = <HTMLFormElement>document.getElementById("registerForm");
    let username = registerForm.username.value;
    let emailAddress = registerForm.emailAddress.value;
    let password = registerForm.password.value;
    let confirmPassword = registerForm.confirmPassword.value;
    let rememberMe = registerForm.rememberMe.checked;

    Validation.ResetErrors(registerForm);
    let errorElements = registerForm.getElementsByClassName("validation-error");
    let usernameError = <HTMLElement>errorElements.namedItem("usernameError");
    let emailError = <HTMLElement>errorElements.namedItem("emailError");
    let passwordError = <HTMLElement>errorElements.namedItem("passwordError");
    let hasErrors = false;

    const usernameRegex = /^[a-zA-Z0-9_]+$/;
    const emailRegex = /^(([^<>()\[\]\.,;:\s@\"]+(\.[^<>()\[\]\.,;:\s@\"]+)*)|(\".+\"))@(([^<>()[\]\.,;:\s@\"]+\.)+[^<>()[\]\.,;:\s@\"]{2,})$/i;

    if (!username) {
        Validation.AppendError(usernameError, "Required");
        hasErrors = true;
    }
    else {
        if (username.length < 3) {
            Validation.AppendError(usernameError, "Must be at least 3 characters long");
            hasErrors = true;
        }
        if (!usernameRegex.test(username)) {
            Validation.AppendError(usernameError, "Must only contain letters, numbers and underscores");
            hasErrors = true;
        }
    }

    if (!emailAddress) {
        Validation.AppendError(emailError, "Required");
        hasErrors = true;
    }
    else {
        if (!emailRegex.test(emailAddress)) {
            Validation.AppendError(emailError, "Invalid email address");
            hasErrors = true;
        }
    }

    if (!password) {
        Validation.AppendError(passwordError, "Required");
        hasErrors = true;
    }
    else {
        if (password.length < 6) {
            Validation.AppendError(passwordError, "Must be at least 6 characters long");
            hasErrors = true;
        }
        if (password !== confirmPassword) {
            Validation.AppendError(passwordError, "Passwords do not match");
            hasErrors = true;
        }
    }

    if (hasErrors) {
        return;
    }

    Page.HideCurrentPage();
    Page.ShowLoading();
    let data = JSON.stringify({ Username: username, EmailAddress: emailAddress, Password: password });
    let response = null;
    try {
        response = await IO.Post("user", data, false);
        if (response.status == 201) {
            if (await IO.Login(username, password, rememberMe)) {
                Page.GoTo(pages.Dashboard);
                return;
            }
        }
    }
    catch (ex) {
        let domEx = ex as DOMException;
        if (domEx != null) {
            if (domEx.message == "Offline") {
                Page.GoTo(pages.Offline)
                return
            }
        }
    }
    Page.HideLoading();
    Page.ShowCurrentPage();
    if (response != null) {
        let responseText = await response.text();
        if (response.status == 400) {
            if (responseText.includes("username")) {
                Validation.AppendError(usernameError, "Invalid username");
            }
            if (responseText.includes("email")) {
                Validation.AppendError(emailError, "Invalid email address");
            }
            return;
        }
        else if (response.status == 409) {
            if (responseText.includes("username")) {
                Validation.AppendError(usernameError, "Username already exists");
            }
            if (responseText.includes("email")) {
                Validation.AppendError(emailError, "Email address aldredy exists");
            }
            return;
        }
    }
    Page.GoTo(pages.Error)
}

/** Logs user in. */
async function Login() {
    let loginForm = <HTMLFormElement>document.getElementById("loginForm");
    let username = loginForm.username.value;
    let password = loginForm.password.value;
    let rememberMe = loginForm.rememberMe.checked;

    Validation.ResetErrors(loginForm);
    let errorElements = loginForm.getElementsByClassName("validation-error");
    let usernameError = <HTMLElement>errorElements.namedItem("usernameError");
    let passwordError = <HTMLElement>errorElements.namedItem("passwordError");
    let hasErrors = false;

    if (!username || username.length < 1) {
        Validation.AppendError(usernameError, "Required");
        hasErrors = true;
    }
    if (!password || password.length < 1) {
        Validation.AppendError(passwordError, "Required");
        hasErrors = true;
    }

    if (hasErrors) {
        return;
    }

    Page.HideCurrentPage();
    Page.ShowLoading();
    try {
        if (await IO.Login(username, password, rememberMe)) {
            Page.GoTo(pages.Dashboard);
            return;
        }
        else {
            Page.HideLoading();
            Page.ShowCurrentPage();
            Validation.AppendError(passwordError, "Invalid username or password");
            return;
        }
    }
    catch (ex) {
        let domEx = ex as DOMException;
        if (domEx != null) {
            if (domEx.message == "Offline") {
                Page.GoTo(pages.Offline)
                return
            }
        }
    }
    Page.GoTo(pages.Error)
}

/** Logs user out. */
function Logout() {
    IO.Logout();
    Page.GoTo(pages.Login);
    location.reload();
}

/** Handles clicking on menubar items. */
function MenuBarClick(e: MouseEvent) {
    var targetElement = <HTMLElement>e.target;
    if (!targetElement.classList.contains("menubar-item")) {
        var menuitem = <HTMLElement>targetElement.closest(".menubar-item");
        let name = menuitem.getAttribute("name")
        switch (name) {
            case "home":
                Page.GoTo(pages.Dashboard);
                break;
            case "bricks":
                Page.GoTo(pages.MyParts);
                break;
            case "sets":
                Page.GoTo(pages.Sets);
                break;
        }
    }
}

/** Sends verification email. */
async function SendVerificationEmail() {
    Page.HideCurrentPage();
    Page.ShowLoading();
    try {
        let response = await IO.Get("user/verify", true);
        if (response.status == 200) {
            Page.GoTo(pages.VerifyEmail);
            return;
        }
    }
    catch { }
    Page.HideLoading();
    Page.ShowCurrentPage();
    Validation.AppendError(document.getElementById("sendVerifyEmailError")!, "Error Sending Email");
}

/** Verifies email if entered code is valid. */
async function VerifyEmail() {
    let verifyEmailForm = <HTMLFormElement>document.getElementById("verifyEmailForm");
    let code = verifyEmailForm.code.value;

    Validation.ResetErrors(verifyEmailForm);
    let errorElements = verifyEmailForm.getElementsByClassName("validation-error");
    let codeError = <HTMLElement>errorElements.namedItem("codeError");

    if (!code) {
        Validation.AppendError(codeError, "Required");
        return
    }

    Page.HideCurrentPage();
    Page.ShowLoading();
    try {
        let response = await IO.Post("user/verify", JSON.stringify(code), true);
        if (response.status == 200) {
            Page.HideLoading();
            Page.ShowCurrentPage();
            Validation.AppendSuccess(codeError, "Email Verified");
            document.getElementById("btnVerifyEmail")!.style.display = "none";
            document.getElementById("btnSendVerifyEmail")!.parentElement!.style.display = "none";
            return;
        }
    }
    catch { }
    Page.HideLoading();
    Page.ShowCurrentPage();
    Validation.AppendError(codeError, "Invalid Code");
}

/** Semds reset pasword code. */
async function SendResetPassword() {
    let btn = <HTMLButtonElement>document.getElementById("btnSendResetPassword");
    let sendResetPasswordForm = <HTMLFormElement>document.getElementById("sendResetPasswordForm");
    let username = sendResetPasswordForm.username.value;

    Validation.ResetErrors(sendResetPasswordForm);
    let errorElements = sendResetPasswordForm.getElementsByClassName("validation-error");
    let usernameError = <HTMLElement>errorElements.namedItem("usernameError");
    console.log(username);

    if (!username) {
        Validation.AppendError(usernameError, "Required");
        return
    }

    Page.HideCurrentPage();
    Page.ShowLoading();
    try {
        let response = await IO.Get(`user/reset/${username}`, false);
        if (response.status == 200) {
            sendResetPasswordForm.style.display = "none";
            document.getElementById("resetPasswordForm")!.style.display = "";
        }
        else if (response.status == 404) {
            Validation.AppendError(usernameError, "No such username or email address");
        }
        else {
            Validation.AppendError(usernameError, "Error Sending Email, please try again later");
            btn.disabled = true;
        }
    }
    catch { }
    Page.HideLoading();
    Page.ShowCurrentPage();
}

/** Resets password if entered code is valid. */
async function ResetPassword() {
    let sendResetPasswordForm = <HTMLFormElement>document.getElementById("sendResetPasswordForm");
    let username = sendResetPasswordForm.username.value;
    let resetPasswordForm = <HTMLFormElement>document.getElementById("resetPasswordForm");
    let code = resetPasswordForm.code.value;
    let password = resetPasswordForm.password.value;
    let confirmPassword = resetPasswordForm.confirmPassword.value;

    Validation.ResetErrors(resetPasswordForm);
    let errorElements = resetPasswordForm.getElementsByClassName("validation-error");
    let codeError = <HTMLElement>errorElements.namedItem("codeError");
    let passwordError = <HTMLElement>errorElements.namedItem("passwordError");
    let resetError = <HTMLElement>errorElements.namedItem("resetError");
    let hasErrors = false;

    if (!code) {
        Validation.AppendError(codeError, "Required");
        hasErrors = true;
    }

    if (!password) {
        Validation.AppendError(passwordError, "Required");
        hasErrors = true;
    }
    else {
        if (password.length < 6) {
            Validation.AppendError(passwordError, "Must be at least 6 characters long");
            hasErrors = true;
        }
        if (password !== confirmPassword) {
            Validation.AppendError(passwordError, "Passwords do not match");
            hasErrors = true;
        }
    }

    if (hasErrors) {
        return;
    }

    Page.HideCurrentPage();
    Page.ShowLoading();
    try {
        let response = await IO.Post(`user/reset/${username}`, JSON.stringify({ Code: code, Password: password }), false);
        if (response.status == 200) {
            if (await IO.Login(username, password, false)) {
                Validation.AppendSuccess(resetError, "Password Reset, Logging you in");
                setTimeout(function () {
                    Page.GoTo(pages.Dashboard);
                }, 1000);
                return;
            }
        }
    }
    catch { }
    Validation.AppendError(resetError, "Invalid Code");
    Page.HideLoading();
    Page.ShowCurrentPage();
}

/** Uploads image for brick detection and adds resulting parts to the user's collection. */
async function UploadImage() {
    var inputElement = <HTMLInputElement>document.getElementById("captureParts");
    if (inputElement.files) {
        Page.HideCurrentPage();
        Page.Toolbar.SetTitle("Adding");
        Page.ShowLoading();
        var fileData = new Blob([inputElement.files![0]]);
        var fr = new FileReader();
        fr.readAsArrayBuffer(fileData);
        fr.onload = async function (event) {
            var bytes = (<FileReader>event.target).result
            var result = await IO.PostImage("part/image", bytes, true)
            var parts = await result.json();
            if (parts) {
                await Page.AddToMyParts(parts);
            }
            Page.HideLoading();
            Page.ShowCurrentPage();
            Page.Toolbar.SetTitle("My Bricks");
        };
    }
}
