document.addEventListener("DOMContentLoaded", () => {
    const postRandomUserButton = document.getElementById("postRandomUser");

    postRandomUserButton.addEventListener("click", async () => {
        const endPoint = "https://localhost:44380/random";

        try {
            const response = await fetch(endPoint, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
            });

            if (!response.ok) {
                throw new Error(`Erro: ${response.status} - ${response.statusText}`);
            }

            const result = await response.json();
            alert("Usuário random criado com sucesso!");
            console.log("Resposta do servidor:", result);

            fetchUsers();
        } catch (error) {
            console.error("Erro ao criar usuário random:", error);
            alert("Falha ao criar usuário random.");
        }
    });
});
