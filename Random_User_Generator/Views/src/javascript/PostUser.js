document.addEventListener("DOMContentLoaded", () => {
    const userForm = document.getElementById("userForm");

    if (!userForm) {
        console.error("O formulário #userForm não foi encontrado no DOM.");
        return;
    }

    userForm.addEventListener("submit", async (event) => {
        event.preventDefault();

        const gender = document.querySelector("input[name='Gender']").value.trim();
        const name = document.querySelector("input[name='Name']").value.trim();
        const email = document.querySelector("input[name='Email:']").value.trim();

        const dados = {
            gender,
            name,
            email,
        };

        const endpoint = "https://localhost:44380/v1/users";

        try {
            const response = await fetch(endpoint, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(dados),
            });

            if (!response.ok) {
                throw new Error(`Erro ao cadastrar: ${response.statusText}`);
            }

            const result = await response.json();
            alert("Usuário cadastrado com sucesso!");
            console.log("Resposta do servidor:", result);

            // Fechar modal
            const modal = bootstrap.Modal.getInstance(document.getElementById("Modal_Post"));
            modal.hide();

            // Limpar campos
            userForm.reset();
        } catch (error) {
            console.error("Erro:", error);
            alert("Não foi possível cadastrar o usuário.");
        }
    });
});
