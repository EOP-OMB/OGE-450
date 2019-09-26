export class GridPersistence {
    public gridOptions: GridOptions;
    public key: string;

    constructor() {
        this.gridOptions = new GridOptions();
    }

    public save(): void {
        sessionStorage.setItem(this.key, JSON.stringify(this.gridOptions));
    }

    public load(key: string): void {
        const opt = sessionStorage.getItem(key);

        if (opt) {
            this.gridOptions = JSON.parse(opt);
            this.key = key;
        }
    }

    public setGridControl(key: string, value: string, controlId: string = "") {
        var option = { key: key, value: value, id: controlId };

        var exists = this.gridOptions.controls.filter(x => x.key == key);

        // if key already exists, remove it and push new option
        if (exists)
            this.gridOptions.controls = this.gridOptions.controls.filter(x => x.key != key);

        this.gridOptions.controls.push(option);
    }

    public setGridFilters(filters: any): void {
        this.gridOptions.filters = filters;

        this.save();
    }

    public setPage(rows: number, first: number): void {
        this.gridOptions.rows = rows;
        this.gridOptions.first = first;

        this.save();
    }

    public setSort(field: string, order: number): void {
        this.gridOptions.sortField = field;
        this.gridOptions.sortOrder = order;

        this.save();
    }

    public resetControls(): void {
        this.gridOptions.controls = [];
    }
}

export class GridOptions {
    public first: number;
    public rows: number;
    public sortField: string;
    public sortOrder: number;
    public filters: any;
    public controls: any[];

    constructor() {
        this.first = 0;
        this.rows = 10;
        this.sortField = 'title';
        this.sortOrder = 1;
        this.filters = null;
        this.controls = [];
    }
}
