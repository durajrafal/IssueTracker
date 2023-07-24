import { Project } from "project-model.js";
import { MembersManagementTablesHandler } from "../common/member-management.js";

class ProjectAdminDetails {
    private project: Project;
    private memberManagement: MembersManagementTablesHandler;
    constructor(private readonly baseUrl: string) {
        this.getProjectDetails();
    }

    private getProjectDetails() {
        const id = location.href.split('/').pop();
        const url = this.baseUrl + '/api/project-management/' + id;
        fetch(url)
            .then(res => res.text())
            .then(data => { this.project = JSON.parse(data); })
            .then(() => { this.memberManagement = new MembersManagementTablesHandler('[data-members-table] > tbody', '[data-other-users-table] > tbody', this.project); })
            .then(() => { this.displayData(); });
    }

    private displayData() {
        const populateTitle = () => {
            (<HTMLElement>document.querySelector('[data-project-title]')).textContent = this.project.title;
        };

        const populateViewWithData = () => {
            populateTitle();
            this.memberManagement.populateUsersTables();
        };

        const hideLoading = () => {
            document.querySelector('[data-loading]').classList.add('d-none');
        };

        const showData = () => {
            document.querySelector('[data-loaded]').classList.remove('d-none');
        };

        populateViewWithData();
        hideLoading();
        showData();
    }
}

new ProjectAdminDetails(window.location.origin);