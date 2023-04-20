const deleteButtons = document.getElementsByName("showDeleteModalButton");
deleteButtons.forEach(
    function (button) {
        button.addEventListener('click', () => {
            SetDeleteModal(button.dataset.title, button.dataset.id)
        });
    }
)

const SetDeleteModal = function (title, id) {
    SetDeleteModalTitle(title);
    document.getElementById('confirmDeleteButton').addEventListener('click', () => {
        DeleteProject(title, id);
    })
}

const SetDeleteModalTitle = function (title) {
    let headerHTML = 'Delete Project - ';
    headerHTML = headerHTML + '<span class="text-warning fw-bold fs-5">' + title + '</span>';
    document.getElementById("deleteProjectModalLabel").innerHTML = headerHTML;
}


const DeleteProject = function(title,id) {
    const enteredTitleField = document.getElementById("deleteProjectTitle");
    let enteredTitle = enteredTitleField.value;
    if (title === enteredTitle) {
        const baseUrl = window.location.origin;
        const url = baseUrl + '/api/project-management/' + id + '/' + title;
        console.log(url)
        axios({
                url: url,
                method: 'delete',
                headers: {
                    "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
                },
            })
            .then(res => {
                window.location.href = baseUrl + '/project-management/'
            })
            .catch(err => {
                console.log(err)
            })
    }
    else {
        enteredTitleField.classList.add("is-invalid");
        enteredTitleField.addEventListener('keyup', (field) => {
            console.log(enteredTitleField.value);
            console.log(title);
            if (title === enteredTitleField.value) {
                enteredTitleField.classList.replace("is-invalid","is-valid");
            }
        })
    }

}
