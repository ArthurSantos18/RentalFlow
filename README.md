# 🏠 RentalFlow - Sistema de Gestão de Locações

## 📋 Sobre o Projeto

🚧 **Projeto em desenvolvimento**

RentalFlow é um sistema backend desenvolvido para gerenciar o fluxo completo de locação de imóveis, desde o cadastro de inquilinos, imóveis e operadores até a formalização de propostas e aprovações.

O projeto está sendo desenvolvido com foco em boas práticas de desenvolvimento backend, organização de código, separação de responsabilidades, testes automatizados e aplicação de padrões arquiteturais.

---

## 🚀 Tecnologias Utilizadas

| Tecnologia | Finalidade |
|------------|------------|
| .NET 10 | Plataforma de desenvolvimento |
| C# | Linguagem principal |
| Entity Framework Core | ORM para acesso a dados |
| SQL Server | Banco de dados relacional |
| LiteBus | Barramento de mensagens para CQRS |
| FluentValidation | Validação de dados |
| Serilog | Logging estruturado (console e arquivo) |
| JSON Web Token (JWT) | Autenticação e autorização |
| BCrypt | Criptografia de senhas |
| Scalar | Documentação interativa da API |
| xUnit / Moq / AutoFixture | Testes unitários |
| FluentAssertions | Asserções legíveis nos testes |
| AspNetCore.HealthChecks.SqlServer | Health check de conectividade com SQL Server |
| Health Checks | Monitoramento de saúde da API |

---

## 📁 Estrutura do Projeto

```
RentalFlow/
├── RentalFlow.API/                 # Camada de Apresentação (Controllers, Middleware)
│   ├── Controllers/
│   ├── Extensions/
│   ├── Filters/
│   ├── Handlers/
│   ├── Helpers/
│   ├── Middlewares/
│   ├── Services/
│   ├── Program.cs
│   └── Usings.cs
├── RentalFlow.Application/         # Camada de Aplicação (Handlers, Commands, Queries)
│   ├── Extensions/
│   ├── Interfaces/
│   ├── Mappers/
│   ├── Models/
│   ├── Requests/
│   ├── Responses/
│   ├── Services/
│   ├── UseCases/
│   │   ├── Commands/
│   │   └── Queries/
│   ├── Validators/
│   └── Usings.cs
├── RentalFlow.Domain/              # Camada de Domínio (Entidades, Enums, Value Objects)
│   ├── Entities/
│   ├── Enums/
│   ├── Errors/
│   ├── Helpers/
│   ├── Patterns/
│   ├── ValueObject/
│   └── Usings.cs
├── RentalFlow.Infrastructure/      # Camada de Infraestrutura (DbContext, Repositories)
│   ├── Data/
│   ├── Extensions/
│   ├── Migrations/
│   ├── Repositories/
│   ├── Services/
│   ├── Settings/
│   └── Usings.cs
├── RentalFlow.Crosscutting/        # Preocupações Transversais (Validações, Configurações)
│   └── Extensions/
│   └── Usings.cs
└── RentalFlow.Tests/               # Testes Unitários
    ├── Application/
    ├── Fixtures/
    └── Usings.cs
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
- [x] **Team** – Cadastro, consulta, atualização, soft delete.

### 🔒 Autenticação e Autorização

- [x] **Login** – Autenticação com geração de access token JWT.
- [x] **Refresh** – Renovação de access token via refresh token persistido.
- [x] **Logout** – Revogação do refresh token.
- [x] **Password** – Senha com hash BCrypt e troca autenticada.
- [x] **Autorização por papéis** – Regras específicas para `Broker`, `Manager` e `Administrator`.
- [x] **Data Scope** – Filtro automático de dados por time/operador conforme papel do usuário.

### 📊 Observabilidade

- [x] **Serilog** – Logging estruturado com saída para console e arquivo.
- [x] **Correlation ID** – Cada request recebe um `X-Correlation-Id` (aceito do cliente ou gerado via `TraceIdentifier`), propagado para os logs e devolvido no header da resposta.
- [x] **LogContext por request** – `UserId`, `Role` e `CorrelationId` injetados automaticamente nos logs via middleware.
- [x] **Logs de Commands** – Início, sucesso e warnings com `ErrorCode` + `ErrorMessage`.
- [x] **Global Exception Handler** – Tratamento centralizado via `IExceptionHandler`, com resposta padronizada em `ProblemDetails` contendo `traceId`/`correlationId` e log estruturado.
- [x] **Health Checks** – Endpoints `/health/live` (liveness) e `/health/ready` (readiness) com resposta JSON, integrados ao SQL Server.

### 🌐 API

- [x] **Scalar** – Documentação interativa em `/scalar`.
- [x] **OpenAPI** – Especificação gerada automaticamente.
- [x] **JWT Bearer** – Autenticação via header `Authorization`.

### 📦 Padrões e Boas Práticas

**Arquitetura**
- **Clean Architecture** – Separação clara de responsabilidades.
- **CQRS** – Commands (escrita) e Queries (leitura) separados via LiteBus.
- **Repository Pattern** – Abstração do acesso a dados por agregado.
- **Result Pattern** – Tratamento explícito de sucesso/erro, sem exceptions para fluxo de negócio.
- **Global Exception Handler** – Tratamento centralizado via `IExceptionHandler` com `ProblemDetails`.

**Modelagem**
- **Soft Delete** – Exclusão lógica com `IsDeleted`.
- **Value Objects** – `Address` encapsulado.

**Validação e Configuração**
- **FluentValidation** – Validação centralizada de requests.
- **User Secrets** – Credenciais sensíveis fora do versionamento.
- **Migrations** – Versionamento do schema via EF Core Migrations.
- **Global Using** – Centralização de `using` por projeto.

---

## 🚀 Como Executar o Projeto

### Pré-requisitos

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
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

3. **Configure o `appsettings.json` com sua connection string e configurações JWT**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "sua-connection-string"
  },
  "Jwt": {
    "Issuer": "sua-issuer",
    "Audience": "sua-audience",
    "SecretKey": "sua-secret-key",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  }
}
```

> `AccessTokenExpirationMinutes`: duração do access token em minutos.  
> `RefreshTokenExpirationDays`: duração do refresh token em dias.

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

Abra o navegador em `https://localhost:<porta>/scalar` (a porta é definida em `Properties/launchSettings.json`).

---

## 📊 Logging e Observabilidade

O projeto utiliza **Serilog** para logging estruturado com:

- Saída para **console** e **arquivo** (rolling file diário, retenção de 30 dias).
- Configuração de níveis por namespace (`Microsoft` e `Microsoft.EntityFrameworkCore` em `Warning`).
- Enriquecimento com `Application`, `CorrelationId`, `UserId` e `Role`.
- Formato dos logs:
  ```
  20:56:35 [INF] [CorrelationId:0HNOV9P3R8100:00000013] [User:84e4897a-...] [Role:Administrator] Creating property...
  ```

---

## 🩺 Health Checks

O projeto expõe dois endpoints de health check no padrão, com resposta em JSON:

| Endpoint | Tipo | Verifica | Ação em caso de falha |
|----------|------|----------|----------------------|
| `/health/live` | Liveness | Processo está vivo | Orquestrador **reinicia** o container |
| `/health/ready` | Readiness | SQL Server acessível | Orquestrador **remove do load balancer** |

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
