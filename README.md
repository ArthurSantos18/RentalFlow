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

---

## 📁 Estrutura do Projeto

```
RentalFlow/
├── RentalFlow.API/                 # Camada de Apresentação (Controllers, Middleware)
│   ├── Controllers/
│   ├── Helpers/
│   └── Program.cs
├── RentalFlow.Application/         # Camada de Aplicação (Handlers, Commands, Queries)
│   ├── UseCases/
│   │   ├── Commands/
│   │   └── Queries/
│   ├── Mappers/
│   ├── Requests/
│   ├── Responses/
│   ├── Interfaces/
│   └── Validators/
├── RentalFlow.Domain/              # Camada de Domínio (Entidades, Enums, Value Objects)
│   ├── Entities/
│   ├── Enums/
│   ├── Errors/
│   ├── Patterns/
│   └── Helpers/
│   └── ValueObject/
├── RentalFlow.Infrastructure/      # Camada de Infraestrutura (DbContext, Repositories)
│   ├── Data/
│   ├── Extensions/
│   ├── Repositories/
├── RentalFlow.Crosscutting/        # Preocupações Transversais (Validações, Configurações)
│   ├── Extensions/
└── RentalFlow.Tests/               # Testes Unitários
    ├── Application/
```

---

---

## 🧩 Entidades Principais

| Entidade | Descrição |
|----------|-----------|
| **Applicant** | Inquilino (pessoa que deseja alugar) |
| **Property** | Imóvel disponível para locação |
| **Operator** | Operador (corretor/gerente/admin) |
| **RentalApplication** | Proposta de locação (une Applicant, Property e Operator) |

---

## 🎯 Funcionalidades

### ✅ CRUDs Completos

- [x] **Applicant** – Cadastro, consulta, atualização, soft delete, restauração.
- [x] **Property** – Cadastro com endereço (Value Object), disponibilidade, soft delete.
- [x] **Operator** – Cadastro, papéis (`Corretor`, `Gerente`, `Administrador`), associação a time.
- [x] **RentalApplication** – Criação, aprovação, rejeição, troca de inquilino/imóvel/operador.

### 📦 Padrões e Boas Práticas

- **Clean Architecture** – Separação clara de responsabilidades.
- **CQRS** – Commands (escrita) e Queries (leitura) separados.
- **Result Pattern** – Tratamento explícito de sucesso/erro.
- **Soft Delete** – Exclusão lógica com `IsActive`.
- **Value Objects** – `Address` encapsulado.
- **FluentValidation** – Validação centralizada.
- **Fluent Interface** – Builders para criação de entidades.

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

2. **Configure a connection string**

   O projeto utiliza **User Secrets** para desenvolvimento. Inicialize e configure:

```bash
dotnet user-secrets init --project RentalFlow.API
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=RentalFlowDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True" --project RentalFlow.API
```

   > Ajuste o `Server` conforme sua instância do SQL Server (ex: `(localdb)\mssqllocaldb`).

3. **Restaure os pacotes e compile**

```bash
dotnet restore
dotnet build
```

4. **Execute as migrações (criação do banco)**

```bash
dotnet ef database update --project RentalFlow.Infrastructure --startup-project RentalFlow.API
```

5. **Execute a aplicação**

```bash
dotnet run --project RentalFlow.API
```

6. **Acesse a API (Scalar)**

   Abra o navegador em: `https://localhost:5001/scalar`

---

## 🧪 Testes

Execute os testes unitários com:

```bash
dotnet test RentalFlow.Tests/RentalFlow.Tests.csproj
```

---

## ✒️ Autor

**Arthur Azevedo**  
- [![LinkedIn](https://img.shields.io/badge/LinkedIn-0077B5?logo=linkedin&logoColor=white)](https://www.linkedin.com/in/arthurazevedo18/)  
- [![GitHub](https://img.shields.io/badge/GitHub-181717?logo=github&logoColor=white)](https://github.com/ArthurSantos18)

**RentalFlow** 🚀
