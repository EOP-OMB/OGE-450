import { environment } from '../../environments/environment'

export class Helper {
    static addDays(date: Date, days: number): Date {
        date.setDate(date.getDate() + days);

        return date;
    }

    static daysBetween = function (date1, date2) {
        //Get 1 day in milliseconds
        var one_day = 1000 * 60 * 60 * 24;

        // Convert both dates to milliseconds
        var date1_ms = date1.getTime();
        var date2_ms = date2.getTime();

        // Calculate the difference in milliseconds
        var difference_ms = date2_ms - date1_ms;

        // Convert back to days and return
        return Math.ceil(difference_ms / one_day);
    }

    static formatDate(date: Date): string {
        var dateString: string = null;

        if (date) {
            dateString = Helper.format(date.getMonth() + 1, "00") + '/' + Helper.format(date.getDate(), "00") + '/' + date.getFullYear();
        }

        return dateString;
    }

    static getDate(value: string, useToday: boolean = false): Date {
        return value ? new Date(value) : useToday ? new Date() : null;
    }

    static format(value: number, format: string) {
        var s = format + value;

        s = s.substring(s.length - format.length);

        return s;
    }
}
