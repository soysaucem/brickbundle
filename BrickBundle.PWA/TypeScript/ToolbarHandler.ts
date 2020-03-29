export enum toolbarbtn {
    Back,
    Account,
    AddParts,
    CaptureParts,
    Tick
}

const toolbarbtnBack = document.getElementById("toolbarbtnBack")!;
const toolbarbtnAccount = document.getElementById("toolbarbtnAccount")!;
const toolbarbtnAddParts = document.getElementById("toolbarbtnAddParts")!;
const toolbarbtnCaptureParts = document.getElementById("toolbarbtnCaptureParts")!;
const toolbarbtnTick = document.getElementById("toolbarbtnTick")!;

export class ToolbarHandler {
    readonly toolbarbtns: Record<toolbarbtn, HTMLElement> = {
        [toolbarbtn.Back]: toolbarbtnBack,
        [toolbarbtn.Account]: toolbarbtnAccount,
        [toolbarbtn.AddParts]: toolbarbtnAddParts,
        [toolbarbtn.CaptureParts]: toolbarbtnCaptureParts,
        [toolbarbtn.Tick]: toolbarbtnTick,
    }

    /**
     * Shows the specified buttons and hides all other buttons.
     * @param toolbarbtns Buttons to be displayed.
     */
    ShowBtns(toolbarbtns: toolbarbtn[] | null) {
        for (let item in toolbarbtn) {
            let btnNo = Number(item);
            if (!isNaN(btnNo)) {
                let btn = <toolbarbtn>btnNo;
                if (toolbarbtns != null && toolbarbtns.includes(btn)) {
                    this.ShowBtn(btn);
                }
                else {
                    this.HideBtn(btn);
                }
            }
        }
    }

    ShowBtn(btn: toolbarbtn) {
        this.toolbarbtns[btn].style.display = "";
    }

    HideBtn(btn: toolbarbtn) {
        this.toolbarbtns[btn].style.display = "none";
    }

    /** Sets the document title and the tile in the toolbar. */
    SetTitle(title: string) {
        document.title = `${title} - BrickBundle`;
        document.getElementById("pageTitle")!.innerText = title;
    }
}
