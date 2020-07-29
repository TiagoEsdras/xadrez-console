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
                Tabuleiro tab = new Tabuleiro(8, 8);

                tab.ColocarPeca(new Torre(tab, Cor.preta), new Posicao(0, 1 ));
                tab.ColocarPeca(new Torre(tab, Cor.preta), new Posicao(1, 3));
                tab.ColocarPeca(new Rei(tab, Cor.branca), new Posicao(0, 2));

                Tela.ImprimirTabuleiro(tab);

                
            }
            catch (TabuleiroException e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine(); 
        }
    }
}
