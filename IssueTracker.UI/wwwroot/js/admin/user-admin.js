let previouslySelectedId, previouslySelectedTableClass;
let rows = document.getElementsByName("table-row");

function GetUserClaims(input) {
    $.ajax({
        type: 'GET',
        url: getClaimsURL,
        data: {
            Id: input
        },
        dataType: 'html',
        success: function (result) {
            $('#partialViewWithClaims').html(result);
            document.getElementById('partialViewPlaceholder').style.display = "none";
            if (previouslySelectedId != undefined) {
                let previous = document.getElementById(previouslySelectedId);
                previous.classList.replace('table-success', previouslySelectedTableClass);
            }
            let selected = document.getElementById(input);
            previouslySelectedTableClass = selected.classList.value;
            selected.classList.replace(previouslySelectedTableClass, 'table-success');
            previouslySelectedId = input;
        }
    })
}

rows.forEach(
    function (node) {
        node.addEventListener('click', () => {
            GetUserClaims(node.dataset.id)
        });
    }
);

import { addTableFilter } from "../modules/filter-table.js";
addTableFilter(rows, "searchInput");