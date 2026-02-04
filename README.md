# 🔗 URL Encurtador

Um encurtador de URLs moderno e eficiente desenvolvido em ASP.NET Core 9.0 com PostgreSQL.

## 📋 Índice

- [Sobre o Projeto](#sobre-o-projeto)
- [Tecnologias](#tecnologias)
- [Funcionalidades](#funcionalidades)
- [Pré-requisitos](#pré-requisitos)
- [Instalação](#instalação)
- [Configuração](#configuração)
- [Uso](#uso)
- [API Endpoints](#api-endpoints)
- [Testes](#testes)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Como Funciona](#como-funciona)
- [Contribuindo](#contribuindo)

## 🎯 Sobre o Projeto

Este projeto é um encurtador de URLs que permite transformar URLs longas em links curtos e fáceis de compartilhar. Utiliza codificação Base62 para gerar hashes únicos e compactos.

### Características Principais

- ✅ Criação de URLs curtas
- ✅ Redirecionamento permanente (301)
- ✅ CRUD completo de URLs
- ✅ Codificação Base62 para hashes compactos
- ✅ Banco de dados PostgreSQL
- ✅ Testes unitários completos (50 testes, 100% de sucesso)
- ✅ Docker Compose para fácil deployment

## 🚀 Tecnologias

- **[.NET 9.0](https://dotnet.microsoft.com/)** - Framework principal
- **[ASP.NET Core](https://docs.microsoft.com/aspnet/core)** - Web API
- **[Entity Framework Core 9.0](https://docs.microsoft.com/ef/core)** - ORM
- **[PostgreSQL 16](https://www.postgresql.org/)** - Banco de dados
- **[Docker](https://www.docker.com/)** - Containerização
- **[xUnit](https://xunit.net/)** - Framework de testes
- **[FluentAssertions](https://fluentassertions.com/)** - Assertions para testes
- **[Moq](https://github.com/moq/moq4)** - Mocking para testes

## ⚡ Funcionalidades

- **Encurtar URLs**: Converte URLs longas em links curtos
- **Redirecionamento**: Redireciona automaticamente para a URL original
- **Gerenciamento**: CRUD completo (Create, Read, Update, Delete)
- **Hash Customizado**: Geração de hash Base62 a partir de IDs
- **Listagem**: Visualize todas as URLs cadastradas

## 📦 Pré-requisitos

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/get-started) e Docker Compose (opcional, para PostgreSQL)
- PostgreSQL 16+ (se não usar Docker)

## 🔧 Instalação

### 1. Clone o repositório

```bash
git clone https://github.com/deureck/urlencurtador.git
cd urlencurtador
cd src
```

### 2. Inicie o banco de dados com Docker

```bash
docker-compose up -d
```

### 3. Execute as migrações

```bash
dotnet ef database update
```

### 4. Execute o projeto

```bash
dotnet run
```

A API estará disponível em `http://localhost:5018` (ou a porta configurada).

## ⚙️ Configuração

### Banco de Dados

Edite o arquivo `appsettings.json` para configurar a conexão com o PostgreSQL:

```json
{
  "ConnectionStrings": {
    "Postgress": "Host=127.0.0.1;Username=meu_usuario;Password=minha_senha_segura;Database=meu_banco_de_dados;"
  }
}
```

### Docker Compose

O arquivo `docker-compose.yml` já está configurado com:
- PostgreSQL 16.11
- Porta: 5432
- Usuário: `meu_usuario`
- Senha: `minha_senha_segura`
- Database: `meu_banco_de_dados`

## 💻 Uso

### Exemplo Rápido

1. **Criar uma URL curta**:
```bash
curl -X POST http://localhost:5018/ \
  -H "Content-Type: application/json" \
  -d '{"url": "https://www.exemplo.com.br/pagina-muito-longa"}'
```

2. **Obter o hash da URL** (usando o ID retornado):
```bash
curl http://localhost:5018/createHash/1
# Retorna: {"hash":"4C93"}
```

3. **Acessar a URL curta**:
```bash
curl -L http://localhost:5018/4C93
# Redireciona para: https://www.exemplo.com.br/pagina-muito-longa
```

## 📡 API Endpoints

### Criar URL
```http
POST /
Content-Type: application/json

{
  "url": "https://www.exemplo.com"
}
```
**Resposta**: `201 Created`

---

### Obter URL por ID
```http
GET /get/{id}
```
**Resposta**: `200 OK` com objeto URL ou `404 Not Found`

---

### Gerar Hash para ID
```http
GET /createHash/{id}
```
**Resposta**: 
```json
{
  "hash": "4C93"
}
```

---

### Redirecionar para URL Original
```http
GET /{hash}
```
**Resposta**: `301 Redirect` para URL original ou `404 Not Found`

---

### Listar Todas as URLs
```http
GET /list
```
**Resposta**: Array de URLs

---

### Atualizar URL
```http
PUT /update/{id}
Content-Type: application/json

{
  "url": "https://www.novo-exemplo.com"
}
```
**Resposta**: `200 OK`

---

### Deletar URL
```http
DELETE /delete/{id}
```
**Resposta**: `200 OK`

## 🧪 Testes

O projeto possui **50 testes unitários** com **100% de taxa de sucesso**.

### Executar todos os testes

```bash
dotnet test --project ./Tests
```

### Executar testes com detalhes

```bash
dotnet test --project ./Tests --verbosity normal
```

### Executar testes de uma classe específica

```bash
dotnet test --project ./Tests --filter "FullyQualifiedName~Base62ConverterTests"
```

### Cobertura de Testes

- ✅ **Base62ConverterTests** (15 testes) - Codificação/Decodificação Base62
- ✅ **ServicesUrlTests** (15 testes) - Lógica de negócio e CRUD
- ✅ **ControllerUrlTests** (20 testes) - Endpoints da API

## 📁 Estrutura do Projeto

```
Tests/
├── Base62ConverterTests.cs   # Testes do conversor
├── ServicesUrlTests.cs       # Testes dos serviços
└── ControllerUrlTests.cs     # Testes do controller
src/
├── controllers/
│   └── ControllerUrl.cs          # Controller da API
├── services/
│   ├── ServicesUrl.cs            # Lógica de negócio
│   ├── Base62Converter.cs        # Conversor Base62
│   └── Interfaces/
│       └── IServices.cs          # Interface genérica
├── model/
│   └── modelurl.cs               # Modelo de dados
├── infra/
│   └── DBurl.cs                  # Contexto do EF Core
├── Migrations/                   # Migrações do banco
├── Program.cs                    # Ponto de entrada
├── appsettings.json             # Configurações
├── docker-compose.yml           # Configuração Docker
README.md                    # Este arquivo
```

## 🔍 Como Funciona

### Codificação Base62

O projeto utiliza codificação Base62 para gerar hashes curtos e legíveis:

- **Alfabeto**: `0-9A-Za-z` (62 caracteres)
- **Offset**: IDs são somados com 1.000.000 antes da codificação
- **Exemplo**: ID `1` → `1000001` → Hash `"4C93"`

### Fluxo de Encurtamento

1. Usuário envia URL longa via POST
2. Sistema salva no banco e recebe um ID auto-incrementado
3. ID é codificado em Base62 (com offset)
4. Hash pode ser usado para acessar: `/{hash}`
5. Sistema decodifica hash, busca URL e redireciona (301)

### Exemplo de Conversão

```
ID no banco: 1
ID + Offset: 1 + 1000000 = 1000001
Base62:      "4C93"
URL curta:   http://localhost:5018/4C93
```

## 🤝 Contribuindo

Contribuições são bem-vindas! Sinta-se à vontade para:

1. Fazer fork do projeto
2. Criar uma branch para sua feature (`git checkout -b feature/MinhaFeature`)
3. Commit suas mudanças (`git commit -m 'Adiciona MinhaFeature'`)
4. Push para a branch (`git push origin feature/MinhaFeature`)
5. Abrir um Pull Request

### Diretrizes

- Mantenha o código limpo e bem documentado
- Adicione testes para novas funcionalidades
- Siga os padrões de código existentes
- Atualize a documentação quando necessário

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo `LICENSE` para mais detalhes.

## 👤 Autor

Desenvolvido com ❤️ por Deureck de Souza Passarela

---

## 📞 Suporte

Se você tiver alguma dúvida ou problema, por favor abra uma [issue](https://github.com/deureck/urlencutador/issues).

---

**⭐ Se este projeto foi útil para você, considere dar uma estrela!**
