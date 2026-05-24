#!/bin/bash
echo "----------- Inicializando Recursos AWS Locais -----------"

# 1. Criar a fila SQS para as transações
echo "Criando fila SQS: transaction-events-queue"
awslocal sqs create-queue --queue-name transaction-events-queue

# 2. Criar a tabela DynamoDB para o histórico de transações
# A chave de partição (Hash Key) será o Id da transação (String)
echo "Criando tabela DynamoDB: TransactionsHistory"
awslocal dynamodb create-table \
    --table-name TransactionsHistory \
    --attribute-definitions AttributeName=Id,AttributeType=S \
    --key-schema AttributeName=Id,KeyType=HASH \
    --billing-mode PAY_PER_REQUEST

echo "----------- Recursos AWS Criados com Sucesso -----------"
