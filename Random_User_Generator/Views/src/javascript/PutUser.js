let currentUserId = null;

async function fetchUserById(userId) {
    try {
        const response = await fetch(`${baseEndPoint}/${userId}`);

        if (!response.ok) {
            throw new Error('Erro ao buscar usuário.');
        }

        const result = await response.json();

        const user = result.data;

        if (user && user.gender) {
            return user;
        } else {
            throw new Error('Usuário não encontrado ou dados inválidos.');
        }
    } catch (error) {
        console.error("Erro ao carregar usuário para edição:", error);
    }
}

async function editUser(userId) {
    const user = await fetchUserById(userId);

    if (user) {
        document.getElementById("gender").value = user.gender;
        document.getElementById("name").value = user.name;
        document.getElementById("email").value = user.email;

        document.getElementById("putUser").dataset.userId = user.id;
    } else {
        console.error("Usuário não encontrado para edição");
    }
}

document.getElementById("putUser").addEventListener("click", async function () {
    const userId = this.dataset.userId;
    const gender = document.getElementById("gender").value;
    const name = document.getElementById("name").value;
    const email = document.getElementById("email").value;

    console.log("Dados para atualização:");
    console.log("ID:", userId);
    console.log("Gender:", gender);
    console.log("Name:", name);
    console.log("Email:", email);

    if (!userId) {
        console.error("ID do usuário não encontrado");
        return;
    }

    try {
        const response = await fetch(`${baseEndPoint}/${userId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                gender: gender,
                name: name,
                email: email
            })
        });

        if (!response.ok) {
            throw new Error('Erro ao atualizar usuário.');
        }

        const result = await response.json();
        console.log("Resultado da atualização:", result);

        alert("Usuário atualizado com sucesso!");
        fetchUsers(paginaAtual, document.getElementById("searchInput").value.trim());
        $('#Modal_Edit').modal('hide');
    } catch (error) {
        console.error("Erro ao atualizar usuário:", error);
    }
});

