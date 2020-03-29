/**
 * Saves data to a cookie.
 * @param name Cookie name
 * @param value Cookie data
 * @param days Cookie expiration time
 */
export function SetCookie(name: string, value: string, days: number | null = null) {
    let expires = "";
    if (days) {
        let date = new Date();
        date.setTime(date.getTime() + days * 24 * 60 * 60 * 1000);
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}

/**
 * Gets data from cookie.
 * @param name Cookie name
 */
export function GetCookie(name: string) {
    let nameEq = name + "=";
    let CookieParts = document.cookie.split(";");
    for (let i = 0; i < CookieParts.length; ++i) {
        let cookiePart = CookieParts[i];
        while (cookiePart.charAt(0) === " ") cookiePart = cookiePart.substring(1, cookiePart.length);
        if (cookiePart.indexOf(nameEq) === 0) {
            return cookiePart.substring(nameEq.length, cookiePart.length);
        }
    }
    return null;
}

/**
 * Erases cookie.
 * @param name Cookie name
 */
export function EraseCookie(name: string) {
    document.cookie = name + "=; Max-Age=-99999999;";
}
