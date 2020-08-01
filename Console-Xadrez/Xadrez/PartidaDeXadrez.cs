﻿using System;
using tabuleiro;

namespace xadrez
{
    class PartidaDeXadrez
    {
        public Tabuleiro Tab { get; private set; }
        private int Turno;
        private Cor JogadorAtual; 
        public bool Terminada { get; private set; }

        public PartidaDeXadrez()
        {
            Tab = new Tabuleiro(8,8);
            Turno = 1;
            JogadorAtual = Cor.branca;
            Terminada = false;
            ColocarPecas();
        }
        public void ExecutaMovimento (Posicao origem, Posicao destino)
        {
            Peca p = Tab.RetirarPeca(origem);
            p.IncrementarQteMovimentos();
            Peca pecaCapturada = Tab.RetirarPeca(destino);
            Tab.ColocarPeca(p, destino);
        }
        private void ColocarPecas()
        {
            Tab.ColocarPeca(new Torre(Tab, Cor.branca), new PosicaoXadrez('c', 1).ToPosicao());
            Tab.ColocarPeca(new Torre(Tab, Cor.branca), new PosicaoXadrez('c', 2).ToPosicao());
            Tab.ColocarPeca(new Torre(Tab, Cor.branca), new PosicaoXadrez('d', 2).ToPosicao());
            Tab.ColocarPeca(new Torre(Tab, Cor.branca), new PosicaoXadrez('e', 2).ToPosicao());
            Tab.ColocarPeca(new Torre(Tab, Cor.branca), new PosicaoXadrez('e', 1).ToPosicao());
            Tab.ColocarPeca(new Rei(Tab, Cor.branca), new PosicaoXadrez('d', 1).ToPosicao());

            Tab.ColocarPeca(new Torre(Tab, Cor.preta), new PosicaoXadrez('c', 7).ToPosicao());
            Tab.ColocarPeca(new Torre(Tab, Cor.preta), new PosicaoXadrez('c', 8).ToPosicao());
            Tab.ColocarPeca(new Torre(Tab, Cor.preta), new PosicaoXadrez('d', 7).ToPosicao());
            Tab.ColocarPeca(new Torre(Tab, Cor.preta), new PosicaoXadrez('e', 7).ToPosicao());
            Tab.ColocarPeca(new Torre(Tab, Cor.preta), new PosicaoXadrez('e', 8).ToPosicao());
            Tab.ColocarPeca(new Rei(Tab, Cor.preta), new PosicaoXadrez('d', 8).ToPosicao());

        }
    }
}
