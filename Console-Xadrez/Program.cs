using System;
using tabuleiro;
using xadrez;

namespace Console_Xadrez
{
    class Program
    {
        static void Main(string[] args)
        {
            PosicaoXadrez pos = new PosicaoXadrez('c', 7);
            Console.WriteLine(pos);
            Console.WriteLine(pos.ToPosicao());

            Console.ReadLine(); 
        }
    }
}
