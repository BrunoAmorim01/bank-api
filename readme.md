# API de Banco

Esta é uma API de banco que oferece várias funcionalidades para gerenciar usuários, transações e autenticação. A API utiliza o framework .NET e o banco de dados PostgreSQL.

## Funcionalidades

1. Criação de usuário com envio de e-mail utilizando o SES da AWS.
2. Visualização do saldo do usuário.
3. Realização de depósitos, que são processados por uma fila no SQS.
4. Login com JWT (JSON Web Token).
5. Utilização de migrações do Entity Framework para gerenciar o banco de dados.

## Rotas Disponíveis

- Depósito: `POST /api/transactions/deposit`
- Cadastro de Usuário: `POST /api/users`
- Login de Usuário: `POST /api/auth/login`

## Configuração

O arquivo de configuração `appsettings.Development.json` contém as configurações necessárias para executar a API. Você pode utilizar o arquivo `appsettings.example.json` como base para criar o seu próprio arquivo de configuração.

## Como Executar

1. Clone este repositório.
2. Configure o arquivo `appsettings.Development.json` com as suas credenciais e configurações.
3. Execute o comando `dotnet run` para iniciar a API.

## Contribuição

Contribuições são bem-vindas! Sinta-se à vontade para abrir uma issue ou enviar um pull request.

## Licença

Este projeto está licenciado sob a [MIT License](LICENSE).