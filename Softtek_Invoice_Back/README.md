# Portal de Pedidos de Insumos - Backend API

Backend em .NET 8.0 para o portal corporativo de pedidos de insumos para uma grande rede de farmácias.

## 📋 Sobre o Projeto

Este projeto implementa uma API REST seguindo os princípios de Clean Architecture e Presentation-Domain-Data Layering (Martin Fowler). O sistema permite que colaboradores façam pedidos de insumos, com integração orientada a eventos e adaptador SAP simulado.

## 🏗️ Arquitetura

O projeto segue a arquitetura em camadas:

- **Presentation**: Controllers, DTOs, Validators
- **Domain**: Entities, Events, Interfaces, Services
- **Data**: DbContext, Repositories
- **Infrastructure**: Event Bus, SAP Adapter

### Princípios Aplicados

- **SOLID**: Separação de responsabilidades, inversão de dependência
- **Clean Code**: Código legível, testável e manutenível
- **Clean Architecture**: Dependências apontam para dentro (Domain no centro)

## 🚀 Tecnologias

- .NET 8.0
- Entity Framework Core 8.0
- SQLite (simulação de SQL Server para desenvolvimento)
- FluentValidation
- Swagger/OpenAPI
- xUnit (testes)

## 📦 Estrutura do Projeto

```
Softtek_Invoice_Back/
├── Domain/              # Camada de domínio
│   ├── Entities/        # Entidades de negócio
│   ├── Events/          # Eventos de domínio
│   ├── Interfaces/      # Contratos/Interfaces
│   └── Services/        # Serviços de domínio
├── Data/                # Camada de dados
│   ├── Repositories/    # Implementações de repositórios
│   └── ApplicationDbContext.cs
├── Infrastructure/      # Infraestrutura
│   ├── EventBus/        # Event bus in-memory
│   └── Sap/             # Adaptador SAP (mock)
├── Presentation/        # Camada de apresentação
│   ├── Controllers/     # Controllers REST
│   ├── DTOs/            # Data Transfer Objects
│   └── Validators/      # Validadores FluentValidation
└── Tests/               # Testes
    ├── Unit/            # Testes unitários
    └── Integration/     # Testes de integração
```

## 🔧 Configuração e Execução

### Pré-requisitos

- .NET 8.0 SDK
- Visual Studio 2022 ou VS Code

### Executar a Aplicação

```bash
# Restaurar pacotes
dotnet restore

# Executar a aplicação
dotnet run --project Softtek_Invoice_Back

# A API estará disponível em:
# https://localhost:5001 ou http://localhost:5000
# Swagger UI: https://localhost:5001/swagger
```

### Banco de Dados

O projeto está configurado para usar SQLite por padrão. O banco de dados será criado automaticamente na primeira execução (`orders.db`).

Para usar banco de dados em memória (útil para testes), descomente a linha no `Program.cs`:

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("OrdersDb"));
```

## 📡 Endpoints da API

### POST /api/orders

Cria um novo pedido de insumo.

**Request Body:**
```json
{
  "branchId": "BR001",
  "itemId": "ITEM001",
  "quantity": 10
}
```

**Response (201 Created):**
```json
{
  "success": true,
  "message": "Order created successfully",
  "data": {
    "id": "guid",
    "branchId": "BR001",
    "itemId": "ITEM001",
    "quantity": 10,
    "createdAt": "2024-01-01T00:00:00Z",
    "status": "Pending"
  }
}
```

## 🔄 Fluxo de Eventos

1. Cliente faz POST para `/api/orders`
2. `OrderService` cria o pedido no banco de dados
3. Evento `OrderCreatedEvent` é publicado no event bus
4. `SapAdapter` consome o evento automaticamente
5. `SapAdapter` chama o `SapApiClient` (mock) para simular integração SAP

## 🧪 Testes

### Executar Testes

```bash
# Executar todos os testes
dotnet test

# Executar testes com cobertura (requer coverlet)
dotnet test /p:CollectCoverage=true
```

### Estrutura de Testes

- **Testes Unitários**: Testam componentes isolados (Services, Adapters)
- **Testes de Integração**: Testam fluxos completos (Controllers, Repositories)

## 🔐 Autenticação

Para este MVP, a autenticação Azure AD B2C está simulada. Em produção, seria necessário:

1. Configurar Azure AD B2C no `Program.cs`
2. Adicionar middleware de autenticação
3. Proteger endpoints com `[Authorize]`

## 📝 Decisões Arquiteturais

Consulte o arquivo [ADRs.md](./ADRs.md) para detalhes sobre as decisões arquiteturais:

- ADR-001: App Service vs AKS
- ADR-002: Service Bus vs In-Memory Queue
- ADR-003: SQL Server vs PostgreSQL

## 🛠️ Uso de IA

Este projeto foi desenvolvido com assistência de IA (GitHub Copilot/Cursor). A IA foi utilizada para:

- Geração inicial de código seguindo padrões estabelecidos
- Sugestões de estrutura e organização
- Revisão e refatoração de código

Todo o código foi revisado e ajustado para seguir os princípios SOLID, Clean Code e Clean Architecture.

## 📄 Licença

Este é um projeto de teste/portfólio.

## 👥 Contribuindo

Este é um projeto de teste. Para melhorias ou sugestões, sinta-se à vontade para abrir issues ou pull requests.

