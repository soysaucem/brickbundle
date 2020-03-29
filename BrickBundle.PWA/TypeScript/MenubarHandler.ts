export enum menubarbtn {
    Home,
    Bricks,
    Sets
}

const menubarbtnHome = document.getElementById("menubarbtnHome")!;
const menubarbtnBricks = document.getElementById("menubarbtnBricks")!;
const menubarbtnSets = document.getElementById("menubarbtnSets")!;

export class MenubarHandler {
    readonly menubarbtns: Record<menubarbtn, HTMLElement> = {
        [menubarbtn.Home]: menubarbtnHome,
        [menubarbtn.Bricks]: menubarbtnBricks,
        [menubarbtn.Sets]: menubarbtnSets,
    }

    selected: menubarbtn | null = null;

    Show() {
        document.getElementById("menubar")!.style.display = "";
    }

    Hide() {
        document.getElementById("menubar")!.style.display = "none";
    }

    /**
     * Changes color of selected button.
     * @param btn Selected button
     */
    SelectBtn(btn: menubarbtn) {
        if (this.selected != null) {
            this.menubarbtns[this.selected].classList.remove("color-purple");
        }
        this.selected = btn;
        this.menubarbtns[btn].classList.add("color-purple");
    }
}
