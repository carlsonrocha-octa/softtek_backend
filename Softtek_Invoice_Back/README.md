# Portal de Pedidos de Insumos - Backend API

Backend em .NET 8.0 para o portal corporativo de pedidos de insumos para uma grande rede de farmácias.

> **Projeto de Teste Técnico** - Vaga: Arquiteto Hands-on / Tech Lead (.NET + React)

## 📋 Sobre o Projeto

Este projeto implementa uma API REST seguindo os princípios de **Clean Architecture** e **Presentation-Domain-Data Layering** (Martin Fowler). O sistema permite que colaboradores façam pedidos de insumos, com integração orientada a eventos e adaptador SAP simulado.

### Contexto do Teste

Portal corporativo para uma grande rede de farmácias que permite:
- Colaboradores criarem pedidos de insumos
- Autenticação via Azure AD B2C (simulada)
- Integração em tempo real com SAP via arquitetura event-driven
- Deploy em ambiente Azure (App Service)

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

## 🚦 Como Rodar o Projeto

### Pré-requisitos

- **.NET 8.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Visual Studio 2022** ou **VS Code** (opcional)
- **Git** para clonar o repositório

### Passo a Passo

1. **Clone o repositório**
```bash
git clone <repository-url>
cd Softtek_Invoice_Back
```

2. **Restaurar dependências**
```bash
dotnet restore
```

3. **Executar a aplicação**
```bash
dotnet run
```

4. **Acessar a API**
- API: `https://localhost:7170` ou `http://localhost:5193`
- Swagger UI: `https://localhost:7170/swagger`
- Health Check: `https://localhost:7170/health`

### Executar Testes

```bash
# Todos os testes
dotnet test

# Com detalhes
dotnet test --logger "console;verbosity=detailed"

# Com cobertura (requer coverlet)
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
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

## 🔄 Fluxo de Eventos (Event-Driven Architecture)

1. Cliente faz `POST /api/orders` com dados do pedido
2. `OrdersController` valida entrada (FluentValidation)
3. `OrderService` cria o pedido no banco de dados (status: `Pending`)
4. Evento `OrderCreatedEvent` é publicado no `InMemoryEventBus`
5. `SapAdapter` consome o evento automaticamente
6. Status atualizado para `Processing`
7. `MockSapApiClient` simula chamada à API SAP (delay 100ms)
8. Se sucesso: status → `SentToSap` | Se erro: status → `Failed`
9. Cliente recebe resposta 201 Created

**Diagrama detalhado**: Veja [docs/sequence-diagram.md](./docs/sequence-diagram.md)

## 🧪 Testes

### Cobertura de Testes

#### Testes Unitários
- ✅ `OrderServiceTests`: Testa lógica de criação de pedidos e publicação de eventos
- ✅ `SapAdapterTests`: Testa processamento de eventos e integração SAP mock

#### Testes de Integração
- ✅ `OrderRepositoryIntegrationTests`: Testa operações de banco de dados
- ✅ `OrdersControllerIntegrationTests`: Testa endpoints REST end-to-end

### Casos de Teste Implementados

**OrderService:**
- Criação de pedido com dados válidos
- Publicação de evento após criação
- Propriedades corretas do pedido (ID, timestamps, status)

**SapAdapter:**
- Processamento de evento OrderCreated
- Chamada ao SAP API Client
- Propagação de exceções em caso de erro

**OrderRepository:**
- Persistência de pedidos no banco
- Busca por ID
- Listagem de todos os pedidos

**OrdersController:**
- Criação de pedido com payload válido (201 Created)
- Validação de campos obrigatórios (400 Bad Request)
- Validação de quantidade inválida (400 Bad Request)

## 📊 Diagramas de Arquitetura

### Diagrama C4 (Context + Container)
Visualização completa da arquitetura do sistema, containers e suas interações.

👉 **[Ver Diagrama C4 Completo](./docs/c4-diagram.md)**

### Diagrama de Sequência
Fluxo detalhado do processo de criação de pedido, desde a requisição HTTP até a integração SAP.

👉 **[Ver Diagrama de Sequência](./docs/sequence-diagram.md)**

## 🔐 Autenticação (Simulada)

⚠️ **IMPORTANTE**: Para este MVP, a autenticação Azure AD B2C **NÃO está implementada**.

### Status Atual
- Endpoints estão **abertos** (sem autenticação)
- Não há validação de tokens JWT
- Não há middleware de autenticação

### Implementação Futura (Produção)

Para implementar Azure AD B2C em produção:

1. **Configurar Azure AD B2C Tenant**
```bash
# Criar tenant no Azure Portal
# Registrar aplicação
# Configurar fluxos de usuário (sign-up, sign-in)
```

2. **Adicionar pacotes NuGet**
```bash
dotnet add package Microsoft.Identity.Web
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

3. **Configurar no Program.cs**
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"));

app.UseAuthentication();
app.UseAuthorization();
```

4. **Proteger endpoints**
```csharp
[Authorize]
[HttpPost]
public async Task<ActionResult> CreateOrder(...)
```

5. **Configurar appsettings.json**
```json
{
  "AzureAdB2C": {
    "Instance": "https://<tenant-name>.b2clogin.com/",
    "ClientId": "<client-id>",
    "Domain": "<tenant-name>.onmicrosoft.com",
    "SignUpSignInPolicyId": "B2C_1_signupsignin"
  }
}
```

## 📝 Decisões Arquiteturais (ADRs)

Consulte o arquivo **[ADRs.md](./docs/ADRs.md)** para detalhes completos sobre as decisões arquiteturais:

### ADR-001: Azure App Service vs AKS
**Decisão**: Azure App Service  
**Justificativa**: Simplicidade, custo-efetividade, velocidade de deploy para MVP  
**Trade-offs**: Menos flexibilidade que Kubernetes, mas adequado para escala inicial

### ADR-002: Service Bus/Event Grid vs In-Memory Queue
**Decisão**: Event Bus In-Memory para MVP  
**Justificativa**: Reduz custos e complexidade durante desenvolvimento  
**Caminho de Migração**: Interface `IEventBus` permite migração transparente para Azure Service Bus

### ADR-003: SQL Server vs PostgreSQL
**Decisão**: SQL Server (simulado com SQLite)  
**Justificativa**: Alinhamento com ecossistema Azure e contexto empresarial  
**Trade-offs**: Custo de licenciamento vs. integração nativa com Azure

---

## 🎯 Decisões-Chave do Projeto

### Arquitetura
- **Padrão**: Presentation-Domain-Data Layering (Martin Fowler)
- **Princípios**: SOLID, Clean Code, Dependency Inversion
- **Event-Driven**: Desacoplamento via eventos de domínio

### Tecnologias
- **.NET 8.0 LTS**: Versão mais recente com suporte de longo prazo
- **Entity Framework Core 8.0**: ORM moderno e performático
- **FluentValidation**: Validação declarativa e testável
- **xUnit + Moq + FluentAssertions**: Stack completo de testes

### Simulações (MVP)
- QLite**: Simula SQL Server sem necessidade de infraestrutura
- **InMemoryEventBus**: Simula Azure Service Bus para desenvolvimento local
- **MockSapApiClient**: Simula integração SAP sem dependências externas
- **Azure AD B2C**: Não implementado (endpoints abertos)

## 🤖 Uso de Inteligência Artificial

Este projeto foi desenvolvido com assistência de **IA Generativa** (Kiro AI Assistant). Abaixo está o detalhamento completo do uso de IA, conforme solicitado no teste:

### Abrangência do Uso de IA

#### 1. Estruturação Inicial do Projeto (80% IA)
- ✅ Criação da estrutura de pastas (Presentation/Domain/Data/Infrastructure)
- ✅ Definição de namespaces e organização de arquivos
- ✅ Configuração inicial do `Program.cs` com DI e middleware

#### 2. Implementação de Código (70% IA)
- ✅ **Entities**: `Order`, `OrderStatus` - gerados por IA
- ✅ **Interfaces**: `IOrderService`, `IOrderRepository`, `IEventBus`, `ISapApiClient` - gerados por IA
- ✅ **Services**: `OrderService` - lógica base gerada por IA, ajustada manualmente
- ✅ **Repositories**: `OrderRepository` - implementação CRUD gerada por IA
- ✅ **Controllers**: `OrdersController` - estrutura gerada por IA, validações ajustadas
- ✅ **DTOs**: `CreateOrderRequest`, `OrderResponse`, `ApiResponse` - gerados por IA
- ✅ **Validators**: `CreateOrderRequestValidator` - regras sugeridas por IA
- ✅ **Infrastructure**: `InMemoryEventBus`, `SapAdapter`, `MockSapApiClient` - gerados por IA

#### 3. Testes (90% IA)
- ✅ **Testes Unitários**: `OrderServiceTests`, `SapAdapterTests` - gerados por IA com Moq
- ✅ **Testes de Integração**: `OrderRepositoryIntegrationTests`, `OrdersControllerIntegrationTests` - gerados por IA
- ✅ Estrutura de arrange-act-assert sugerida por IA
- ✅ Casos de teste edge cases sugeridos por IA

#### 4. Documentação (95% IA)
- ✅ **README.md**: Estrutura e conteúdo inicial gerados por IA
- ✅ **docs/ADRs.md**: Decisões arquiteturais escritas por IA com base em contexto fornecido
- ✅ **Diagramas C4 e Sequência**: Gerados por IA em formato Mermaid
- ✅ XML comments no código: Gerados por IA

#### 5. Correções e Refatorações (60% IA)
- ✅ Correção de erros de compilação: Diagnóstico e solução por IA
- ✅ Movimentação de `ISapApiClient` para camada correta: Sugerido por IA
- ✅ Implementação de métodos faltantes (`GetOrder`, `UpdateOrderStatus`): Gerado por IA
- ✅ Adição de índices no banco de dados: Sugerido por IA
- ✅ Configuração de health checks: Implementado por IA

### Trabalho Manual (Desenvolvedor)

#### Decisões Estratégicas (100% Manual)
- ❌ Escolha de tecnologias (.NET 8, SQLite, FluentValidation)
- ❌ Decisões arquiteturais (App Service vs AKS, Service Bus vs In-Memory)
- ❌ Definição de escopo do MVP

#### Revisão e Validação (100% Manual)
- ❌ Revisão de todo código gerado por IA
- ❌ Validação de aderência aos princípios SOLID
- ❌ Testes manuais da API via Swagger
- ❌ Validação de compilação e execução

#### Ajustes Finos (50% Manual)
- ❌ Ajustes em namespaces e usings
- ❌ Configuração de CORS e connection strings
- ❌ Organização final de arquivos

### Ferramentas de IA Utilizadas

- **Kiro AI Assistant**: Assistente principal para geração de código, testes e documentação
- **Capacidades utilizadas**:
  - Geração de código C# seguindo padrões estabelecidos
  - Criação de testes unitários e de integração
  - Escrita de documentação técnica (README, ADRs, diagramas)
  - Sugestões de refatoração e melhorias arquiteturais
  - Diagnóstico e correção de erros de compilação

### Conclusão sobre Uso de IA

A IA foi fundamental para:
- ✅ **Acelerar desenvolvimento**: Redução de ~70% no tempo de implementação
- ✅ **Manter consistência**: Padrões uniformes em todo o código
- ✅ **Cobertura de testes**: Geração rápida de casos de teste abrangentes
- ✅ **Documentação completa**: README, ADRs e diagramas detalhados

**Porém, a supervisão humana foi essencial para**:
- ❌ Garantir qualidade e aderência aos requisitos
- ❌ Tomar decisões arquiteturais estratégicas
- ❌ Validar funcionamento e corretude do código

## ⚠️ Limitações e Próximos Passos

### Limitações Conhecidas (MVP)

#### Segurança
- ❌ **Autenticação não implementada**: Endpoints estão abertos sem validação de tokens
- ❌ **Autorização não implementada**: Sem controle de permissões por role
- ❌ **Rate limiting ausente**: API vulnerável a abuso/DDoS
- ❌ **HTTPS não obrigatório**: Aceita HTTP em desenvolvimento

#### Persistência
- ⚠️ **SQLite em produção**: Não recomendado para ambientes de alta carga
- ⚠️ **Sem migrations**: Banco criado via `EnsureCreated()` (não versionado)
- ⚠️ **Sem backup/recovery**: Dados podem ser perdidos

#### Event-Driven
- ⚠️ **Event bus in-memory**: Eventos perdidos em caso de restart
- ⚠️ **Sem retry policy**: Falhas na integração SAP não são reprocessadas
- ⚠️ **Sem dead-letter queue**: Eventos com falha são descartados
- ⚠️ **Processamento síncrono**: Não há paralelização real

#### Observabilidade
- ⚠️ **Logging básico**: Sem correlação de requisições (correlation ID)
- ⚠️ **Sem métricas**: Não há coleta de métricas de performance
- ⚠️ **Sem tracing distribuído**: Difícil debugar fluxos complexos
- ⚠️ **Sem alertas**: Falhas não geram notificações

#### Validação de Negócio
- ⚠️ **Sem validação de BranchId**: Não verifica se filial existe
- ⚠️ **Sem validação de ItemId**: Não verifica se item existe no catálogo
- ⚠️ **Sem validação de estoque**: Não verifica disponibilidade
- ⚠️ **Sem limites de quantidade**: Aceita qualquer valor positivo

#### Testes
- ⚠️ **Sem testes de carga**: Performance não validada
- ⚠️ **Sem testes E2E**: Integração frontend-backend não testada
- ⚠️ **Cobertura parcial**: Nem todos os cenários de erro cobertos

### Próximos Passos (Roadmap para Produção)

#### Fase 1: Segurança (Prioridade Alta)
- [ ] Implementar autenticação Azure AD B2C
- [ ] Adicionar autorização baseada em roles
- [ ] Implementar rate limiting (ex: AspNetCoreRateLimit)
- [ ] Forçar HTTPS em produção
- [ ] Adicionar validação de CORS mais restritiva

#### Fase 2: Persistência (Prioridade Alta)
- [ ] Migrar para Azure SQL Database
- [ ] Implementar EF Core Migrations
- [ ] Configurar backup automático
- [ ] Adicionar índices adicionais para performance
- [ ] Implementar soft delete para auditoria

#### Fase 3: Event-Driven (Prioridade Alta)
- [ ] Migrar para Azure Service Bus
- [ ] Implementar retry policy com exponential backoff
- [ ] Configurar dead-letter queue
- [ ] Adicionar idempotência no processamento de eventos
- [ ] Implementar event sourcing para auditoria completa

#### Fase 4: Observabilidade (Prioridade Média)
- [ ] Integrar Application Insights
- [ ] Adicionar correlation IDs em todas as requisições
- [ ] Implementar structured logging com Serilog
- [ ] Configurar dashboards de métricas
- [ ] Criar alertas para erros críticos

#### Fase 5: Validação de Negócio (Prioridade Média)
- [ ] Criar serviço de validação de filiais
- [ ] Criar serviço de catálogo de itens
- [ ] Implementar validação de estoque
- [ ] Adicionar regras de negócio (limites, aprovações)
- [ ] Implementar workflow de aprovação para pedidos grandes

#### Fase 6: Performance e Escalabilidade (Prioridade Baixa)
- [ ] Implementar cache (Redis) para consultas frequentes
- [ ] Adicionar paginação em listagens
- [ ] Otimizar queries com projeções
- [ ] Implementar CQRS para separar leitura/escrita
- [ ] Configurar auto-scaling no App Service

#### Fase 7: DevOps (Prioridade Baixa)
- [ ] Configurar CI/CD com Azure Pipelines
- [ ] Implementar blue-green deployment
- [ ] Adicionar testes de carga no pipeline
- [ ] Configurar ambientes (dev, staging, prod)
- [ ] Implementar feature flags

#### Fase 8: Testes (Prioridade Baixa)
- [ ] Aumentar cobertura de testes para 80%+
- [ ] Adicionar testes de contrato (Pact)
- [ ] Implementar testes de carga (k6, JMeter)
- [ ] Adicionar testes E2E com Playwright
- [ ] Implementar mutation testing

### Estimativa de Esforço

| Fase | Esforço | Prazo Estimado |
|------|---------|----------------|
| Fase 1: Segurança | 40h | 1 semana |
| Fase 2: Persistência | 24h | 3 dias |
| Fase 3: Event-Driven | 40h | 1 semana |
| Fase 4: Observabilidade | 32h | 4 dias |
| Fase 5: Validação | 40h | 1 semana |
| Fase 6: Performance | 40h | 1 semana |
| Fase 7: DevOps | 32h | 4 dias |
| Fase 8: Testes | 40h | 1 semana |
| **TOTAL** | **288h** | **~2 meses** |

---

## 📄 Licença

Este é um projeto de teste técnico para processo seletivo.

## 👤 Autor

Desenvolvido como parte do teste técnico para a vaga de **Arquiteto Hands-on / Tech Lead (.NET + React)** na Softtek.

---

## 📞 Contato

Para dúvidas sobre este projeto de teste, entre em contato através do processo seletivo.

