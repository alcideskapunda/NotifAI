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

## 🛡️ Resiliência e Tratamento de Erros

Para garantir que o sistema seja tolerante a falhas (padrão de produção), o Worker foi desenhado com as seguintes premissas:

- **Garantia de Descarte Prévio**: O processamento de mensagens possui tratamento contra `JsonException` e payloads nulos. Caso uma mensagem corrompida chegue à fila, o sistema regista o erro no log e remove-a da fila, evitando loops infinitos de processamento (_Poison Messages_).
- **Idempotência e Polling**: O Worker utiliza _Long Polling_ (`WaitTimeSeconds = 20`) para otimizar o consumo de CPU e chamadas de rede ao LocalStack/SQS, aguardando que novas mensagens fiquem disponíveis de forma eficiente.

---

## 🧪 Exemplo de Payload para Teste (Postman / cURL)

Para testar o endpoint `POST /api/transactions`, utilize a seguinte estrutura JSON:

```json
{
  "id": "tx_9b1deb4d-3b7d-4bad-9bdd-2b0d7b3dcb6d",
  "clientId": "cli_77fa2843-1102-4ef8-bc10-ef54c2565ebd",
  "amount": 250.75,
  "description": "Pagamento de Licença de Software",
  "createdAt": "2026-05-24T11:00:00Z"
}
```
