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
        if (date) {
            var dt = new Date(date);

            return Helper.format(dt.getMonth() + 1, "00") + '/' + Helper.format(dt.getDate(), "00") + '/' + dt.getFullYear();
        }
        else
            return null;
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

export class Guid {
    static newGuid() {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }
}