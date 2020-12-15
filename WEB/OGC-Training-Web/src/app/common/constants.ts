export class RecipientTypes {
    public static USER: string = "User";
    public static GROUP: string = "Group";
}

export class Divisions {
    public static OA: string = "OA";
    public static OGC: string = "OGC";
    public static OTHER: string = "Other";
}

import { SelectItem } from 'primeng/primeng';

export class Lookups {
    public static YEARS: SelectItem[];
    public static RECIPIENT_TYPES: SelectItem[];
    public static DIVISIONS: SelectItem[];
    public static TRAINING_TYPES: SelectItem[];

    public static initialize() {
        Lookups.YEARS = [];

        var year = new Date().getFullYear();

        Lookups.YEARS.push({ label: 'All', value: null });

        for (var i = year; i >= year-5; i--) {
            Lookups.YEARS.push({ label: i.toString(), value: i.toString() });
        }
   
        Lookups.RECIPIENT_TYPES = [];
        Lookups.RECIPIENT_TYPES.push({ label: RecipientTypes.USER, value: RecipientTypes.USER });
        Lookups.RECIPIENT_TYPES.push({ label: RecipientTypes.GROUP, value: RecipientTypes.GROUP });

        Lookups.DIVISIONS = [];
        Lookups.DIVISIONS.push({ label: '', value: '' });
        Lookups.DIVISIONS.push({ label: Divisions.OA, value: Divisions.OA });
        Lookups.DIVISIONS.push({ label: Divisions.OGC, value: Divisions.OGC });
        Lookups.DIVISIONS.push({ label: Divisions.OTHER, value: Divisions.OTHER });

        Lookups.TRAINING_TYPES = [];
        Lookups.TRAINING_TYPES.push({ label: '', value: '' });
        Lookups.TRAINING_TYPES.push({ label: 'Initial', value: 'Initial' });
        Lookups.TRAINING_TYPES.push({ label: 'Annual', value: 'Annual' });
    }
}
Lookups.initialize();