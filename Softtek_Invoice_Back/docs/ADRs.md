# Registros de Decisões Arquiteturais (ADRs)

Este documento contém as decisões arquiteturais tomadas para o projeto backend do Portal de Pedidos de Insumos para Farmácias.

## ADR-001: App Service vs AKS (Azure Kubernetes Service)

**Status:** Aceito  
**Data:** 2024  
**Contexto:** Precisamos decidir entre Azure App Service e Azure Kubernetes Service (AKS) para hospedar a API .NET.

**Decisão:** Usar **Azure App Service** para este MVP.

**Justificativa:**
- **Simplicidade**: App Service fornece uma plataforma gerenciada com configuração mínima necessária. Para um MVP, isso reduz a complexidade operacional.
- **Custo-efetividade**: App Service tem custos operacionais menores comparado ao AKS, que requer gerenciamento de cluster, pools de nós e monitoramento adicional.
- **Velocidade de deploy**: Ciclos de deploy mais rápidos com a integração CI/CD integrada do App Service.
- **Escalabilidade**: App Service fornece capacidades de escalonamento automático suficientes para as necessidades do MVP sem a complexidade da orquestração Kubernetes.
- **Tamanho da equipe**: Para uma equipe pequena e focada, App Service reduz a necessidade de expertise em Kubernetes.

**Trade-offs:**
- Menos flexibilidade comparado ao AKS para necessidades complexas de orquestração de containers.
- Limitado às restrições da plataforma Microsoft.
- Para crescimento futuro, migração para AKS pode ser considerada se orquestração de containers se tornar necessária.

**Consequências:**
- Tempo de mercado mais rápido para o MVP.
- Redução da sobrecarga de gerenciamento de infraestrutura.
- Onboarding mais fácil para desenvolvedores.
- Pode exigir caminho de migração se orquestração de containers se tornar crítica posteriormente.

---

## ADR-002: Service Bus/Event Grid vs Fila In-Memory para MVP

**Status:** Aceito  
**Data:** 2024  
**Contexto:** Precisamos escolher entre Azure Service Bus/Event Grid e um event bus em memória para a integração orientada a eventos do MVP.

**Decisão:** Usar **Event Bus In-Memory** para o MVP, com abstração clara para migração futura.

**Justificativa:**
- **Escopo do MVP**: O MVP é uma prova de conceito que não requer filas de mensagens de nível de produção inicialmente.
- **Redução de custos**: Elimina custos do Azure Service Bus durante desenvolvimento e testes iniciais.
- **Simplicidade**: Implementação em memória permite desenvolvimento e testes mais rápidos sem dependências externas.
- **Camada de abstração**: A interface `IEventBus` fornece uma abstração limpa, tornando a migração futura para Azure Service Bus direta.
- **Desenvolvimento local**: Desenvolvedores podem executar a aplicação sem configuração de infraestrutura Azure.

**Trade-offs:**
- **Sem persistência**: Eventos são perdidos se a aplicação falhar (aceitável para MVP).
- **Sem processamento distribuído**: Limitado ao processamento de instância única (suficiente para escala do MVP).
- **Sem entrega garantida**: Sem mecanismos de retry ou dead-letter queues (aceitável para MVP).

**Caminho de Migração:**
- A interface `IEventBus` permite substituição sem problemas com implementação Azure Service Bus.
- Implementação futura pode usar `ServiceBusClient` e `ServiceBusSender` mantendo a mesma interface.
- Handlers de eventos permanecem inalterados durante a migração.

**Consequências:**
- Ciclo de desenvolvimento mais rápido.
- Custos de infraestrutura menores durante a fase MVP.
- Caminho de migração fácil quando requisitos de escala emergirem.
- Requer design cuidadoso para manter limites de abstração.

---

## ADR-003: SQL Server vs PostgreSQL

**Status:** Aceito  
**Data:** 2024  
**Contexto:** Precisamos escolher entre SQL Server e PostgreSQL para o banco de dados.

**Decisão:** Usar **SQL Server** (simulado com SQLite/InMemory para MVP).

**Justificativa:**
- **Ecossistema Azure**: SQL Server se integra perfeitamente com serviços Azure (Azure SQL Database, Managed Instance).
- **Alinhamento empresarial**: O cliente (grande rede de farmácias) provavelmente possui infraestrutura e expertise existente em SQL Server.
- **Integração .NET**: Suporte nativo no Entity Framework Core com provedores otimizados.
- **Performance**: SQL Server oferece excelente performance para cargas de trabalho empresariais.
- **Ferramentas**: Ecossistema rico de ferramentas (SSMS, Azure Data Studio) familiar para equipes empresariais.
- **Simulação MVP**: SQLite fornece sintaxe compatível com SQL Server para desenvolvimento local, tornando a migração direta.

**Trade-offs:**
- **Custo**: Licenciamento SQL Server pode ser mais caro que PostgreSQL (mitigado pelo modelo pay-as-you-go do Azure SQL Database).
- **Lock-in de plataforma**: Mais centrado em Microsoft comparado à natureza cross-platform do PostgreSQL.
- **Open source**: PostgreSQL é totalmente open-source, enquanto SQL Server tem considerações de licenciamento.

**Consideração da Alternativa PostgreSQL:**
- PostgreSQL ofereceria custos de licenciamento menores.
- Melhor suporte cross-platform.
- Comunidade open-source forte.
- No entanto, integração com ecossistema Azure e alinhamento empresarial favorecem SQL Server para este projeto.

**Consequências:**
- Melhor alinhamento com infraestrutura Azure.
- Integração mais fácil com sistemas empresariais existentes.
- Stack tecnológico familiar para equipes empresariais.
- Custos potencialmente maiores comparado ao PostgreSQL (aceitável dado o contexto empresarial).

---

## Resumo

Estas decisões priorizam:
1. **Entrega rápida do MVP** com complexidade mínima de infraestrutura
2. **Custo-efetividade** durante a fase de desenvolvimento
3. **Caminhos de migração claros** para escalonamento em produção
4. **Alinhamento empresarial** com ecossistema Azure e SQL Server
5. **Manutenibilidade** através de abstrações limpas e princípios SOLID

Todas as decisões mantêm flexibilidade para evolução futura conforme requisitos e escala crescem.

