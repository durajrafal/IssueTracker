let app = Vue.createApp({
    data: function () {
        return {
            loading: true,
            project: null,
            memberFilter: '',
            userFilter: '',
            editTitle: false,
            newTitle: '',
            baseUrl: ''
        }
    },
    created() {
        this.baseUrl = window.location.origin;
    },
    beforeMount() {
        this.GetProjectDetails()

    },
    methods: {
        GetProjectDetails() {
            const id = location.href.split('/').pop();
            let url = this.baseUrl + '/api/project-management/' + id
            axios.get(url)
                .then(res => {
                    this.project = res.data;
                })
                .catch(err => {
                    console.log(err);
                })
                .then(() => {
                    this.loading = false;
                });
        },
        UpdateProject() {
            let url = this.baseUrl + '/api/project-management/' + this.project.id
            axios.put(url,
                {
                    ProjectId: this.project.id,
                    Title: this.project.title,
                    Members: this.project.members,
                },
                {
                    headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() }
                }
            )
                .catch(err => {
                    console.log(err);
                })
                .then(() => {
                    window.location.href = "../project-management";
                })
        },
        AddUserToProject(user) {
            let newMember = {
                projectId: this.project.id,
                userId: user.userId,
                user: user,
            }
            this.project.members.push(newMember)
            let index = this.project.otherUsers.indexOf(user)
            this.project.otherUsers.splice(index, 1)
        },
        DeleteMember(member) {
            let deletedUser = {
                userId: member.user.userId,
                email: member.user.email,
                firstName: member.user.firstName,
                lastName: member.user.lastName,
            }
            if (this.project.members.length > 1) {
                this.project.otherUsers.push(deletedUser)
                let index = this.project.members.indexOf(member)
                this.project.members.splice(index, 1)
            }
        },
        ToggleEditTitle() {
            this.editTitle = !this.editTitle;
            if (this.editTitle) {
                this.newTitle = this.project.title;
            }
        },
        AcceptNewTitle() {
            this.project.title = this.newTitle
            this.ToggleEditTitle();
        }
    },
    computed: {
        filteredMembers() {
            return this.project.members.filter(member => {
                const email = member.user.email;
                const firstName = member.user.firstName.toLowerCase();
                const lastName = member.user.lastName.toLowerCase();
                const searchVal = this.memberFilter.toLowerCase();

                return email.includes(searchVal) || firstName.includes(searchVal) || lastName.includes(searchVal)
            })
        },
        filteredUsers() {
            return this.project.otherUsers.filter(user => {
                const email = user.email.toUpperCase();
                const firstName = user.firstName.toUpperCase();
                const lastName = user.lastName.toUpperCase();
                const searchVal = this.userFilter.toUpperCase();

                return email.includes(searchVal) || firstName.includes(searchVal) || lastName.includes(searchVal)
            })
        }
    }
})

app.mount('#app')


