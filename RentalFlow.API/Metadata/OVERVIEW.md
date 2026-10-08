# 📡 RentalFlow API

API REST para gestão de locação de imóveis.

**Base URL:** `https://localhost:7050`
**Documentação interativa:** `/scalar`

---

## 🔐 Autenticação

A API usa **JWT Bearer Token**.

```http
POST /api/auth/login
Content-Type: application/json

{ "email": "admin@rentalflow.com", "password": "sua-senha" }
```

**Resposta:**
```json
{
  "accessToken": "eyJhbGci...",
  "refreshToken": "base64...",
  "userId": "84e4897a-...",
  "email": "admin@rentalflow.com",
  "role": "Administrator"
}
```

Use o `accessToken` em todas as requisições:

```http
Authorization: Bearer eyJhbGci...
```

Quando o access token expirar, renove com `POST /api/auth/refresh`.

### Roles

| Role | Permissões |
|------|-----------|
| **Administrator** | Acesso total |
| **Manager** | Gerencia o próprio time |
| **Broker** | Acesso apenas ao próprio usuário |

---

## 📋 Endpoints

### Auth
| Método | Rota | Descrição |
|--------|------|-----------|
| `POST` | `/api/auth/login` | Login |
| `POST` | `/api/auth/refresh` | Renova token |
| `POST` | `/api/auth/logout` | Logout |
| `POST` | `/api/auth/change-password` | Altera senha |

### Audit Log
| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/api/audit-log` | Lista |

### Applicants
| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/api/applicants` | Lista |
| `POST` | `/api/applicants` | Cria |
| `PATCH` | `/api/applicants/{id}` | Atualiza |
| `DELETE` | `/api/applicants/{id}` | Remove |

### Operators
| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/api/operators` | Lista |
| `GET` | `/api/operators/{id}` | Busca por ID |
| `POST` | `/api/operators` | Cria |
| `PATCH` | `/api/operators/{id}` | Atualiza |
| `DELETE` | `/api/operators/{id}` | Remove |
| `PATCH` | `/api/operators/{operatorId}/team/{teamId}` | Associa a time |

### Properties
| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/api/properties` | Lista |
| `POST` | `/api/properties` | Cria |
| `PATCH` | `/api/properties/{id}` | Atualiza |
| `DELETE` | `/api/properties/{id}` | Remove |

### Rental Applications
| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/api/rental-applications` | Lista |
| `POST` | `/api/rental-applications` | Cria |
| `PATCH` | `/api/rental-applications/{id}` | Atualiza |
| `PATCH` | `/api/rental-applications/{id}/status` | Muda status |
| `DELETE` | `/api/rental-applications/{id}` | Remove |

### Reports
| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/api/reports/dashboard` | Dashboard |
| `GET` | `/api/reports/top-properties` | Top propriedades |
| `GET` | `/api/reports/top-operators` | Top operadores |
| `GET` | `/api/reports/applications-by-period` | Aplicações por período |
| `GET` | `/api/reports/conversion-rate` | Taxa de conversão das aplicações |

### Teams
| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/api/teams` | Lista |
| `POST` | `/api/teams` | Cria |
| `PATCH` | `/api/teams/{id}` | Atualiza |
| `DELETE` | `/api/teams/{id}` | Remove |

### Users
| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/api/users/me` | Perfil do usuário autenticado |

---

## 📄 Paginação

Todas as listagens retornam:

```json
{
  "page": 1,
  "pageSize": 60,
  "totalResults": 150,
  "totalPages": 3,
  "hasPrevious": false,
  "hasNext": true,
  "results": []
}
```

Parâmetros: `?page=1&pageSize=60` (padrão: `page=1`, `pageSize=60`).

---

## ⚠️ Formato de Erro

**Erros de negócio** (400, 401, 403, 404, 409, 429):

```json
{ "code": 404, "message": "Applicant not found." }
```

**Erros inesperados** (500): formato `ProblemDetails` (RFC 7807) com `correlationId`.

---

## 🚦 Rate Limiting

| Endpoint | Limite |
|----------|--------|
| `/api/auth/login` | 5/min por IP |
| `/api/auth/refresh` | 10/min por IP |
| endpoints autenticados | 100/min por usuário ou IP |
| Demais endpoints | 100/min por usuário ou IP |

Excedeu → `429 Too Many Requests` com header `Retry-After`.

---