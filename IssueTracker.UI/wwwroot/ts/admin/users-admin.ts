import { addTableFilter } from "../common/filter-table.js";

class GetPartialViewWithUserClaims {
    private readonly rows: NodeListOf<HTMLTableRowElement>;
    private previouslySelectedId: string;
    private previouslySelectedTableClass: string;

    constructor(private readonly baseUrl: string) {
        this.rows = document.querySelectorAll('tr');
        this.rows.forEach(row => row.addEventListener('click', () => this.getUserClaims(row.dataset.id)));
        addTableFilter(this.rows, 'searchInput');
    }

    private getUserClaims(id: string) {
        const url = this.baseUrl + id;
        fetch(url)
            .then(res => res.text())
            .then(data => this.renderPartialView(data))
            .then(() => this.changeSelectedRowInTable(id));
    }

    private renderPartialView(html: string) {
        document.querySelector('[data-partialView]').innerHTML = html;
        (<HTMLElement>document.querySelector('[data-placeholder]')).style.display = "none";
    }

    private changeSelectedRowInTable(id: string) {
        if (this.previouslySelectedId != undefined) {
            const previous = document.querySelector(`[data-id="${this.previouslySelectedId}"]`);
            previous.classList.replace('table-success', this.previouslySelectedTableClass);
        }
        const selected = document.querySelector(`[data-id="${id}"]`);
        this.previouslySelectedTableClass = selected.classList.value;
        selected.classList.replace(this.previouslySelectedTableClass, 'table-success');
        this.previouslySelectedId = id;
    }
}

new GetPartialViewWithUserClaims("/Identity/Admin/GetUserClaims/");