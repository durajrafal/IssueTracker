export interface MembersManagement {
    id: number;
    members: Array<Member>;
    otherUsers: Array<User>;
}

export interface Member {
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

    populateUsersTables() {
        this.populateTableWithUsers(this.membersTable, this.userLists.members.map(m => m.user), this.appendDeleteMemberButton.bind(this));
        this.populateTableWithUsers(this.otherUsersTable, this.userLists.otherUsers, this.appendAddMemberButton.bind(this));
    }

    private populateTableWithUsers(table: HTMLTableElement, users: Array<User>, appendButtonCallbackFunction: (user: User, row: HTMLTableRowElement) => void ) {
        users.forEach(user => {
            this.insertTableRowWithUser(table, user, appendButtonCallbackFunction);
        });
    }

    private insertTableRowWithUser(table: HTMLTableElement, user: User, appendButtonCallbackFunction: (user: User, row: HTMLTableRowElement) => void ){
        const row = table.insertRow();
        row.style.verticalAlign = "middle";
        row.insertCell().textContent = user.email;
        row.insertCell().textContent = user.firstName;
        row.insertCell().textContent = user.lastName;
        appendButtonCallbackFunction(user, row);
    }

    private appendDeleteMemberButton(user: User, row: HTMLTableRowElement) {
        this.appendActionButton(this.deleteMemberButtonHtml, user, row, this.deleteMember.bind(this));
    }

    private appendAddMemberButton(user: User, row: HTMLTableRowElement) {
        this.appendActionButton(this.addMemberButtonHtml, user, row, this.addMember.bind(this));
    }

    private appendActionButton(buttonHtml: string, user: User, row: HTMLTableRowElement, buttonCallbackFunction: (user: User, row: HTMLTableRowElement) => void) {
        row.insertCell().innerHTML = buttonHtml;
        const lastChild = (<HTMLTableCellElement>row.lastChild);
        lastChild.classList.add('text-center');
        lastChild.querySelector('button').addEventListener('click', () => buttonCallbackFunction(user, row));
    }

    private deleteMember(user: User, row: HTMLTableRowElement) {
        const deletedUser = {
            userId: user.userId,
            email: user.email,
            firstName: user.firstName,
            lastName: user.lastName,
        };
        if (this.userLists.members.length > 1 || this.canHaveNoMembers) {
            this.userLists.otherUsers.push(deletedUser);
            const index = this.userLists.members.findIndex(x => x.user === user);
            this.userLists.members.splice(index, 1);

            this.membersTable.deleteRow(row.rowIndex - 1);
            row.deleteCell(row.cells.length - 1);
            this.appendAddMemberButton(user, row);
            this.otherUsersTable.appendChild(row);
        }
    }

    private addMember(user: User, row: HTMLTableRowElement) {
        const newMember = {
            userId: user.userId,
            user: user
        };
        this.userLists.members.push(newMember);
        const index = this.userLists.otherUsers.indexOf(user);
        this.userLists.otherUsers.splice(index, 1);

        this.otherUsersTable.deleteRow(row.rowIndex - 1);
        row.deleteCell(row.cells.length - 1);
        this.appendDeleteMemberButton(user, row)
        this.membersTable.appendChild(row);
    }
}