# Diagrama C4 - Portal de Pedidos de Insumos

## Nível 1: Diagrama de Contexto

```mermaid
graph TB
    User[👤 Colaborador da Farmácia<br/>Usuário do Sistema]
    
    System[🏢 Portal de Pedidos de Insumos<br/>Sistema Web para gestão de pedidos]
    
    SAP[📦 SAP ERP<br/>Sistema de gestão empresarial]
    
    ADB2C[🔐 Azure AD B2C<br/>Serviço de autenticação]
    
    User -->|Faz login e cria pedidos| System
    System -->|Autentica usuários| ADB2C
    System -->|Envia pedidos via API| SAP
    SAP -->|Confirma processamento| System
    
    style System fill:#1168bd,stroke:#0b4884,color:#ffffff
    style User fill:#08427b,stroke:#052e56,color:#ffffff
    style SAP fill:#999999,stroke:#666666,color:#ffffff
    style ADB2C fill:#999999,stroke:#666666,color:#ffffff
```

### Descrição

**Portal de Pedidos de Insumos**: Sistema web corporativo que permite colaboradores de farmácias criarem pedidos de insumos. O sistema autentica usuários via Azure AD B2C e integra com SAP ERP para processamento dos pedidos.

**Usuários**: Colaboradores das farmácias (gerentes, farmacêuticos, assistentes) que precisam solicitar insumos para suas filiais.

**Sistemas Externos**:
- **Azure AD B2C**: Gerencia autenticação e autorização de usuários
- **SAP ERP**: Processa e gerencia pedidos de insumos no sistema corporativo

---

## Nível 2: Diagrama de Container

```mermaid
graph TB
    subgraph Browser["🌐 Navegador Web"]
        SPA[Single Page Application<br/>React + Next.js + TypeScript<br/>Interface do usuário]
    end
    
    subgraph Azure["☁️ Microsoft Azure"]
        subgraph AppService["Azure App Service"]
            API[API REST<br/>.NET 8.0 Web API<br/>Endpoints REST + Swagger]
        end
        
        subgraph Database["Azure SQL Database"]
            DB[(SQL Server<br/>Armazena pedidos)]
        end
        
        ADB2C[Azure AD B2C<br/>Autenticação OAuth 2.0]
    end
    
    subgraph External["🏢 Sistemas Externos"]
        SAPAPI[SAP API<br/>REST API<br/>Integração ERP]
    end
    
    User[👤 Colaborador] -->|HTTPS| SPA
    SPA -->|JSON/HTTPS<br/>POST /api/orders| API
    SPA -->|OAuth 2.0| ADB2C
    API -->|Entity Framework Core<br/>SQL Queries| DB
    API -->|Event-Driven<br/>HTTP/JSON| SAPAPI
    API -->|Valida tokens| ADB2C
    
    style SPA fill:#1168bd,stroke:#0b4884,color:#ffffff
    style API fill:#1168bd,stroke:#0b4884,color:#ffffff
    style DB fill:#1168bd,stroke:#0b4884,color:#ffffff
    style ADB2C fill:#999999,stroke:#666666,color:#ffffff
    style SAPAPI fill:#999999,stroke:#666666,color:#ffffff
    style User fill:#08427b,stroke:#052e56,color:#ffffff
```

### Descrição dos Containers

#### 1. Single Page Application (Frontend)
- **Tecnologia**: React 18+ com Next.js 14+ e TypeScript
- **Responsabilidade**: Interface do usuário, formulários, validação client-side
- **Comunicação**: Consome API REST via HTTPS/JSON

#### 2. API REST (Backend)
- **Tecnologia**: .NET 8.0 (C#) com ASP.NET Core Web API
- **Responsabilidade**: 
  - Lógica de negócio
  - Validação de dados (FluentValidation)
  - Orquestração de eventos
  - Integração com SAP
- **Arquitetura**: Presentation-Domain-Data Layering
- **Comunicação**: 
  - Recebe requisições HTTP do frontend
  - Persiste dados no SQL Server
  - Publica eventos para integração SAP
  - Valida tokens JWT do Azure AD B2C

#### 3. SQL Server Database
- **Tecnologia**: Azure SQL Database (simulado com SQLite no MVP)
- **Responsabilidade**: Persistência de pedidos, histórico, status
- **Acesso**: Via Entity Framework Core

#### 4. Azure AD B2C
- **Tecnologia**: Serviço gerenciado Microsoft
- **Responsabilidade**: Autenticação e autorização de usuários
- **Protocolo**: OAuth 2.0 / OpenID Connect
- **Status MVP**: Simulado (não implementado)

#### 5. SAP API
- **Tecnologia**: REST API (sistema externo)
- **Responsabilidade**: Processar pedidos no ERP corporativo
- **Status MVP**: Mock/Simulado

---

## Arquitetura Interna da API (.NET)

```mermaid
graph TB
    subgraph API[".NET 8.0 Web API"]
        subgraph Presentation["📱 Presentation Layer"]
            Controllers[Controllers<br/>OrdersController]
            DTOs[DTOs<br/>Request/Response Models]
            Validators[Validators<br/>FluentValidation]
        end
        
        subgraph Domain["🎯 Domain Layer"]
            Services[Services<br/>OrderService]
            Entities[Entities<br/>Order, OrderStatus]
            Events[Events<br/>OrderCreatedEvent]
            Interfaces[Interfaces<br/>IOrderService, IOrderRepository<br/>IEventBus, ISapApiClient]
        end
        
        subgraph Data["💾 Data Layer"]
            Repositories[Repositories<br/>OrderRepository]
            DbContext[DbContext<br/>ApplicationDbContext]
        end
        
        subgraph Infrastructure["🔧 Infrastructure Layer"]
            EventBus[Event Bus<br/>InMemoryEventBus]
            SapAdapter[SAP Adapter<br/>SapAdapter]
            SapClient[SAP Client<br/>MockSapApiClient]
        end
    end
    
    Controllers --> Services
    Controllers --> DTOs
    Controllers --> Validators
    Services --> Interfaces
    Services --> Entities
    Services --> Events
    Repositories --> Interfaces
    Repositories --> DbContext
    EventBus --> Interfaces
    SapAdapter --> Interfaces
    SapClient --> Interfaces
    
    style Presentation fill:#e3f2fd,stroke:#1976d2,color:#000000
    style Domain fill:#fff3e0,stroke:#f57c00,color:#000000
    style Data fill:#f3e5f5,stroke:#7b1fa2,color:#000000
    style Infrastructure fill:#e8f5e9,stroke:#388e3c,color:#000000
```

### Camadas e Responsabilidades

#### Presentation Layer
- **Controllers**: Recebem requisições HTTP, delegam para services
- **DTOs**: Modelos de transferência de dados (Request/Response)
- **Validators**: Validação de entrada com FluentValidation

#### Domain Layer (Núcleo)
- **Services**: Lógica de negócio, orquestração
- **Entities**: Modelos de domínio (Order, OrderStatus)
- **Events**: Eventos de domínio (OrderCreatedEvent)
- **Interfaces**: Contratos que definem dependências

#### Data Layer
- **Repositories**: Implementação de acesso a dados
- **DbContext**: Contexto do Entity Framework Core

#### Infrastructure Layer
- **Event Bus**: Implementação de mensageria (in-memory para MVP)
- **SAP Adapter**: Adaptador para integração externa
- **SAP Client**: Cliente HTTP mock para simular SAP

### Princípios Aplicados

✅ **Dependency Inversion**: Domain não depende de Infrastructure/Data  
✅ **Single Responsibility**: Cada camada tem responsabilidade única  
✅ **Open/Closed**: Extensível via interfaces  
✅ **Interface Segregation**: Interfaces específicas e coesas  
✅ **Liskov Substitution**: Implementações substituíveis via DI

---

## Fluxo de Deploy (Futuro - Produção)

```mermaid
graph LR
    Dev[👨‍💻 Desenvolvedor] -->|git push| Repo[GitHub/Azure DevOps]
    Repo -->|trigger| Pipeline[Azure Pipelines<br/>CI/CD]
    Pipeline -->|build + test| Build[Build Artifacts]
    Build -->|deploy| AppService[Azure App Service]
    AppService -->|conecta| SQLDB[(Azure SQL Database)]
    AppService -->|publica eventos| ServiceBus[Azure Service Bus]
    ServiceBus -->|consome| Function[Azure Function<br/>SAP Integration]
    Function -->|HTTP| SAP[SAP API]
    
    style Dev fill:#08427b,stroke:#052e56,color:#ffffff
    style AppService fill:#1168bd,stroke:#0b4884,color:#ffffff
    style SQLDB fill:#1168bd,stroke:#0b4884,color:#ffffff
    style ServiceBus fill:#1168bd,stroke:#0b4884,color:#ffffff
    style Function fill:#1168bd,stroke:#0b4884,color:#ffffff
```

### Estratégia de Deploy

**MVP (Atual)**:
- Deploy manual ou via Visual Studio
- SQLite local
- Event bus in-memory

**Produção (Futuro)**:
- CI/CD com Azure Pipelines
- Azure App Service com auto-scaling
- Azure SQL Database
- Azure Service Bus para eventos
- Azure Functions para integração SAP
- Application Insights para monitoramento

---

## Decisões Arquiteturais

Para detalhes completos sobre decisões técnicas, consulte [ADRs.md](./ADRs.md):

- **ADR-001**: Azure App Service escolhido por simplicidade e custo-efetividade
- **ADR-002**: Event Bus in-memory para MVP, migração futura para Service Bus
- **ADR-003**: SQL Server por alinhamento com ecossistema Azure e contexto empresarial
