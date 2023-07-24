export function addTableFilter(tableRows: NodeListOf<HTMLTableRowElement>, searchFieldId: string) {
    const search = document.getElementById(searchFieldId) as HTMLInputElement;
    search.addEventListener('keyup', () => filterFunction(tableRows, search));
}

function filterFunction(rows: NodeListOf<HTMLTableRowElement>, searchField: HTMLInputElement) {
    const filter = searchField.value.toUpperCase();
    rows.forEach(
        function (row) {
            let match = false;
            row.childNodes.forEach(
                function (childNode) {
                    const containsNotOnlyNumbers = function (str: string) {
                        return !/^\d+$/.test(str);
                    };

                    if (childNode.nodeType == Node.ELEMENT_NODE) {
                        const childText = childNode.textContent.toLocaleUpperCase();
                        if (containsNotOnlyNumbers(childText) && childText.includes(filter)) {
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
    )
}