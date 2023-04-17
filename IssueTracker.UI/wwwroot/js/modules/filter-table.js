export function addTableFilter(tableRows, searchFieldId) {
    let search = document.getElementById(searchFieldId);
    search.addEventListener('keyup', () => filterFunction(tableRows, search));
}

function filterFunction(rows, searchField) {
    let filter = searchField.value.toUpperCase();
    rows.forEach(
        function (node) {
            let match = false;
            node.childNodes.forEach(
                function (childNode) {
                    let containsNotOnlyNumbers = function (str) {
                        return !/^\d+$/.test(str);
                    };

                    if (childNode.nodeType == Node.ELEMENT_NODE) {
                        let childText = childNode.textContent.toLocaleUpperCase();
                        if (containsNotOnlyNumbers(childText) && childText.includes(filter)) {
                            match = true;
                        }
                    }
                }
            );
            if (match) {
                node.style.display = "table-row";
            }
            else {
                node.style.display = "none";
            }
        }
    )
}