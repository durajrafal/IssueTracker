export interface MembersManagement {
    id: number;
    members: Array<Member>;
    otherUsers: Array<User>;
}

export interface Member {
    user: User;
}

export interface User {
    userId: string;
    email: string;
    firstName: string;
    lastName: string;
}

export class MembersManagementTablesHandler {
    private readonly deleteButtonHtml = `<button type="button" class="btn btn-danger" v-on:click="DeleteMember(member)">
                                            <i class="bi bi-person-dash-fill"></i>
                                        </button>`;

    private readonly addButtonHtml = `<button type="button" class="btn btn-success" v-on:click="AddUserToProject(user)">
                                        <i class="bi bi-person-plus-fill"></i>
                                    </button>`;

    private readonly membersTable: HTMLTableElement;
    private readonly otherUsersTable: HTMLTableElement;

    constructor(memberTableBodySelector: string, otherUserTableBodySelector: string, private userLists: MembersManagement) {
        this.membersTable = document.querySelector(memberTableBodySelector);
        this.otherUsersTable = document.querySelector(otherUserTableBodySelector);
    }

    populateUsersTables() {
        this.populateTableWithUsers(this.membersTable, this.userLists.members.map(m => m.user), this.deleteButtonHtml, this.deleteMember.bind(this));
        this.populateTableWithUsers(this.otherUsersTable, this.userLists.otherUsers, this.addButtonHtml, this.addUserToProject.bind(this));
    }

    private populateTableWithUsers(table: HTMLTableElement, users: Array<User>, buttonHtml: string, buttonCallbackFunction: (user: User, row: HTMLTableRowElement) => void) {
        users.forEach(user => {
            this.insertTableRowWithUser(table, user, buttonHtml, buttonCallbackFunction);
        });
    }

    private insertTableRowWithUser(table: HTMLTableElement, user: User, buttonHtml: string, buttonCallbackFunction: (user: User, row: HTMLTableRowElement) => void) {
        const row = table.insertRow();
        row.style.verticalAlign = "middle";
        row.insertCell().textContent = user.email;
        row.insertCell().textContent = user.firstName;
        row.insertCell().textContent = user.lastName;
        this.appendActionButton(buttonHtml, user, row, buttonCallbackFunction);
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
        if (this.userLists.members.length > 1) {
            this.userLists.otherUsers.push(deletedUser);
            const index = this.userLists.members.findIndex(x => x.user === user);
            this.userLists.members.splice(index, 1);
            this.membersTable.deleteRow(row.rowIndex - 1);
            row.deleteCell(row.cells.length - 1);
            this.appendActionButton(this.addButtonHtml, user, row, this.addUserToProject.bind(this));
            this.otherUsersTable.appendChild(row);
        }
    }

    private addUserToProject(user: User, row: HTMLTableRowElement) {
        const newMember = {
            user: user
        };
        this.userLists.members.push(newMember);
        const index = this.userLists.otherUsers.indexOf(user);
        this.userLists.otherUsers.splice(index, 1);
        this.otherUsersTable.deleteRow(row.rowIndex - 1);
        row.deleteCell(row.cells.length - 1);
        this.appendActionButton(this.deleteButtonHtml, user, row, this.deleteMember.bind(this));
        this.membersTable.appendChild(row);
    }
}