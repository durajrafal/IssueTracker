import { TableFilter } from "../common/filter-table.js";

class GetPartialViewWithUserClaims {
    private previouslySelectedId: string;
    private previouslySelectedTableClass: string;

    constructor(private readonly baseUrl: string) {
        const rows = document.querySelectorAll('tr');
        rows.forEach(row => row.addEventListener('click', () => this.getUserClaims(row.dataset.id)));
        TableFilter.create('[data-users-table]', '[data-search-input]');
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