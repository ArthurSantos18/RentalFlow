## Auth

Endpoints de **autenticação** e gerenciamento de sessão de usuários.

### Responsabilidades

- Emitir tokens JWT (**access token** + **refresh token**) no login
- Renovar sessões expiradas via refresh token
- Revogar tokens no logout (invalidação do refresh token)
- Permitir troca de senha do usuário autenticado

### Comportamento

Os endpoints de **login** e **refresh** são anônimos — não exigem token prévio, pois são a porta de entrada do usuário. Os demais (`logout`, `change-password`) exigem token válido.

### Rate Limiting

Este grupo tem as políticas mais rígidas da API:

- `/login`: **5 tentativas por minuto** por IP (proteção contra brute force)
- `/refresh`: **10 requisições por minuto** por IP
- Demais: **100 requisições por minuto** por usuário autenticado

---

## Applicant

Gerencia os **inquilinos** — pessoas físicas que desejam alugar imóveis.

### Responsabilidades

- Cadastro com CPF, e-mail, telefone e renda mensal
- Consulta paginada com filtros avançados (nome, CPF, e-mail, telefone, faixa de renda, etc.)
- Atualização de dados cadastrais
- Remoção lógica (soft delete)

### Regras de Negócio

- **CPF único**: não é possível cadastrar dois inquilinos com o mesmo CPF
- **Remoção protegida**: um inquilino com propostas de locação associadas não pode ser removido
- **Validação de CPF**: o CPF é validado antes da persistência (dígitos verificadores)
- **Soft delete**: registros removidos não aparecem em consultas, mas permanecem no banco

---

## Operator

Gerencia os **operadores** do sistema — corretores, gerentes e administradores.

### Papéis

| Papel | Responsabilidades |
|-------|-------------------|
| **Broker** | Corretor. Acesso apenas aos próprios dados e propostas. |
| **Manager** | Gerente de time. Gerencia operadores e propostas do próprio time. Não pode promover a `Administrator`. |
| **Administrator** | Acesso total ao sistema. Único que pode criar e excluir times, deletar operadores e gerenciar outros administradores. |

### Responsabilidades

- Cadastro com geração automática de senha temporária
- Associação a um time específico
- Atualização de dados (nome, e-mail, papel, status ativo)
- Remoção lógica (soft delete)
- Reatribuição de time

### Regras de Proteção

- Um operador **não pode se auto-remover**
- O **último administrador ativo** não pode ser removido, desativado ou rebaixado
- **Gerentes** só podem gerenciar operadores do próprio time
- **Gerentes** não podem promover ninguém a `Administrator`
- **Gerentes** não podem editar dados de `Administrator`

---

## Property

Gerencia os **imóveis** disponíveis para locação.

### Responsabilidades

- Cadastro com endereço completo, preço de aluguel, número de quartos e disponibilidade
- Consulta paginada com filtros (cidade, estado, bairro, CEP, faixa de preço, quartos, disponibilidade)
- Atualização de dados
- Remoção lógica (soft delete)

### Estrutura de Endereço

O endereço é um **Value Object** imutável, contendo: `street`, `number`, `complement`, `neighborhood`, `city`, `state`, `zipCode`.

### Regras de Negócio

- **Remoção protegida**: um imóvel com propostas de locação associadas não pode ser removido
- **Disponibilidade**: apenas imóveis com `isAvailable = true` podem receber novas propostas
- **Soft delete**: registros removidos não aparecem em consultas

---

## RentalApplication

Gerencia as **propostas de locação** — o coração do fluxo de negócio. Cada proposta une um inquilino, um imóvel e um operador responsável.

### Responsabilidades

- Criação de propostas com número único, valores (financiado e total) e número de parcelas
- Consulta paginada com filtros (número, status, faixas de valores, datas, etc.)
- Atualização de dados (inquilino, operador, imóvel, valores)
- Mudança de status
- Remoção lógica (soft delete)

### Ciclo de Vida

| Status | Descrição |
|--------|-----------|
| **Draft** | Proposta em rascunho. Editável pelo Broker. |
| **Pending** | Proposta enviada para análise. |
| **Approved** | Proposta aprovada. Não editável. |
| **Rejected** | Proposta rejeitada. Não editável. |

### Regras de Negócio

- **Transições válidas**: nem toda mudança de status é permitida (ex.: `Approved` não volta para `Draft`)
- **Edição restrita**: propostas aprovadas ou rejeitadas **não podem ser editadas**
- **Broker**: só pode editar propostas com status `Draft` e das quais é o operador responsável
- **Escopo de dados**: `Manager` vê apenas propostas do próprio time; `Broker` vê apenas as próprias
- **Validações na criação**: inquilino, imóvel e operador devem existir e estar ativos; o imóvel deve estar disponível

---

## Team

Gerencia as **equipes** de operadores.

### Responsabilidades

- Criação de times com nome único e descrição
- Consulta paginada com filtros (nome, descrição, status ativo)
- Atualização de dados
- Remoção lógica (soft delete)
- Listagem de operadores do time

### Regras de Negócio

- **Nome único**: não é possível criar dois times com o mesmo nome
- **Remoção protegida**: times com operadores ativos não podem ser removidos
- **Acesso restrito**: apenas `Administrator` pode criar e excluir times
- **Edição**: `Manager` só pode editar o próprio time
- **Soft delete**: times removidos não aparecem em consultas