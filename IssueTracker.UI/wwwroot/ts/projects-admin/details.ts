import { Project } from "project-model.js";
import { MembersManagementTablesHandler } from "../common/member-management.js";
import { TableFilter } from "../common/filter-table.js";
import { renderAlert } from "../common/components.js";

class ProjectAdminDetails {
    private project: Project;
    private memberManagement: MembersManagementTablesHandler;
    constructor(private readonly baseUrl: string) {
        this.getProjectDetails();
        document.addEventListener('DOMContentLoaded', () => this.setupEventListeners());
        TableFilter.create('[data-members-table]', '[data-members-filter-input]');
        TableFilter.create('[data-other-users-table]', '[data-other-user-filter-input]');
    }

    private getProjectDetails() {
        const id = location.href.split('/').pop();
        const url = this.baseUrl + '/api/project-management/' + id;

        fetch(url)
            .then(res => {
                if (res.status == 200) {
                    return res.text()
                }
                if (res.status == 404) {
                    return res.text().then(t => { throw Error(t.substring(1, t.length - 1)); })
                }
                throw Error("Something went wrong. Please try again.")
            })
            .then(data => { this.project = JSON.parse(data); })
            .then(() => { this.memberManagement = new MembersManagementTablesHandler('[data-members-table] > tbody', '[data-other-users-table] > tbody', this.project); })
            .then(() => { this.displayData(); })
            .catch(err => this.displayError(err));
    }

    private setupEventListeners() {
        document.querySelector('[data-update-project]').addEventListener('click', () => this.updateProject());
        document.querySelectorAll('[data-toogle-title-edit]').forEach(e => e.addEventListener('click', () => this.toogleTitleEdit()));
        document.querySelector('[data-accept-new-title]').addEventListener('click', () => this.acceptTitleEdit());
    }

    private updateProject() {
        const url = this.baseUrl + '/api/project-management/' + this.project.id;
        fetch(url, {
            method: 'PUT',
            headers: {
                "Content-Type": "application/json",
                "RequestVerificationToken": (<HTMLInputElement>document.querySelector('input[name="__RequestVerificationToken"]')).value as string
            },
            body: JSON.stringify(this.project),
        })
            .then(res => {
                if (res.ok)
                    window.history.back();
                else
                    return res.text().then(t => { throw Error(t.substring(1, t.length - 1)); });
            })
            .catch(err => this.displayError(err));
    }

    private displayData() {
        const populateViewWithData = () => {
            this.memberManagement.populateMembersTables();
        };

        const hideLoading = () => {
            document.querySelector('[data-loading]').classList.add('d-none');
        };

        const showData = () => {
            document.querySelector('[data-loaded]').classList.remove('d-none');
        };

        populateViewWithData();
        this.displayTitle();
        hideLoading();
        showData();
    }

    private displayTitle() {
        (<HTMLElement>document.querySelector('[data-project-title]')).textContent = this.project.title;
    }

    private displayError(message: string) {
        renderAlert('[data-error-message]', message);
    }

    private toogleTitleEdit() {
        const toogleDisplayClass = (element: HTMLElement) => {
            element.classList.toggle('d-flex');
            element.classList.toggle('d-none');
        };

        const showTitle = document.querySelector('[data-show-title]') as HTMLElement;
        toogleDisplayClass(showTitle);

        const editTitle = document.querySelector('[data-edit-title]') as HTMLElement;
        toogleDisplayClass(editTitle);
        editTitle.querySelector('input').value = this.project.title;
    }

    private acceptTitleEdit() {
        const newTitleValue = (<HTMLInputElement>document.querySelector('[data-edit-title] > input')).value;
        this.project.title = newTitleValue;
        this.displayTitle();
        this.toogleTitleEdit();
    }
}

new ProjectAdminDetails(window.location.origin);