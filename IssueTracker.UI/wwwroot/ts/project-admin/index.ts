class DeleteProjectModal {

    constructor() {
        const deleteButtons = document.querySelectorAll('[data-show-delete-modal]') as NodeListOf<HTMLButtonElement>;
        deleteButtons.forEach(button => {
            button.addEventListener('click', () => { this.setDeleteModal(button.dataset.title, button.dataset.id); });
        });
    }

    private setDeleteModal(title: string, id: string) {
        this.setDeleteModalTitle(title);
        document.querySelector('[data-confirm-delete]')
            .addEventListener('click', () => { this.deleteProject(title, id); });
    }

    private setDeleteModalTitle(title: string) {
        let headerHTML = 'Delete Project - ';
        headerHTML = headerHTML + '<span class="text-warning fw-bold fs-5">' + title + '</span>';
        document.querySelector('[data-delete-project]').innerHTML = headerHTML;
    }

    private deleteProject(title: string, id: string) {
        const enteredTitleField = document.querySelector('[data-delete-project-title]') as HTMLInputElement;
        const enteredTitle = enteredTitleField.value;
        if (title === enteredTitle) {
            const baseUrl = window.location.origin;
            const url = baseUrl + '/api/project-management/' + id + '/' + title;
            fetch(url, {
                method: 'DELETE',
                headers: {
                    "RequestVerificationToken": (<HTMLInputElement>document.querySelector('input[name="__RequestVerificationToken"]')).value as string
                },
            })
                .then(() => {
                    window.location.href = baseUrl + '/project-management/';
                });
        }
        else {
            enteredTitleField.classList.add('is-invalid');
            enteredTitleField.addEventListener('keyup', () => {
                if (title === enteredTitleField.value) {
                    enteredTitleField.classList.replace('is-invalid', 'is-valid');
                }
                else {
                    enteredTitleField.classList.replace('is-valid', 'is-invalid');
                }
            });
        }
    }
}

new DeleteProjectModal();