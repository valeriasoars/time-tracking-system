# Sistema de Ponto

API RESTful para controle de ponto eletrônico de empresas. Funcionários registram entrada e saída, e administradores acompanham, gerenciam e geram relatórios de horas trabalhadas.

Desenvolvido em **.NET / C#**, com **Entity Framework Core**

---

## Funcionalidades

- Cadastro e autenticação de usuários (Funcionário / Admin)
- Registro de ponto (entrada/saída) com validação de sequência
- Cálculo automático de horas trabalhada
- Histórico de ponto por funcionário
- Relatórios administrativos com filtros por funcionário e período
- Configuração de jornada padrão e taxa de hora extra

---

## Arquitetura

O projeto segue o padrão **Controller → Service → Interface → DTO**, com regras de negócio centralizadas na camada de serviço.

```
SistemaPontos/
├── Controllers/          
├── Services/            
│   └── Interfaces/      
├── Helpers/             
├── Dto/                  
├── models/              
├── enums/               
└── data/                
```

---

##  Autenticação e Autorização

Autenticação via **JWT**. O token contém claims de `Id`, `Email` e `Role`, e expira em **4 horas**.

| Papel | Permissões |
|---|---|
| `EMPLOYEE` | Registrar próprio ponto, consultar próprio histórico |
| `ADMIN` | Tudo que o `EMPLOYEE` faz, além de: cadastrar funcionários, ver relatórios da empresa, listar pontos de todos, configurar jornada padrão |

---

## Endpoints

### Autenticação

| Método | Rota | Autenticação | Descrição |
|---|---|---|---|
| `POST` | `/api/auth/signup` | Nenhuma | Cria uma conta de administrador |
| `POST` | `/api/auth/login` | Nenhuma | Autentica e retorna JWT + role |

**POST `/api/auth/login`**
```json
// Request
{
  "email": "admin@empresa.com",
  "password": "123456"
}

// Response
{
  "token": "jwt-token",
  "role": "ADMIN"
}
```

### Usuários

| Método | Rota | Autenticação | Descrição |
|---|---|---|---|
| `POST` | `/api/user/register/employee` | ADMIN | Cadastra um funcionário |

### Ponto

| Método | Rota | Autenticação | Descrição |
|---|---|---|---|
| `POST` | `/api/punch-clock` | Autenticado | Registra entrada ou saída |
| `GET` | `/api/punch-clock/history` | Autenticado | Histórico de ponto do próprio usuário |
| `GET` | `/api/punch-clock/admin` | ADMIN | Lista pontos de todos, com filtros |

**POST `/api/punch-clock`**
```json
// Request
{ "typePunch": "CHECKIN" }

// Response
{
  "message": "Entrada registrada com sucesso!",
  "timeStamp": "2026-07-15T08:00:00Z"
}
```

**GET `/api/punch-clock/admin?employeeId={id}&startDate=2026-07-01&endDate=2026-07-31`**
```json
[
  {
    "employee": "Fulano da Silva",
    "date": "2026-07-15",
    "checkIn": "08:00",
    "checkOut": "17:00",
    "hoursWorked": 8
  }
]
```

### Relatórios (Dashboard)

| Método | Rota | Autenticação | Descrição |
|---|---|---|---|
| `GET` | `/api/dashboard/admin/reports` | ADMIN | Total de horas da empresa por período, por funcionário |

**GET `/api/dashboard/admin/reports?startDate=2026-07-01&endDate=2026-07-31`**
```json
{
  "totalHours": 84,
  "employees": [
    { "name": "Fulano da Silva", "hoursWorked": 44 },
    { "name": "Ciclana Machado", "hoursWorked": 40 }
  ]
}
```

### Configurações

| Método | Rota | Autenticação | Descrição |
|---|---|---|---|
| `GET` | `/api/settings` | Nenhuma | Consulta jornada padrão e taxa de hora extra |
| `POST` | `/api/settings` | ADMIN | Cria/atualiza jornada padrão e taxa de hora extra |

**POST `/api/settings`**
```json
{
  "workdayHours": 8,
  "overtimeRate": 1.5
}
```

---

## Estrutura do Banco de Dados

### Users
| Campo | Tipo |
|---|---|
| `Id` | UUID (PK) |
| `Name` | string |
| `Email` | string (único) |
| `Password` | string (hash BCrypt) |
| `Role` | enum (`EMPLOYEE`, `ADMIN`) |

### PunchClock
| Campo | Tipo |
|---|---|
| `Id` | UUID (PK) |
| `UserId` | UUID (FK → Users) |
| `TypePunch` | enum (`CHECKIN`, `CHECKOUT`) |
| `TimeStamp` | DateTime (UTC) |

### Settings
| Campo | Tipo |
|---|---|
| `Id` | UUID (PK) |
| `WorkdayHours` | decimal |
| `OvertimeRate` | decimal |

---

##  Como rodar o projeto

### Pré-requisitos
- [.NET SDK](https://dotnet.microsoft.com/) instalado
- SQLServer rodando localmente (ou em container)

### Passos

```bash
# Clonar o repositório
git clone https://github.com/valeriasoars/time-tracking-system.git

# Restaurar dependências
dotnet restore

# Ajustar a connection string em appsettings.json
# e a chave JWT em JwtSettings:Secret

# Aplicar as migrations
dotnet ef database update

# Rodar a aplicação
dotnet run
```

### Documentação interativa (Scalar)

```
https://localhost:{porta}/scalar/v1
```

### Variáveis de configuração (`appsettings.json`)

```json
{
  "ConnectionStrings": {
      "DefaultConnection": "Server=seuservidor;Database=db_sistema_pontos;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "Secret": "sua-chave-secreta-com-pelo-menos-32-caracteres"
  }
}
```
---

##  Tecnologias

- .NET / C#
- Entity Framework Core
- SQLServer
- JWT
- BCrypt 
- Scalar 
