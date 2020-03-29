import { ApiIO } from "./ApiIO.js";
import { toolbarbtn } from "./ToolbarHandler.js";
import { ToolbarHandler } from "./ToolbarHandler.js";
import { menubarbtn } from "./MenubarHandler.js";
import { MenubarHandler } from "./MenubarHandler.js";


// To add a new page:
// 1. add to enum pages
// 2. add to PageHandler.pagedetails
// 3. add <section id="page{num}" class="content" style="display:none;"></section> to index.html
/** Pages in the app. */
export enum pages {
    None = 0,
    Offline = 1,
    Error = 2,
    Dashboard = 3,
    Login = 4,
    Register = 5,
    Account = 6,
    MyParts = 7,
    VerifyEmail = 8,
    ResetPassword = 9,
    AddParts = 10,
    Categories = 11,
    PartDetails = 12,
    Sets = 13,
}

let loadedParts = false;
let displayedCategories = false;
let userparts: any = null;
let partsColorsCategories: any = null;
let addPartsCategory: string | null = null;
let addPartsSearchTerm: string = "";
let currentPart: any = null;
let loadedBuildSets = false;
let buildSets: any = null;

const addScrollItems = 100;
let currentScrollItems = 0;

/** Handles page switching and navigation. */
export class PageHandler {
    readonly pagedetails: Record<pages, PageDetail> = {
        // [page]: new PageDetail(title, url, redirectTo, toolbarbtns)
        [pages.None]: new PageDetail("", null, null, null),
        [pages.Offline]: new PageDetail("Offline", null, null, null),
        [pages.Error]: new PageDetail("Error", null, null, null),
        [pages.Dashboard]: new PageDetail("Dashboard", "/", null, [toolbarbtn.Account]),
        [pages.Login]: new PageDetail("Login", "/login", null, null),
        [pages.Register]: new PageDetail("Register", "/register", null, null),
        [pages.Account]: new PageDetail("My Account", "/account", null, null),
        [pages.MyParts]: new PageDetail("My Bricks", "/bricks", null, [toolbarbtn.AddParts, toolbarbtn.CaptureParts]),
        [pages.VerifyEmail]: new PageDetail("Verify", "/account/verify", pages.Account, null),
        [pages.ResetPassword]: new PageDetail("Reset Password", "/resetpassword", null, null),
        [pages.AddParts]: new PageDetail("Add Bricks", "/bricks/add", null, null),
        [pages.Categories]: new PageDetail("Category", "/bricks/add/catagory", null, null),
        [pages.PartDetails]: new PageDetail("Add", "/bricks/{code}", null, [toolbarbtn.Tick]),
        [pages.Sets]: new PageDetail("Sets", "/sets", null, null),
    }

    IO: ApiIO;
    Toolbar = new ToolbarHandler();
    Menubar = new MenubarHandler();
    CurrentPage: pages = pages.None;
    LoadingPage: HTMLElement = document.getElementById("loading")!;
    /** The number of states the page can go back to. */
    BackStates = 0;

    constructor(io: ApiIO) {
        this.IO = io;
    }

    /**
     * Navigates to page for url.
     * @param url Url of page
     */
    GoToUrl(url: string) {
        let lowerUrl = url.toLowerCase();
        if (lowerUrl == "" || lowerUrl == "/") {
            this.GoTo(pages.Dashboard);
            return;
        }
        if (lowerUrl == "/logout") {
            this.IO.Logout();
            this.GoTo(pages.Login);
            location.reload();
            return;
        }
        for (let item in pages) {
            let pageNo = Number(item)
            if (!isNaN(pageNo)) {
                let detail = this.pagedetails[<pages>pageNo];
                if (detail.Url == lowerUrl) {
                    if (detail.RedirectTo != null) {
                        this.GoTo(detail.RedirectTo);
                    }
                    else {
                        this.GoTo(pageNo);
                    }
                    return;
                }
            }
        }
        this.GoTo(pages.Dashboard);
    }

    /**
     * Navigates to page.
     * @param page Page to navigate to
     */
    GoTo(page: pages) {
        if (page === this.CurrentPage) {
            return;
        }
        this.GoToState(new PageState(page));
    }

    /**
     * Goes to page with specified state.
     * @param state Page state
     * @param isBack Whether the bage is being returned to
     */
    async GoToState(state: PageState, isBack: boolean = false) {
        this.HideCurrentPage();
        this.Menubar.Hide();
        this.ShowLoading();

        if (isBack) {
            this.BackStates = state.BackStates;
        }

        if (this.IO.IsLoggedIn() || this.IO.HasSavedCredentials()) {
            // Logged in
        }
        else if (navigator.onLine) {
            // Online but not logged in
            if (this.CurrentPage == pages.None && state.Page == pages.ResetPassword) {
                this.CurrentPage = pages.Login;
                history.replaceState(new PageState(pages.Login), this.pagedetails[pages.Login].Title, this.pagedetails[pages.Login].Url);
            }
            else if (state.Page != pages.Login && state.Page != pages.Register && state.Page != pages.ResetPassword) {
                state = new PageState(pages.Login);
                isBack = false;
            }
        }
        else {
            state = new PageState(pages.Offline);
            isBack = false;
        }

        let pageDetail = this.pagedetails[state.Page];
        let title = pageDetail.Title;
        let url = pageDetail.Url;
        this.Toolbar.SetTitle(title);
        this.Toolbar.ShowBtns(pageDetail.ToolbarBtns)

        switch (state.Page) {
            case pages.Dashboard:
                this.Menubar.Show();
                this.Menubar.SelectBtn(menubarbtn.Home)
                break;
            case pages.MyParts:
                if (!loadedParts) {
                    this.GetMyParts();
                    loadedParts = true;
                }
                this.Menubar.Show();
                this.Menubar.SelectBtn(menubarbtn.Bricks)
                break;
            case pages.AddParts:
                if (partsColorsCategories == null) {
                    await this.GetAllParts();
                }
                break;
            case pages.Categories:
                if (!displayedCategories) {
                    this.ShowCategories();
                    displayedCategories = true;
                }
                break;
            case pages.PartDetails:
                let myPart;
                for (let part of partsColorsCategories.parts) {
                    if (part.id == state.Data) {
                        myPart = part;
                        currentPart = part;
                        break;
                    }
                }
                document.getElementById("addPart-id")!.textContent = myPart.id;
                document.getElementById("addPart-name")!.textContent = myPart.name;
                document.getElementById("addPart-code")!.textContent = myPart.code;
                url = (<string>url).replace("{code}", myPart.code);
                break;
            case pages.Sets:
                if (!loadedBuildSets) {
                    await this.GetSetsCanBuild();
                    this.ShowSetsCanBuild();
                }
                this.Menubar.Show();
                this.Menubar.SelectBtn(menubarbtn.Sets);
                break;
        }

        if (!isBack) {
            if (state.Page == pages.Offline || state.Page == pages.Dashboard || state.Page == pages.Login || state.Page == pages.Register || state.Page == pages.MyParts || state.Page == pages.Sets) {
                history.replaceState(state, title, url);
                this.BackStates = 0;
            }
            else {
                ++this.BackStates;
                state.BackStates = this.BackStates;
                history.pushState(state, title, url);
            }
        }

        if (this.BackStates > 0) {
            this.Toolbar.ShowBtn(toolbarbtn.Back);
        }

        this.CurrentPage = state.Page;
        this.HideLoading();
        this.ShowCurrentPage();
    }

    /** Makes the current page visible. */
    ShowCurrentPage() {
        document.getElementById(`page${this.CurrentPage}`)!.style.display = "";
    }

    /** Makes the current page hidden. */
    HideCurrentPage() {
        document.getElementById(`page${this.CurrentPage}`)!.style.display = "none";
    }

    /** Overlays loading animation. */
    ShowLoading() {
        this.LoadingPage.style.display = "";
    }

    /** Hides loading animation. */
    HideLoading() {
        this.LoadingPage.style.display = "none";
    }

    /** Handles popstate events when a page in history is navigated to. */
    HandlePopState(e: PopStateEvent) {
        let state = e.state as PageState;
        if (state != null) {
            this.GoToState(state, true);
        }
        else {
            this.GoTo(pages.Dashboard);
        }
    }

    /** Gets parts for user and displays them in a list. */
    async GetMyParts() {
        try {
            let response = await this.IO.Get(`user/parts`, true);
            if (response.status == 200) {
                userparts = await response.json();
                this.SearchMyParts();
            }
        }
        catch { }
    }

    /** Displays user's parts that contain the search term. */
    SearchMyParts() {
        let term = (<HTMLInputElement>document.getElementById("searchMyParts")).value.toLowerCase();
        const container = document.getElementById("myparts-container")!;
        const template = <HTMLTemplateElement>document.getElementById("userPartTemplate");

        container.innerHTML = "";
        for (let userpart of userparts) {
            if ((<string>userpart.part.name).toLowerCase().includes(term) || (<string>userpart.part.code).toLowerCase().includes(term)) {
                let element = document.importNode(template.content.querySelector("li")!, true);
                let idElement = <HTMLElement>element.getElementsByClassName("part-id").item(0);
                let nameElement = <HTMLElement>element.getElementsByClassName("part-name").item(0);
                let codeElement = <HTMLElement>element.getElementsByClassName("part-code").item(0);
                let qtyElement = <HTMLElement>element.getElementsByClassName("part-qty").item(0);

                idElement.textContent = userpart.part.id;
                nameElement.textContent = userpart.part.name;
                codeElement.textContent = userpart.part.code;
                let total = 0;
                for (let colorQuantity of userpart.colorQuantities) {
                    total += colorQuantity.quantity;
                }
                qtyElement.textContent = total.toString();

                if (userpart.part.code == "3001" || userpart.part.code == "3003" || userpart.part.code == "3004" || userpart.part.code == "3005") {
                    let imgElement = <HTMLImageElement>(<HTMLElement>element.getElementsByClassName("part-img").item(0)).firstChild;
                    imgElement.src = `/images/${userpart.part.code}.png`;
                }

                container.appendChild(element);
            }
        }
    }

    /** Fetches all parts, categories and colors. */
    async GetAllParts() {
        try {
            let response = await this.IO.Get(`part`, true);
            if (response.status == 200) {
                partsColorsCategories = await response.json();
            }
        }
        catch { }
    }

    /** Shows a list of part categories. */
    ShowCategories() {
        const container = document.getElementById("category-container")!;
        const template = <HTMLTemplateElement>document.getElementById("categoryTemplate");
        let categories = partsColorsCategories.categories;
        for (let category of categories) {
            let element = document.importNode(template.content.querySelector("li")!, true);
            element.textContent = category;
            container.appendChild(element);
        }
    }

    /** Filters add-bricks by category. */
    CategoryClick(e: MouseEvent) {
        let target = <HTMLElement>e.target;
        if (target.className.includes("category")) {
            document.getElementById("btnChooseCategory")!.innerText = `Category: ${target.textContent}`
            document.getElementById("btnRemoveCategory")!.style.display = "";
            addPartsCategory = target.textContent
            this.ShowAddBricks();
        }
        history.back();
    }

    /** Filters add-bricks by search term. */
    SearchAddBricks() {
        addPartsSearchTerm = (<HTMLInputElement>document.getElementById("searchAddParts")).value.toLowerCase();
        this.ShowAddBricks();
    }

    /** Displays the first 100 add-brick matching the filters. */
    ShowAddBricks() {
        const container = document.getElementById("addparts-container")!;
        container.innerHTML = "";
        currentScrollItems = 0;
        if (addPartsSearchTerm || addPartsCategory != null) {
            this.ShowNextAddBricks();
        }
    }

    /** Displays the next 100 add-brick matching the filters. */
    ShowNextAddBricks() {
        const container = document.getElementById("addparts-container")!;
        const template = <HTMLTemplateElement>document.getElementById("addPartTemplate");
        let newitems = currentScrollItems + addScrollItems;
        let i = 0;
        for (let part of partsColorsCategories.parts) {
            if (((<string>part.name).toLowerCase().includes(addPartsSearchTerm) || (<string>part.code).toLowerCase().includes(addPartsSearchTerm)) && (addPartsCategory == null || part.category == addPartsCategory)) {
                if (++i > currentScrollItems) {
                    let element = document.importNode(template.content.querySelector("li")!, true);
                    let idElement = <HTMLElement>element.getElementsByClassName("part-id").item(0);
                    let nameElement = <HTMLElement>element.getElementsByClassName("part-name").item(0);
                    let codeElement = <HTMLElement>element.getElementsByClassName("part-code").item(0);
                    //let imgElement = <HTMLElement>element.getElementsByClassName("part-img").item(0);

                    idElement.textContent = part.id;
                    nameElement.textContent = part.name;
                    codeElement.textContent = part.code;

                    container.appendChild(element);
                    if (i == newitems) {
                        break;
                    }
                }
            }
        }
        currentScrollItems = i;
    }

    /** Removes the add-brick category filter. */
    RemoveAddCategory() {
        document.getElementById("btnChooseCategory")!.innerText = "Select Category";
        document.getElementById("btnRemoveCategory")!.style.display = "none";
        addPartsCategory = null;
        this.ShowAddBricks();
    }

    /** Displays more add-bricks if the scroll position igreater 80% */
    ScrollAddBricks() {
        const page = document.getElementById("page10")!;
        let val = page.scrollTop / page.scrollHeight;
        if (val > 0.8) {
            this.ShowNextAddBricks();
        }
    }

    /** Navigates to the part's details page. */
    PartClick(e: MouseEvent) {
        let target = <HTMLElement>e.target;
        let element = target.closest(".addPart");
        if (element) {
            let id = (<HTMLElement>element.getElementsByClassName("part-id").item(0)).textContent;
            this.GoToState(new PageState(pages.PartDetails, id));
        }
    }

    /**
     * Adds parts to users parts.
     * @param userparts Parts, colors and quantities to add to user
     */
    async AddToMyParts(userparts: any) {
        try {
            let data = JSON.stringify(userparts);
            console.log(data)
            let response = await this.IO.Post(`user/parts`, data, true);
            if (response.status == 200) {
                console.log("success");
                await this.GetMyParts();
            }
        }
        catch { }
    }

    /** Adds the selected part to the user's collection. */
    async AddPart() {
        let qty = (<HTMLInputElement>document.getElementById("addPart-qty")).value;
        let userpart = [{
            "part": { "id": currentPart.id }, "colorQuantities": [{ "color": { "id": 1 }, "quantity": qty }]
        }]
        this.HideCurrentPage();
        this.ShowLoading();
        try {
            await this.AddToMyParts(userpart);
            this.HideLoading();
            history.back();
            this.ShowCurrentPage();
            return;
        }
        catch { }
        this.GoTo(pages.Error);
    }

    /** Gets a list of sets that the user can build. */
    async GetSetsCanBuild() {
        try {
            let response = await this.IO.Get(`user/buildsets`, true);
            if (response.status == 200) {
                buildSets = await response.json();
                loadedBuildSets = true;
            }
        }
        catch (ex) {
            console.log(ex);
        }
    }

    /** Displays the sets that the user can build. */
    ShowSetsCanBuild() {
        const container = document.getElementById("sets-container")!;
        const template = <HTMLTemplateElement>document.getElementById("setTemplate");
        const messageElement = <HTMLElement>document.getElementById("sets-message");
        container.innerHTML = "";

        if (buildSets.length == 0) {
            messageElement.style.display = "";
            loadedBuildSets = false;
        }
        else {
            messageElement.style.display = "none";
        }

        for (let set of buildSets) {
            let element = document.importNode(template.content.querySelector("li")!, true);
            let idElement = <HTMLElement>element.getElementsByClassName("set-id").item(0);
            let nameElement = <HTMLElement>element.getElementsByClassName("set-name").item(0);
            let numberElement = <HTMLElement>element.getElementsByClassName("set-number").item(0);
            let numPartsElement = <HTMLElement>element.getElementsByClassName("set-numParts").item(0);
            let percentElement = <HTMLElement>element.getElementsByClassName("set-percent").item(0);

            idElement.textContent = set.id;
            nameElement.textContent = set.name;
            numberElement.textContent = set.setNum;
            numPartsElement.textContent = set.numParts;
            percentElement.textContent = (<number>set.percentage).toFixed(0) + "%";

            container.appendChild(element);
        }
    }
}

/** State of the page remembered by the browser. */
class PageState {
    Page: pages;
    Data: any;
    BackStates = 0;

    constructor(page: pages, data: any = null) {
        this.Page = page;
        this.Data = data;
    }
}

/** Information about a page. */
class PageDetail {
    Title: string;
    Url: string | null;
    RedirectTo: pages | null;
    ToolbarBtns: toolbarbtn[] | null;

    constructor(title: string, url: string | null, redirectTo: pages | null, toolbarbtns: toolbarbtn[] | null) {
        this.Title = title;
        this.Url = url;
        this.RedirectTo = redirectTo;
        this.ToolbarBtns = toolbarbtns;
    }
}
