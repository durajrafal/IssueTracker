import { renderAlert } from "../common/components.js";
import { TableFilter } from "../common/filter-table.js";
import { Member, MembersManagement, MembersManagementTablesHandler, User } from "../common/member-management.js";

class Issue implements MembersManagement {
    id: number;
    members: Member[];
    otherUsers: User[];
}

class IssueMembersManagementDetails {
    private issue: Issue;
    private issueId: string;
    private projectId: string;
    private memberManagement: MembersManagementTablesHandler;

    constructor(private readonly baseUrl: string) {
        this.getProjectIdAndIssueId();
        this.getIssueMembersManagementDetails();
        TableFilter.create('[data-members-table]', '[data-members-filter-input]');
        TableFilter.create('[data-other-users-table]', '[data-other-user-filter-input]');
        this.setupEventListeners();
    }

    private getProjectIdAndIssueId() {
        const modalData = <HTMLElement>document.querySelector('[data-open-modal]');
        this.projectId = modalData.dataset.projectid;
        this.issueId = modalData.dataset.issueid;
    }

    private getIssueMembersManagementDetails() {
        const url = this.baseUrl + '/api/projects/' + this.projectId + '/issues/' + this.issueId + '/members';

        fetch(url)
            .then(res => {
                if (res.status == 200) {
                    return res.text();
                }
                if (res.status == 404) {
                    return res.text().then(t => { throw Error(t.substring(1, t.length - 1)); });
                }
                throw Error("Something went wrong. Please try again.");
            })
            .then(data => { this.issue = JSON.parse(data); })
            .then(() => { this.memberManagement = new MembersManagementTablesHandler('[data-members-table] > tbody', '[data-other-users-table] > tbody', this.issue, true); })
            .then(() => { this.memberManagement.populateUsersTables(); })
            .catch(err => this.displayError(err));
    }

    private updateIssueMembers() {
        const url = this.baseUrl + '/api/projects/' + this.projectId + '/issues/' + this.issueId + '/members';

        fetch(url, {
            method: 'PUT',
            headers: {
                "Content-Type": "application/json",
                "RequestVerificationToken": (<HTMLInputElement>document.querySelector('input[name="__RequestVerificationToken"]')).value as string
            },
            body: JSON.stringify(this.issue),
        })
            .then(res => {
                if (res.ok)
                    location.reload();
                else
                    return res.text().then(t => { throw Error(t.substring(1, t.length - 1)); });
            })
            .catch(err => this.displayError(err));
    }

    private setupEventListeners() {
        document.querySelector('[data-confirm-update]').addEventListener('click', () => this.updateIssueMembers());
    }

    private displayError(message: string) {
        renderAlert('[data-error-message]', message);
    }
}



class MembersModalBuilder {
    issueMembersManagement: IssueMembersManagementDetails;
    constructor() {
    }

    SetupMembersModalCreation() {
    const modalButton = document.querySelector('[data-open-modal]') as HTMLButtonElement;
        modalButton.addEventListener('click', () => {
            if (this.issueMembersManagement == null) {
                this.issueMembersManagement = new IssueMembersManagementDetails(window.location.origin);
            }
        });
}
}

new MembersModalBuilder().SetupMembersModalCreation();