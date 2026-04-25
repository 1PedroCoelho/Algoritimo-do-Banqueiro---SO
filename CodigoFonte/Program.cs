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

