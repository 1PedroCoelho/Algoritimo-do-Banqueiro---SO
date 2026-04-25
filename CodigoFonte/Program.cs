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
        static void ReleaseResources(int customer, int[] release)
    {
        lock (mutex)
        {
            for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
            {
                available[i] += release[i];
                allocation[customer, i] -= release[i];
                need[customer, i] += release[i];
            }

            Console.WriteLine($"Cliente {customer} liberou recursos.");
        }
    }
    static bool IsSafe() // Verifica se o estado atual é seguro
    {
        int[] work = new int[NUMBER_OF_RESOURCES];
        bool[] finish = new bool[NUMBER_OF_CUSTOMERS];

        Array.Copy(available, work, NUMBER_OF_RESOURCES);

        for (int i = 0; i < NUMBER_OF_CUSTOMERS; i++) // Inicializa finish 
            finish[i] = false;

        bool found; // Indica se um cliente foi encontrado para alocar recursos

        do
        {
            found = false;

            for (int i = 0; i < NUMBER_OF_CUSTOMERS; i++) // Verifica se o cliente i pode ser alocado com os recursos disponíveis
            {
                if (!finish[i])
                {
                    bool possible = true;

                    for (int j = 0; j < NUMBER_OF_RESOURCES; j++) // Verifica se a necessidade do cliente i pode ser satisfeita com os recursos disponíveis
                    {
                        if (need[i, j] > work[j])
                        {
                            possible = false;
                            break;
                        }
                    }

                    if (possible) // Se o cliente i pode ser alocado, simula a alocação e marca o cliente como finalizado
                    {
                        for (int j = 0; j < NUMBER_OF_RESOURCES; j++)
                        {
                            work[j] += allocation[i, j];
                        }

                        finish[i] = true;
                        found = true;
                    }
                }
            }

        } while (found); // Continua tentando alocar clientes enquanto encontrar um cliente que possa ser alocado

        for (int i = 0; i < NUMBER_OF_CUSTOMERS; i++) // Verifica se todos os clientes foram alocados
        {
            if (!finish[i]) // Se algum cliente não foi alocado, o estado não é seguro
                return false;
        }

        return true;
    }
}

