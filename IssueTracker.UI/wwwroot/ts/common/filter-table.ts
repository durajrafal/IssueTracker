export class TableFilter {
    private constructor(private readonly table: HTMLTableElement, private readonly filterInput: HTMLInputElement) {
        filterInput.addEventListener('keyup', () => this.filterFuntion());
    }

    static create(tableSelector: string, inputSelector: string) {
        const table = document.querySelector(tableSelector);
        const filterInput = document.querySelector(inputSelector);
        if (!(table instanceof HTMLTableElement))
            throw Error("Table not found.");
        else if (!(filterInput instanceof HTMLInputElement))
            throw Error("Filter input not found.");
        else {
            return new this(table, filterInput);
        }
    }

    private filterFuntion() {
        const rows = this.table.rows;
        for (let i = 1; i < rows.length; i++) {
            const row = rows.item(i);
            this.filterRow(row, this.filterInput.value);
        }
        console.log(this.table.tBodies);
    }

    private filterRow(row: HTMLTableRowElement, filter: string) {
        let match = false;
        row.childNodes.forEach(childNode => {
            const containsNotOnlyNumbers = function (str: string) {
                return !/^\d+$/.test(str);
            };

            if (childNode.nodeType == Node.ELEMENT_NODE) {
                const childText = childNode.textContent.toLocaleUpperCase();
                if (containsNotOnlyNumbers(childText) && childText.includes(filter.toLocaleUpperCase())) {
                    match = true;
                }
            }
        }
        );
        if (match) {
            row.style.display = "table-row";
        }
        else {
            row.style.display = "none";
        }
    }
}