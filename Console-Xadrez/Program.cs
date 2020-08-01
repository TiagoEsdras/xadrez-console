using System;
using tabuleiro;
using xadrez;

namespace Console_Xadrez
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                PartidaDeXadrez partida = new PartidaDeXadrez();

                while (!partida.Terminada)
                {
                    Console.Clear();
                    Tela.ImprimirTabuleiro(partida.Tab);

                    Console.Write("Origem: ");
                    Posicao origem = Tela.LerPosicaoXadrez().ToPosicao();

                    Console.Write("Destino: ");
                    Posicao detino = Tela.LerPosicaoXadrez().ToPosicao();

                    partida.ExecutaMovimento(origem, detino);
                }
               

                
            }
            catch (TabuleiroException e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine(); 
        }
    }
}
