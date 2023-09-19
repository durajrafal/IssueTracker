export interface MembersManagement {
    id: number;
    members: Array<Member>;
    otherUsers: Array<Member>;
}

export interface Member {
    id: number;
    userId: string;
    user: User;
}

export interface User {
    userId: string;
    email: string;
    firstName: string;
    lastName: string;
}

export class MembersManagementTablesHandler {
    private readonly deleteMemberButtonHtml = `<button type="button" class="btn btn-danger" v-on:click="DeleteMember(member)">
                                            <i class="bi bi-person-dash-fill"></i>
                                        </button>`;

    private readonly addMemberButtonHtml = `<button type="button" class="btn btn-success" v-on:click="AddUserToProject(user)">
                                        <i class="bi bi-person-plus-fill"></i>
                                    </button>`;

    private readonly membersTable: HTMLTableElement;
    private readonly otherUsersTable: HTMLTableElement;

    constructor(memberTableBodySelector: string, otherUserTableBodySelector: string, private userLists: MembersManagement, private canHaveNoMembers: boolean = false) {
        this.membersTable = document.querySelector(memberTableBodySelector);
        this.otherUsersTable = document.querySelector(otherUserTableBodySelector);
    }

    populateMembersTables() {
        this.populateTableWithMembers(this.membersTable, this.userLists.members, this.appendDeleteMemberButton.bind(this));
        this.populateTableWithMembers(this.otherUsersTable, this.userLists.otherUsers, this.appendAddMemberButton.bind(this));
    }

    private populateTableWithMembers(table: HTMLTableElement, members: Array<Member>, appendButtonCallbackFunction: (member: Member, row: HTMLTableRowElement) => void) {
        members.forEach(member => {
            this.insertTableRowWithMember(table, member, appendButtonCallbackFunction);
        });
    }

    private insertTableRowWithMember(table: HTMLTableElement, member: Member, appendButtonCallbackFunction: (member: Member, row: HTMLTableRowElement) => void ){
        const row = table.insertRow();
        row.style.verticalAlign = "middle";
        row.insertCell().textContent = member.user.email;
        row.insertCell().textContent = member.user.firstName;
        row.insertCell().textContent = member.user.lastName;
        appendButtonCallbackFunction(member, row);
    }

    private appendDeleteMemberButton(member: Member, row: HTMLTableRowElement) {
        this.appendActionButton(this.deleteMemberButtonHtml, member, row, this.deleteMember.bind(this));
    }

    private appendAddMemberButton(member: Member, row: HTMLTableRowElement) {
        this.appendActionButton(this.addMemberButtonHtml, member, row, this.addMember.bind(this));
    }

    private appendActionButton(buttonHtml: string, member: Member, row: HTMLTableRowElement, buttonCallbackFunction: (member: Member, row: HTMLTableRowElement) => void) {
        row.insertCell().innerHTML = buttonHtml;
        const lastChild = (<HTMLTableCellElement>row.lastChild);
        lastChild.classList.add('text-center');
        lastChild.querySelector('button').addEventListener('click', () => buttonCallbackFunction(member, row));
    }

    private deleteMember(member: Member, row: HTMLTableRowElement) {
        if (this.userLists.members.length > 1 || this.canHaveNoMembers) {
            this.userLists.otherUsers.push(member);
            const index = this.userLists.members.findIndex(x => x === member);
            this.userLists.members.splice(index, 1);

            this.membersTable.deleteRow(row.rowIndex - 1);
            row.deleteCell(row.cells.length - 1);
            this.appendAddMemberButton(member, row);
            this.otherUsersTable.appendChild(row);
        }
    }

    private addMember(member: Member, row: HTMLTableRowElement) {
        this.userLists.members.push(member);
        const index = this.userLists.otherUsers.indexOf(member);
        this.userLists.otherUsers.splice(index, 1);

        this.otherUsersTable.deleteRow(row.rowIndex - 1);
        row.deleteCell(row.cells.length - 1);
        this.appendDeleteMemberButton(member, row)
        this.membersTable.appendChild(row);
    }
}