# 🔗 URL Encurtador API

[![NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?logo=postgresql)](https://www.postgresql.org/)
[![Docker](https://img.shields.io/badge/Docker-Enabled-2496ED?logo=docker)](https://www.docker.com/)
[![OpenTelemetry](https://img.shields.io/badge/OpenTelemetry-Enabled-000000?logo=opentelemetry)](https://opentelemetry.io/)
[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE.md)

Um serviço web moderno, de alta performance e resiliência para encurtamento de URLs, desenvolvido em **ASP.NET Core 9.0**, **PostgreSQL** e **Entity Framework Core**. Conta com algoritmo customizado de codificação **Base62**, suporte completo a **Docker / Docker Compose**, rastreamento distribuído via **OpenTelemetry** e suíte de testes unitários automatizados.

---

## 📋 Índice

- [Sobre o Projeto](#-sobre-o-projeto)
- [Tecnologias Utilizadas](#-tecnologias-utilizadas)
- [Arquitetura & Como Funciona](#-arquitetura--como-funciona)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Configuração (.env)](#-configuração-env)
- [Como Executar](#-como-executar)
  - [Opção 1: Via Docker Compose (Recomendado)](#opção-1-via-docker-compose-recomendado)
  - [Opção 2: Execução Local (.NET CLI)](#opção-2-execução-local-net-cli)
- [Documentação da API (Endpoints)](#-documentação-da-api-endpoints)
- [Observabilidade](#-observabilidade)
- [Testes Unitários](#-testes-unitários)
- [Licença](#-licença)
- [Autor](#-autor)

---

## 🎯 Sobre o Projeto

O **URL Encurtador** permite transformar links extensos em URLs curtas e de fácil compartilhamento. Cada link recebe um código único e compacto gerado por um algoritmo numérico baseado em **Base62**.

### 🌟 Principais Recursos

- ⚡ **Geração de Hash Base62 único**: Códigos curtos derivados de números aleatórios seguros com offset (`1.000.000`) para evitar adivinhação sequencial.
- 🔁 **Redirecionamento HTTP 301**: Redirecionamento permanente (`Moved Permanently`) otimizado para SEO e navegação transparente.
- 🛠️ **CRUD Completo de URLs**: Criação, busca por código, redirecionamento, listagem de todas as URLs, atualização e deleção.
- 🐳 **Pronto para Produção com Docker**: Build multi-stage otimizado no `Dockerfile` e orquestração simplificada com `docker-compose.yml`.
- 📊 **Observabilidade Nativa**: Rastreamento distribuído via **OpenTelemetry (OTLP)**.
- 🧪 **Testes Unitários**: Testes automatizados cobrindo Controllers, Services e o conversor Base62.
- 🔄 **Auto Migration**: Aplicação automática de migrações do EF Core na inicialização do servidor.

---

## 🚀 Tecnologias Utilizadas

- **Linguagem & Framework**: C# / .NET 9.0 (ASP.NET Core Web API)
- **Persistência de Dados**: Entity Framework Core 9.0 + PostgreSQL (`Npgsql.EntityFrameworkCore.PostgreSQL`)
- **Documentação de API**: OpenAPI (`Microsoft.AspNetCore.OpenApi`)
- **Observabilidade**: OpenTelemetry (`OpenTelemetry.Extensions.Hosting`, `Instrumentation.AspNetCore`, `Exporter.OpenTelemetryProtocol`)
- **Containerização**: Docker & Docker Compose
- **Testes Unitários**: xUnit, Moq, FluentAssertions, EntityFrameworkCore.InMemory

---

## 🧠 Arquitetura & Como Funciona

### 1. Codificação Base62

O algoritmo converte valores numéricos inteiros para uma representação textual no alfabeto de 62 caracteres legíveis `[0-9A-Za-z]`:

$$\text{Alfabeto} = \text{"0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"}$$

### 2. Fluxo de Geração de Código

1. O cliente faz uma requisição `POST /` enviando a URL original no body.
2. A aplicação gera um número aleatório de 64 bits (`Random.NextInt64()`) e adiciona o deslocamento base (`IDOFFSET = 1.000.000`).
3. O valor resultante é convertido para Base62 (ex: `4C93`).
4. O sistema garante a unicidade do código no banco de dados PostgreSQL.
5. O registro é persistido com a URL original e o código encurtado.

### 3. Fluxo de Redirecionamento

```
Cliente ───────> GET /{code} ───────> ControllerUrl ───────> ServicesUrl ───────> DB (PostgreSQL)
                                                                                       │
                                      ┌────────────────────────────────────────────────┘
                                      ▼
                             ┌─────────────────┐
                             │ Código Existe?  │
                             └────────┬────────┘
                                      │
                         ┌────────────┴────────────┐
                         ▼                         ▼
                   [Sim] 301 Redirect         [Não] 404 Not Found
```

---

## 📁 Estrutura do Projeto

```
urlencurtador/
├── .env                        # Variáveis de ambiente locais (ignorado no git)
├── .env.example                # Modelo de variáveis de ambiente
├── .gitignore                  # Regras de ignorados do Git
├── Dockerfile                  # Build multi-stage para containerização da API
├── docker-compose.yml          # Orquestração da API e banco de dados PostgreSQL
├── LICENSE.md                  # Termos de licença MIT
├── README.md                   # Documentação do projeto
├── src/                        # Código-fonte da aplicação
│   ├── urlencurtador.csproj    # Projeto ASP.NET Core Web API (.NET 9.0)
│   ├── urlencurtador.sln       # Solution file do .NET
│   ├── Program.cs              # Entrypoint, DI, OpenTelemetry e Migrations
│   ├── appsettings.json        # Configurações de logging e connections
│   ├── controllers/
│   │   └── ControllerUrl.cs    # Controller REST com os endpoints HTTP
│   ├── services/
│   │   ├── ServicesUrl.cs      # Lógica de negócio e geração de códigos
│   │   ├── Base62Converter.cs  # Algoritmo de conversão Base10 ↔ Base62
│   │   └── Interfaces/
│   │       └── IServices.cs    # Interface genérica de serviços
│   ├── model/
│   │   └── modelurl.cs         # Entidade de domínio (Id, Url, Code)
│   ├── infra/
│   │   └── DBurl.cs            # DbContext do Entity Framework Core
│   └── Migrations/             # Migrações do PostgreSQL
└── Tests/                      # Projeto de testes unitários (xUnit)
    ├── urlencurtador.Tests.csproj
    ├── ControllerUrlTests.cs   # Testes dos endpoints HTTP
    ├── ServicesUrlTests.cs     # Testes da regra de negócio
    └── Base62ConverterTests.cs # Testes do algoritmo Base62
```

---

## ⚙️ Configuração (.env)

A aplicação utiliza variáveis de ambiente para fácil implantação e desacoplamento de credenciais.

Crie um arquivo `.env` na raiz do projeto copiando o modelo `.env.example`:

```bash
cp .env.example .env
```

### Exemplo de arquivo `.env`:

```env
# Configurações da Aplicação
APP_PORT=8080
ASPNETCORE_ENVIRONMENT=Development

# Banco de Dados (PostgreSQL)
DB_HOST=db
DB_NAME=meubanco
DB_USER=postgres
DB_PASSWORD=senha123

# String de Conexão formatada para o Entity Framework Core
ConnectionStrings__DefaultConnection="Host=db;Database=meubanco;Username=postgres;Password=senha123"
```

---

## 🛠️ Como Executar

### Opção 1: Via Docker Compose (Recomendado)

Certifique-se de ter o [Docker](https://www.docker.com/) e [Docker Compose](https://docs.docker.com/compose/) instalados em sua máquina.

1. **Inicie os serviços (API + PostgreSQL):**

   ```bash
   docker-compose up -d --build
   ```

2. **Verifique o status dos contêineres:**

   ```bash
   docker-compose ps
   ```

3. **Acesse a API:**
   - A aplicação estará pronta e escutando em: `http://localhost:8080` (conforme definido na variável `APP_PORT`).

4. **Para encerrar a execução:**

   ```bash
   docker-compose down
   ```

---

### Opção 2: Execução Local (.NET CLI)

#### Pré-requisitos:
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Instância ativa do **PostgreSQL 16**

1. **Suba um contêiner PostgreSQL para testes locais (opcional):**

   ```bash
   docker run --name postgres-url -e POSTGRES_DB=meubanco -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=senha123 -p 5432:5432 -d postgres:16-alpine
   ```

2. **Ajuste a String de Conexão no `src/appsettings.json` ou exporte no ambiente:**

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=meubanco;Username=postgres;Password=senha123"
     }
   }
   ```

3. **Navegue até a pasta do projeto e execute:**

   ```bash
   cd src
   dotnet run
   ```

> 💡 *Nota: As migrações do banco de dados são aplicadas automaticamente durante a inicialização da aplicação (`db.Database.Migrate()`).*

---

## 📡 Documentação da API (Endpoints)

### 1. Criar URL Encurtada

- **Endpoint**: `POST /`
- **Headers**: `Content-Type: application/json`
- **Body**:
  ```json
  {
    "url": "https://www.google.com"
  }
  ```
- **Resposta (`201 Created`)**:
  ```json
  {
    "id": 1,
    "url": "https://www.google.com",
    "code": "4C93"
  }
  ```
- **Exemplo cURL**:
  ```bash
  curl -X POST http://localhost:8080/ \
    -H "Content-Type: application/json" \
    -d '{"url": "https://www.google.com"}'
  ```

---

### 2. Redirecionar para URL Original

- **Endpoint**: `GET /{code}`
- **Exemplo**: `GET /4C93`
- **Resposta**:
  - `301 Moved Permanently` (redireciona para o link original)
  - `404 Not Found` (caso o código não seja localizado)
- **Exemplo cURL**:
  ```bash
  curl -i http://localhost:8080/4C93
  ```

---

### 3. Consultar URL Original por Código

- **Endpoint**: `GET /get/{code}`
- **Exemplo**: `GET /get/4C93`
- **Resposta (`200 OK`)**:
  ```json
  "https://www.google.com"
  ```
- **Resposta (`404 Not Found`)**: Código não cadastrado.
- **Exemplo cURL**:
  ```bash
  curl -X GET http://localhost:8080/get/4C93
  ```

---

### 4. Listar Todas as URLs Cadastradas

- **Endpoint**: `GET /list`
- **Resposta (`200 OK`)**:
  ```json
  [
    {
      "id": 1,
      "url": "https://www.google.com",
      "code": "4C93"
    }
  ]
  ```
- **Exemplo cURL**:
  ```bash
  curl -X GET http://localhost:8080/list
  ```

---

### 5. Atualizar URL Registrada

- **Endpoint**: `PUT /update/{id}`
- **Headers**: `Content-Type: application/json`
- **Body**:
  ```json
  {
    "url": "https://www.novo-exemplo.com"
  }
  ```
- **Resposta (`200 OK`)**
- **Exemplo cURL**:
  ```bash
  curl -X PUT http://localhost:8080/update/1 \
    -H "Content-Type: application/json" \
    -d '{"url": "https://www.novo-exemplo.com"}'
  ```

---

### 6. Deletar URL por ID

- **Endpoint**: `DELETE /delete/{id}`
- **Exemplo**: `DELETE /delete/1`
- **Resposta (`200 OK`)**
- **Exemplo cURL**:
  ```bash
  curl -X DELETE http://localhost:8080/delete/1
  ```

---

## 📊 Observabilidade

A API conta com rastreamento distribuído pré-configurado via **OpenTelemetry**:

- **Tracing de HTTP Requests**: Captura instrumentação de rotas e requisições no ASP.NET Core (`AddAspNetCoreInstrumentation()`).
- **Exportador OTLP**: Envio via `AddOtlpExporter()`, pronto para integração com ferramentas como **Jaeger**, **Grafana Tempo**, **Datadog** ou **OpenTelemetry Collector**.

---

## 🧪 Testes Unitários

O repositório possui cobertura de testes unitários desenvolvida com **xUnit**, **Moq**, **FluentAssertions** e **EF Core InMemory Database**.

Para rodar a suíte de testes unitários:

```bash
dotnet test src/urlencurtador.sln
```

Ou apontando diretamente para o projeto de testes:

```bash
dotnet test Tests/urlencurtador.Tests.csproj
```

---

## 📄 Licença

Este projeto está licenciado sob a licença **MIT** - consulte o arquivo [LICENSE.md](LICENSE.md) para mais detalhes.

---

## 👤 Autor

Desenvolvido por **Deureck de Souza Passarela**.

[![GitHub](https://img.shields.io/badge/GitHub-deureck-181717?logo=github)](https://github.com/deureck)
