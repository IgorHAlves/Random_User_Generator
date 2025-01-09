let paginaAtual = 0;
const itensPorPagina = 10;
const baseEndPoint = "https://localhost:44380/v1/users";

async function fetchUsers(page = 0, searchQuery = "") {
    const skip = page * itensPorPagina;
    const endPoint = searchQuery
        ? `${baseEndPoint}/skip/${skip}/take/${itensPorPagina}?name=${encodeURIComponent(searchQuery)}`
        : `${baseEndPoint}/skip/${skip}/take/${itensPorPagina}`;

    try {
        const response = await fetch(endPoint);

        if (!response.ok) {
            throw new Error(`Erro: ${response.status}`);
        }

        const result = await response.json();
        const users = result.retornoUsers.data;

        const tbody = document.getElementById("userTableBody");
        if (!tbody) {
            console.error("Elemento tbody não encontrado.");
            return;
        }

        tbody.innerHTML = "";

        users.forEach(user => {
            const row = document.createElement("tr");

            row.innerHTML = `
            <td>${user.id}</td>
            <td>${user.gender}</td>
            <td>${user.name}</td>
            <td>${user.email}</td>
             <td class="center-button">
        <button class="btn btn-warning btn-sm edit-btn" 
            onclick="editUser(${user.id})"
            data-bs-toggle="modal" 
            data-bs-target="#Modal_Edit"
            data-user-id="${user.id}"> <!-- Adicionando o atributo data-user-id -->
            <i class="fa fa-edit"></i> Editar
        </button>
    </td>
        `;
            tbody.appendChild(row);
        });

        updatePaginationControls(page, result.retornoUsers.total);
    } catch (error) {
        console.error("Erro ao buscar usuários:", error);
    }
}

function updatePaginationControls(currentPage, totalItems) {
    const totalPages = Math.ceil(totalItems / itensPorPagina);
    document.getElementById("paginaAnterior").classList.toggle("disabled", currentPage <= 0);
    document.getElementById("proximaPagina").classList.toggle("disabled", currentPage >= totalPages - 1);
}

document.addEventListener("DOMContentLoaded", () => {
    const searchForm = document.getElementById("searchForm");
    const searchInput = document.getElementById("searchInput");

    fetchUsers();

    searchForm.addEventListener("submit", (event) => {
        event.preventDefault();
        const query = searchInput.value.trim();
        paginaAtual = 0;
        fetchUsers(paginaAtual, query);
    });

    document.getElementById("paginaAnterior").addEventListener("click", (event) => {
        event.preventDefault();
        if (paginaAtual > 0) {
            paginaAtual--;
            fetchUsers(paginaAtual, searchInput.value.trim());
        }
    });

    document.getElementById("proximaPagina").addEventListener("click", (event) => {
        event.preventDefault();
        paginaAtual++;
        fetchUsers(paginaAtual, searchInput.value.trim());
    });
});
