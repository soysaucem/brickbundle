/**
 * Appends error message to a html element.
 * @param textContainer HTML element for containing error massages
 * @param text Error message
 */
export function AppendError(textContainer: HTMLElement, text: string): void {
    let template = <HTMLTemplateElement>document.getElementById("formErrorTemplate");
    let element = document.importNode(template.content.querySelector("div")!, true);
    element.textContent = text;
    textContainer.appendChild(element);
    textContainer.style.display = "";
}

/**
 * Appends success message to a html element.
 * @param textContainer HTML element for containing success message
 * @param text Success message
 */
export function AppendSuccess(textContainer: HTMLElement, text: string): void {
    let template = <HTMLTemplateElement>document.getElementById("formSuccessTemplate");
    let element = document.importNode(template.content.querySelector("div")!, true);
    element.textContent = text;
    textContainer.appendChild(element);
    textContainer.style.display = "";
}

/**
 * Clears errors and success messages.
 * @param textContainer HTML element containing messages
 */
export function ResetErrors(textContainer: HTMLElement) {
    let errorHolders = textContainer.getElementsByClassName("validation-error");
    for (let errorHolder of errorHolders) {
        errorHolder.innerHTML = "";
        (<HTMLElement>errorHolder).style.display = "none";
    }
}