# Algoritmo do Banqueiro — C#

Implementação do Banker's Algorithm em C# usando threads. O programa simula 5 clientes competindo por 3 tipos de recursos, garantindo que o sistema nunca entre em deadlock.

## Como funciona

Cada cliente roda em uma thread separada e fica em loop: pede recursos, usa por um tempo e libera. Antes de conceder qualquer pedido, o sistema verifica se o estado continua **seguro** (ou seja, se todos os clientes ainda conseguem terminar). Se não for seguro, o pedido é negado e a alocação é revertida.

As principais estruturas são:
- `available` — recursos disponíveis no momento
- `maximum` — o máximo que cada cliente pode pedir
- `allocation` — o que cada cliente tem alocado agora
- `need` — o que cada cliente ainda pode precisar (maximum - allocation)

## Como rodar

Você precisa ter o [.NET SDK](https://dotnet.microsoft.com/download) instalado.

```bash
# Cria o projeto
dotnet new console -n BankersAlgorithm
cd BankersAlgorithm
```

Cole o código no arquivo `Program.cs`, depois rode passando a quantidade de cada recurso como argumento:

```bash
dotnet run -- 10 5 7
```

O programa roda indefinidamente. Para parar, use `Ctrl + C`.

## Saída esperada

```
Cliente 2 recebeu recursos.
Cliente 0 recebeu recursos.
Cliente 2 liberou recursos.
Cliente 4 recebeu recursos.
...
```
