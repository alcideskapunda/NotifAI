# "NotifAI" - Sistema de Alertas de Transações

## NotifAI - Sistema de Alertas de Transações Económicas

O **NotifAI** é uma solução de backend de alta performance desenhada para processar transações financeiras e disparar notificações de alertas de forma assíncrona. O projeto foi construído para demonstrar a integração prática do ecossistema **.NET 10** com serviços de Cloud da **AWS**, utilizando engenharia de software moderna.

## 🏗️ Arquitetura do Sistema

O sistema é dividido em dois microsserviços principais utilizando o padrão de arquitetura orientada a eventos (EDA):

1. **Web API (.NET 10)**: Endpoint REST que recebe os dados da transação, faz a validação de dados, persiste o histórico no banco de dados e publica um evento na fila de mensageria.
2. **Worker Service (.NET 10)**: Processador de segundo plano (Background Service) que consome as mensagens da fila de forma assíncrona e simula o processamento e envio do alerta.

### Serviços Cloud Utilizados (Simulados via LocalStack):

- **AWS DynamoDB**: Banco de dados NoSQL utilizado para armazenamento persistente e de baixa latência das transações.
- **AWS SQS (Simple Queue Service)**: Fila de mensageria utilizada para desacoplar a API do processador de notificações, garantindo resiliência e tolerância a falhas.

---

## 🛠️ Tecnologias e Conceitos Aplicados

- **C# 14 & .NET 10** (Controllers API's, Worker Services)
- **Programação Assíncrona** (Async/Await avançado)
- **Injeção de Dependência Nativa**
- **AWS SDK para .NET** (AWSSDK.DynamoDBv2, AWSSDK.SQS)
- **LocalStack & Docker** (Para desenvolvimento e testes locais 100% gratuitos)
- **FluentValidation** (Para garantir a integridade das transações recebidas)

---

## 🚀 Como Executar o Projeto Localmente

### Pré-requisitos

- [.NET 10 SDK](https://microsoft.com)
- [Docker Desktop](https://docker.com)

### 1. Subir o Ambiente AWS Local (LocalStack)

Na raiz do projeto, execute o Docker Compose para iniciar o DynamoDB e o SQS localmente:

```bash
docker compose up -d
```

### 2. Executar a Web API

```bash
cd src/NotifAI.API
dotnet run
```

### 3. Executar o Worker (Processador)

```bash
cd src/NotifAI.Worker
dotnet run
```
