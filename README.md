# 🏠 RentalFlow - Sistema de Gestão de Locações

## 📋 Sobre o Projeto

🚧 **Projeto em desenvolvimento**

RentalFlow é um sistema backend desenvolvido para gerenciar o fluxo completo de locação de imóveis, desde o cadastro de inquilinos, imóveis e operadores até a formalização de propostas e aprovações.

O projeto está sendo desenvolvido com foco em boas práticas de desenvolvimento backend, organização de código, separação de responsabilidades, testes automatizados e aplicação de padrões arquiteturais.

---

## 🚀 Tecnologias Utilizadas

| Tecnologia | Finalidade |
|------------|------------|
| .NET | Plataforma de desenvolvimento |
| C# | Linguagem principal |
| Entity Framework Core | ORM para acesso a dados (SQL Server) |
| LiteBus | Barramento de mensagens para CQRS |
| FluentValidation | Validação de dados |
| Moq / AutoFixture / xUnit | Testes unitários |
| SQL Server | Banco de dados relacional |
| JSON Web Token (JWT) | Autenticação e Autorização |
| BCrypt | Criptografia de senhas |

---

## 📁 Estrutura do Projeto

```
RentalFlow/
├── RentalFlow.API/                 # Camada de Apresentação (Controllers, Middleware)
│   ├── Controllers/
│   ├── Helpers/
│   └── Program.cs
│   └── Usings.cs
├── RentalFlow.Application/         # Camada de Aplicação (Handlers, Commands, Queries)
│   ├── UseCases/
│   │   ├── Commands/
│   │   └── Queries/
│   ├── Mappers/
│   ├── Requests/
│   ├── Responses/
│   ├── Interfaces/
│   └── Validators/
│   └── Usings.cs
├── RentalFlow.Domain/              # Camada de Domínio (Entidades, Enums, Value Objects)
│   ├── Entities/
│   ├── Enums/
│   ├── Errors/
│   ├── Patterns/
│   └── Helpers/
│   └── ValueObject/
│   └── Usings.cs
├── RentalFlow.Infrastructure/      # Camada de Infraestrutura (DbContext, Repositories)
│   ├── Data/
│   ├── Extensions/
│   ├── Repositories/
│   └── Usings.cs/
├── RentalFlow.Crosscutting/        # Preocupações Transversais (Validações, Configurações)
│   ├── Extensions/
└── RentalFlow.Tests/               # Testes Unitários
    ├── Application/
    ├── Fixtures/
│   └── Usings.cs
```

---

## 🧩 Entidades Principais

| Entidade | Descrição |
|----------|-----------|
| **Applicant** | Inquilino (pessoa que deseja alugar) |
| **Property** | Imóvel disponível para locação |
| **Operator** | Operador (corretor/gerente/admin) |
| **RentalApplication** | Proposta de locação (une Applicant, Property e Operator) |
| **Team** | Times ao qual os operadores pertencem |
| **User** | Usuário ao qual possuem as credenciais para login |
| **UserToken** | Entidade que guarda os tokens e informações adicionais da autenticação do usuário |

---

## 🎯 Funcionalidades

### ✅ CRUDs Completos

- [x] **Applicant** – Cadastro, consulta, atualização, soft delete.
- [x] **Property** – Cadastro com endereço (Value Object), consulta, atualização, soft delete.
- [x] **Operator** – Cadastro, papéis (`Broker`, `Manager`, `Administrator`), associação a time, consulta, atualização, soft delete.
- [x] **RentalApplication** – Cadastro, consulta, atualização, mudança de status, soft delete.
- [x] **Team** - Cadastro, consulta, atualização, soft delete.

### ✅ Autenticação

- [x] **Login** - Login de usuário com geração de token Jwt.
- [x] **Refresh** - Refresh token com endpoint para renovação do mesmo.
- [ ] **Password** - Senha criptografada, com opção de mudança.

### 📦 Padrões e Boas Práticas

- **Clean Architecture** – Separação clara de responsabilidades.
- **CQRS** – Commands (escrita) e Queries (leitura) separados.
- **Result Pattern** – Tratamento explícito de sucesso/erro.
- **Soft Delete** – Exclusão lógica com `IsDeleted`.
- **Value Objects** – `Address` encapsulado.
- **FluentValidation** – Validação centralizada.
- **Global Using** - Utilização de global using para centralização.

---

## 🚀 Como Executar o Projeto

### Pré‑requisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [SQL Server 2022](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads) (ou SQL Server Express/LocalDB)
- [Git](https://git-scm.com/)

### Passo a Passo

1. **Clone o repositório**

```bash
git clone https://github.com/ArthurSantos18/RentalFlow.git
cd RentalFlow
```

2. **Configure as credenciais de administrador**

   O projeto utiliza **User Secrets** para desenvolvimento. Inicialize e configure:

```bash
dotnet user-secrets init --project RentalFlow.API
dotnet user-secrets set "Seed:AdminEmail" "email-do-administrador" --project RentalFlow.API
dotnet user-secrets set "Seed:AdminPassword" "senha-do-administrador" --project RentalFlow.API
dotnet user-secrets set "Seed:AdminName" "nome-do-administrador" --project RentalFlow.API
dotnet user-secrets set "Seed:TeamName" "nome-do-grupo-do-administrador" --project RentalFlow.API
```

3. **Configure o appsettings com sua connection string e configurações jwt**

```bash
{
  "ConnectionStrings": {
    "DefaultConnection": "sua-connection-string"
  },
  "Jwt": {
    "Issuer": "sua-issuer",
    "Audience": "sua-audience",
    "SecretKey": "sua-secret-key",
    "AccessTokenExpirationMinutes": 0, // Tempo em minutos de duração do access token
    "RefreshTokenExpirationDays": 0 // Tempo em dias de duração do refresh token
  }
}
```


4. **Restaure os pacotes e compile**

```bash
dotnet restore
dotnet build
```

5. **Execute as migrações (criação do banco)**

```bash
dotnet ef database update --project RentalFlow.Infrastructure --startup-project RentalFlow.API
```

6. **Execute a aplicação**

```bash
dotnet run --project RentalFlow.API
```

7. **Acesse a API (Scalar)**

   Abra o navegador em: `https://localhost:5001/scalar`

---

## 🧪 Testes

Execute os testes unitários com:

```bash
dotnet test RentalFlow.Tests/RentalFlow.Tests.csproj
```

---

## ✒️ Autor

**Arthur Santos Azevedo**  
- [![LinkedIn](https://img.shields.io/badge/LinkedIn-0077B5?logo=linkedin&logoColor=white)](https://www.linkedin.com/in/arthurazevedo18/)  
- [![GitHub](https://img.shields.io/badge/GitHub-181717?logo=github&logoColor=white)](https://github.com/ArthurSantos18)

**RentalFlow** 🚀
