using System.Collections.Generic;
using System.Security.AccessControl;
using tabuleiro;

namespace xadrez
{
    class PartidaDeXadrez
    {
        public Tabuleiro Tab { get; private set; }
        public int Turno { get; private set; }
        public Cor JogadorAtual { get; private set; } 
        public bool Terminada { get; private set; }
        private HashSet<Peca> Pecas;
        private HashSet<Peca> Capturadas;

        public PartidaDeXadrez()
        {
            Tab = new Tabuleiro(8,8);
            Turno = 1;
            JogadorAtual = Cor.branca;
            Terminada = false;
            Pecas = new HashSet<Peca>();
            Capturadas = new HashSet<Peca>();
            ColocarPecas();
        }
        public void ExecutaMovimento (Posicao origem, Posicao destino)
        {
            Peca p = Tab.RetirarPeca(origem);
            p.IncrementarQteMovimentos();
            Peca pecaCapturada = Tab.RetirarPeca(destino);
            Tab.ColocarPeca(p, destino);
            if (pecaCapturada != null)
            {
                Capturadas.Add(pecaCapturada);
            }
        }

        public void RealizaJogada(Posicao origem, Posicao destino)
        {
            ExecutaMovimento(origem, destino);
            Turno++;
            MudaJogador();
        }
        public void ValidarPosicaoDeOrigem(Posicao pos)
        {
            if (Tab.Peca(pos) == null)
            {
                throw new TabuleiroException("Não existe peça na posição de origem escolhida!");
            }
            else if(JogadorAtual != Tab.Peca(pos).Cor)
            {
                throw new TabuleiroException("A peça de origem escolhida não é sua!");
            }
            else if (!Tab.Peca(pos).ExisteMovimentosPossiveis())
            {
                throw new TabuleiroException("Não existe movimentos possíveis para a peça de origem escolhida!");
            }
        }
        public void ValidarPosicaoDeDestino (Posicao origem, Posicao destino)
        {
            if (!Tab.Peca(origem).PodeMoverPara(destino))
            {
                throw new TabuleiroException("Posição de destino inválida!");
            }
        }
        private void MudaJogador()
        {
            if(JogadorAtual == Cor.branca)
            {
                JogadorAtual = Cor.preta;
            }
            else
            {
                JogadorAtual = Cor.branca;
            }
        }
        public HashSet<Peca> PecasCapturadas(Cor cor)
        {
            HashSet<Peca> aux = new HashSet<Peca>();
            foreach (Peca x in Capturadas)
            {
                if (x.Cor == cor)
                {
                    aux.Add(x);
                }
            }
            return aux;
        }

        public HashSet<Peca> PecasEmJogo(Cor cor)
        {
            HashSet<Peca> aux = new HashSet<Peca>();
            foreach (Peca x in Pecas)
            {
                if (x.Cor == cor)
                {
                    aux.Add(x);
                }
            }
            aux.ExceptWith(PecasCapturadas(cor));
            return aux;
        }

        public void ColocarNovaPeca(char coluna, int linha, Peca peca)
        {
            Tab.ColocarPeca(peca, new PosicaoXadrez(coluna, linha).ToPosicao());
            Pecas.Add(peca);
        }
        
        private void ColocarPecas()
        {
            ColocarNovaPeca('c', 1, new Torre(Tab, Cor.branca));
            ColocarNovaPeca('c', 2, new Torre(Tab, Cor.branca));
            ColocarNovaPeca('d', 2, new Torre(Tab, Cor.branca));
            ColocarNovaPeca('e', 2, new Torre(Tab, Cor.branca));
            ColocarNovaPeca('e', 1, new Torre(Tab, Cor.branca));
            ColocarNovaPeca('d', 1, new Rei(Tab, Cor.branca));


            ColocarNovaPeca('c', 7, new Torre(Tab, Cor.preta));
            ColocarNovaPeca('c', 8, new Torre(Tab, Cor.preta));
            ColocarNovaPeca('d', 7, new Torre(Tab, Cor.preta));
            ColocarNovaPeca('e', 7, new Torre(Tab, Cor.preta));
            ColocarNovaPeca('e', 8, new Torre(Tab, Cor.preta));
            ColocarNovaPeca('d', 8, new Rei(Tab, Cor.preta));

        }
    }
}
