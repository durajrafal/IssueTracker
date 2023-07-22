import addTableFilter from "../common/filter-table.js";

class GetPartialViewWithUserClaims {
    private rows: NodeListOf<HTMLTableRowElement>;
    private baseUrl: string;
    private previouslySelectedId: string;
    private previouslySelectedTableClass: string;

    constructor(url: string) {
        this.rows = document.querySelectorAll('tr');
        this.baseUrl = url;
        this.rows.forEach(node => node.addEventListener('click', () => this.GetUserClaims(node.dataset.id)));
        addTableFilter(this.rows, 'searchInput');
    }

    private GetUserClaims(id: string) {
        const url = this.baseUrl + id;
        fetch(url)
            .then(res => res.text())
            .then(data => this.RenderPartialView(data))
            .then(() => this.ChangeSelectedRowInTable(id));
    }

    private RenderPartialView(html: string) {
        document.querySelector('[data-partialView]').innerHTML = html;
        (<HTMLElement>document.querySelector('[data-placeholder]')).style.display = "none";
    }

    private ChangeSelectedRowInTable(id: string) {
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