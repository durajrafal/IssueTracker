class DeleteProjectModal {
    private enteredTitleField: HTMLInputElement
    constructor() {
        const deleteButtons = document.querySelectorAll('[data-show-delete-modal]') as NodeListOf<HTMLButtonElement>;
        deleteButtons.forEach(button => {
            button.addEventListener('click', () => { this.setDeleteModal(button.dataset.title, button.dataset.id); });
        });
        this.enteredTitleField = document.querySelector('[data-delete-project-title]') as HTMLInputElement;
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
        const enteredTitle = this.enteredTitleField.value;
        if (title === enteredTitle) {
            const baseUrl = window.location.origin;
            const url = baseUrl + '/api/project-management/' + id + '/' + enteredTitle;
            fetch(url, {
                method: 'DELETE',
                headers: {
                    "RequestVerificationToken": (<HTMLInputElement>document.querySelector('input[name="__RequestVerificationToken"]')).value as string
                },
            })
                .then(res => {
                    if (res.ok)
                        window.location.href = baseUrl + '/project-management/';
                    else
                        res.text().then(t => {
                            const validationFeedback = document.querySelector('[data-delete-project-feedback]');
                            validationFeedback.textContent = t.substring(1, t.length - 1);
                            this.AddKeyStrokeInputValidation(title);
                        })
                })
                .catch();
        }
        else {
            this.AddKeyStrokeInputValidation(title);
        }
    }

    private AddKeyStrokeInputValidation(title: string) {
        this.enteredTitleField.classList.add('is-invalid');
        this.enteredTitleField.addEventListener('keyup', () => {
            if (title === this.enteredTitleField.value) {
                this.enteredTitleField.classList.replace('is-invalid', 'is-valid');
            }
            else {
                this.enteredTitleField.classList.replace('is-valid', 'is-invalid');
            }
        });
    }
}

new DeleteProjectModal();