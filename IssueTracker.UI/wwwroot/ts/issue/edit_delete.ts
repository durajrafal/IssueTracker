import { renderAlert } from "../common/components.js";

class DeleteIssueModal {
    private deleteButton: HTMLButtonElement;
    private id: string;
    private projectId: string;

    constructor() {
        this.deleteButton = document.querySelector('[data-confirm-delete]');
        this.id = this.deleteButton.dataset.id;
        this.projectId = this.deleteButton.dataset.projectid;
        this.deleteButton.addEventListener('click', () => this.deleteIssue());
    }

    deleteIssue() {
        const baseUrl = window.location.origin;
        const url = baseUrl + '/api/projects/' + this.projectId + '/issues/' + this.id;
        fetch(url, {
            method: 'DELETE',
            headers: {
                "RequestVerificationToken": (<HTMLInputElement>document.querySelector('input[name="__RequestVerificationToken"]')).value as string
            },
        })
        .then(res => {
            if (res.ok)
                window.location.href = baseUrl + '/projects/' + this.projectId;
            else
                res.text().then(t => {
                    renderAlert('[data-error-message]', t);
                });
        })
        .catch();
    }
}

new DeleteIssueModal;