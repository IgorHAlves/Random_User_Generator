# Randon User Generator
Este projeto FullStack no modelo MVC, onde no Back-end foi desenvolvido um banco de dados Postgres e uma API .NET para intermediar a comunicação entre o Front-end e o banco.
A API permite operações CRUD (Create, Read, Update, Delete) no banco criado e também se comunica com a api Randon User Generator (https://randomuser.me) para gerar Users de forma aleatória e fazer a persistência dos mesmos no banco de dados. Ressalto que no projeto também foi utilizado o Docker para facilitar a execução do Banco de Dados.

## Tecnologias Utilizadas
- Linguagem API: C#
- Framework: .NET | Entity Framework Core | AspNetCore
- Banco de Dados: PostgreSQL
- Contêinerização: Docker
-Funcionalidades
- Gerenciamento de Users: Adicionar, visualizar, editar e excluir informações de clientes.
- Inserção de Users Aleatórios através do consumo da API Randon User Generator.

## Inicialização
Certifique-se de ter o Docker instalado em sua máquina.
Executando o Projeto
-1. Clone o Repositório
-2. Faça o download da imagem Postgres no Docker e crie seu Container
Faça o download da imagem do Postgres no Docker e crie um container com o nome de usuario e senha conforme a conection string do DbContext.cs
-3. Execute o docker e atualize a database com as migrations
Inicie o container criado no Docker, no seu SGBD, crie um DataBase chamado Randon_User_Generator, e por fim, abra o terminal no projeto e atualize o database com o seguinte comando: dotnet ef migrations database update.
Assim que rodar a API o swagger será iniciado, onde é possível consultar os endpoints e visualizar como são os retornos json de uma forma mais clara.

Dentro do Front-end é possível realizar todas as operações citadas que foram desenvolvidas na API, onde é podemos visualizar de forma paginada os Users e adicionar tanto Randons quanto Users desejados, também temos uma side bar para futuras melhorias.


Segue vídeo para demonstrar como é o projeto funcionando: https://youtu.be/PqxEWQAs9qE
