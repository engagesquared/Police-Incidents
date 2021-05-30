export function formatDateTime(locale: string, date: Date | string | undefined): string {
    if (typeof date === typeof "") {
        const dateObj = new Date(date as string);
        if (!isNaN(dateObj.valueOf())) {
            return formatDateTimeInternal(locale, dateObj);
        }
    } else if (!!date) {
        return formatDateTimeInternal(locale, date as Date);
    }
    return "";
}

function formatDateTimeInternal(locale: string, date: Date): string {
    return `${date.toLocaleDateString(locale, { year: "numeric", month: "numeric", day: "numeric" })} ${date
        .toLocaleTimeString(locale, {
            hour: "2-digit",
            minute: "2-digit",
        })
        .toLocaleUpperCase()}`;
}
