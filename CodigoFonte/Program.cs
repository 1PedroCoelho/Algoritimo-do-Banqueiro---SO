using System;
using System.Threading;

class BankersAlgorithm // Algoritmo do Banqueiro
{
    const int NUMBER_OF_CUSTOMERS = 5;
    const int NUMBER_OF_RESOURCES = 3;

    static int[] available = new int[NUMBER_OF_RESOURCES];
    static int[,] maximum = new int[NUMBER_OF_CUSTOMERS, NUMBER_OF_RESOURCES];
    static int[,] allocation = new int[NUMBER_OF_CUSTOMERS, NUMBER_OF_RESOURCES];
    static int[,] need = new int[NUMBER_OF_CUSTOMERS, NUMBER_OF_RESOURCES];

    static readonly object mutex = new object();
    static Random rand = new Random();

    static void Main(string[] args) // args: recursos disponíveis (ex: 10 5 7)
    {
        // Inicializa recursos disponíveis (ex: 10 5 7)
        for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
        {
            available[i] = int.Parse(args[i]);
        }

        // Inicializa maximum com valores aleatórios
        for (int i = 0; i < NUMBER_OF_CUSTOMERS; i++)
        {
            for (int j = 0; j < NUMBER_OF_RESOURCES; j++)
            {
                maximum[i, j] = rand.Next(1, available[j] + 1);
                allocation[i, j] = 0;
                need[i, j] = maximum[i, j];
            }
        }
        // Cria threads (clientes)
        for (int i = 0; i < NUMBER_OF_CUSTOMERS; i++)
        {
            int customerId = i;
            new Thread(() => CustomerThread(customerId)).Start();
        }
    }
    static void CustomerThread(int customerId)
    {
        while (true)
        {
            Thread.Sleep(rand.Next(1000, 3000));

            int[] request = new int[NUMBER_OF_RESOURCES];

            for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
            {
                request[i] = rand.Next(0, need[customerId, i] + 1);
            }

            if (RequestResources(customerId, request) == 0)
            {
                Thread.Sleep(rand.Next(1000, 3000));

                int[] release = new int[NUMBER_OF_RESOURCES];

                for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
                {
                    release[i] = rand.Next(0, allocation[customerId, i] + 1);
                }

                ReleaseResources(customerId, release);
            }
        }
    }
    static int RequestResources(int customer, int[] request) // Retorna 0 se alocação bem-sucedida, -1 caso contrário
    {
        lock (mutex) // Garante exclusão mútua
        {
            // Regra 1: request <= need
            for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
            {
                if (request[i] > need[customer, i])
                    return -1;
            }

            // Regra 2: request <= available
            for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
            {
                if (request[i] > available[i])
                    return -1;
            }

            // Simula alocação
            for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
            {
                available[i] -= request[i];
                allocation[customer, i] += request[i];
                need[customer, i] -= request[i];
            }

            // Verifica estado seguro
            if (!IsSafe())
            {
                // rollback
                for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
                {
                    available[i] += request[i];
                    allocation[customer, i] -= request[i];
                    need[customer, i] += request[i];
                }

                return -1;
            }

            Console.WriteLine($"Cliente {customer} recebeu recursos."); // Log de alocação, mensagem de sucesso
            return 0;
        }
    }