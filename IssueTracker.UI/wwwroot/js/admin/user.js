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

function filterFunction() {
    let input = document.getElementById("searchInput");
    let filter = input.value.toUpperCase();
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