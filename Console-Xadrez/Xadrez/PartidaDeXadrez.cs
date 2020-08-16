using System.Collections.Generic;
using System.Security.AccessControl;
using System.Security.Cryptography;
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
        public bool Xeque { get; private set; }
        public Peca VulneravelEnPassant { get; private set; }

        public PartidaDeXadrez()
        {
            Tab = new Tabuleiro(8, 8);
            Turno = 1;
            JogadorAtual = Cor.branca;
            Terminada = false;
            Xeque = false;
            VulneravelEnPassant = null;
            Pecas = new HashSet<Peca>();
            Capturadas = new HashSet<Peca>();
            ColocarPecas();
        }
        public Peca ExecutaMovimento(Posicao origem, Posicao destino)
        {
            Peca p = Tab.RetirarPeca(origem);
            p.IncrementarQteMovimentos();
            Peca pecaCapturada = Tab.RetirarPeca(destino);
            Tab.ColocarPeca(p, destino);
            if (pecaCapturada != null)
            {
                Capturadas.Add(pecaCapturada);
            }

            // # jogada especial roque pequeno
            if (p is Rei && destino.Coluna == origem.Coluna + 2)
            {
                Posicao origemT = new Posicao(origem.Linha, origem.Coluna + 3);
                Posicao destinoT = new Posicao(origem.Linha, origem.Coluna + 1);
                Peca T = Tab.RetirarPeca(origemT);
                T.IncrementarQteMovimentos();
                Tab.ColocarPeca(T, destinoT);
            }
            // # jogada especial roque grande
            if (p is Rei && destino.Coluna == origem.Coluna - 2)
            {
                Posicao origemT = new Posicao(origem.Linha, origem.Coluna - 4);
                Posicao destinoT = new Posicao(origem.Linha, origem.Coluna - 1);
                Peca T = Tab.RetirarPeca(origemT);
                T.IncrementarQteMovimentos();
                Tab.ColocarPeca(T, destinoT);
            }

            //# jogada especial en passant

            if (p is Peao)
            {
                if (origem.Coluna != destino.Coluna && pecaCapturada == null)
                {
                    Posicao posP;
                    if (p.Cor == Cor.branca)
                    {
                        posP = new Posicao(destino.Linha + 1, destino.Coluna);
                    }
                    else
                    {
                        posP = new Posicao(destino.Linha - 1, destino.Coluna);
                    }
                    pecaCapturada = Tab.RetirarPeca(posP);
                    Capturadas.Add(pecaCapturada);
                }
            }
            return pecaCapturada;
        }

        public void RealizaJogada(Posicao origem, Posicao destino)
        {

            if (EstaEmXeque(JogadorAtual))
            {
                Peca PecaCapturada = ExecutaMovimento(origem, destino);

                if (EstaEmXeque(JogadorAtual))
                {
                    DesfazMovimento(origem, destino, PecaCapturada);
                    throw new TabuleiroException("Você precisa sair do XEQUE!");
                }

            }
            else
            {
                Peca PecaCapturada = ExecutaMovimento(origem, destino);

                if (EstaEmXeque(JogadorAtual))
                {
                    DesfazMovimento(origem, destino, PecaCapturada);
                    throw new TabuleiroException("Você não pode se colocar em Xeque");
                }

            }

            Peca p = Tab.Peca(destino);

            //# jogada especial promocao

            if (p is Peao)
            {
                if ((p.Cor == Cor.branca && destino.Linha == 0) || (p.Cor == Cor.preta && destino.Linha == 8))
                {
                    p = Tab.RetirarPeca(destino);
                    Pecas.Remove(p);
                    Peca dama = new Dama(Tab, p.Cor);
                    Tab.ColocarPeca(dama, destino);
                    Pecas.Add(dama);
                    
                }
            }

            if (EstaEmXeque(Adversaria(JogadorAtual)))
            {
                Xeque = true;
            }
            else
            {
                Xeque = false;
            }

            if (TesteXequeMate(Adversaria(JogadorAtual)))
            {
                Terminada = true;
            }
            else
            {
                Turno++;
                MudaJogador();
            }            

            //# jogada especial en Passant 

            if (p is Peao && (destino.Linha == origem.Linha -2 || destino.Linha == origem.Linha + 2))
            {
                VulneravelEnPassant = p;
            }
            else
            {
                VulneravelEnPassant = null;
            }
        }

        public void DesfazMovimento(Posicao origem, Posicao destino, Peca pecaCapturada)
        {
            Peca p = Tab.RetirarPeca(destino);
            p.DecrementarQteMovimentos();
            if (pecaCapturada != null)
            {
                Tab.ColocarPeca(pecaCapturada, destino);
                Capturadas.Remove(pecaCapturada);
            }
            Tab.ColocarPeca(p, origem);

            // # jogada especial roque pequeno
            if (p is Rei && destino.Coluna == origem.Coluna + 2)
            {
                Posicao origemT = new Posicao(origem.Linha, origem.Coluna + 3);
                Posicao destinoT = new Posicao(origem.Linha, origem.Coluna + 1);
                Peca T = Tab.RetirarPeca(destinoT);
                T.DecrementarQteMovimentos();
                Tab.ColocarPeca(T, origemT);
            }
            // # jogada especial roque grande
            if (p is Rei && destino.Coluna == origem.Coluna - 2)
            {
                Posicao origemT = new Posicao(origem.Linha, origem.Coluna - 4);
                Posicao destinoT = new Posicao(origem.Linha, origem.Coluna - 1);
                Peca T = Tab.RetirarPeca(destinoT);
                T.IncrementarQteMovimentos();
                Tab.ColocarPeca(T, origemT);
            }
            // # jogasa especial en passant
            if (p is Peao)
            {
                if (origem.Coluna != destino.Coluna && pecaCapturada == VulneravelEnPassant)
                {
                    Peca peao = Tab.RetirarPeca(destino);
                    Posicao posP;
                    if (p.Cor == Cor.branca)
                    {
                        posP = new Posicao(3, destino.Coluna);
                    }
                    else
                    {
                        posP = new Posicao(4, destino.Coluna);
                    }
                    Tab.ColocarPeca(peao, posP);
                }
            }
        }
        public void ValidarPosicaoDeOrigem(Posicao pos)
        {
            if (Tab.Peca(pos) == null)
            {
                throw new TabuleiroException("Não existe peça na posição de origem escolhida!");
            }
            else if (JogadorAtual != Tab.Peca(pos).Cor)
            {
                throw new TabuleiroException("A peça de origem escolhida não é sua!");
            }
            else if (!Tab.Peca(pos).ExisteMovimentosPossiveis())
            {
                throw new TabuleiroException("Não existe movimentos possíveis para a peça de origem escolhida!");
            }
        }
        public void ValidarPosicaoDeDestino(Posicao origem, Posicao destino)
        {
            if (!Tab.Peca(origem).MovimentoPossivel(destino))
            {
                throw new TabuleiroException("Posição de destino inválida!");
            }
        }
        private void MudaJogador()
        {
            if (JogadorAtual == Cor.branca)
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

        private Cor Adversaria(Cor cor)
        {
            if (cor == Cor.branca)
            {
                return Cor.preta;
            }
            else
            {
                return Cor.branca;
            }
        }

        private Peca Rei(Cor cor)
        {
            foreach (Peca x in PecasEmJogo(cor))
            {
                if (x is Rei)
                {
                    return x;
                }
            }
            return null;
        }
        public bool EstaEmXeque(Cor cor)
        {
            Peca R = Rei(cor);

            if (R == null)
            {
                throw new TabuleiroException("Não tem rei da cor " + cor + "no tabuleiro!");
            }
            foreach (Peca x in PecasEmJogo(Adversaria(cor)))
            {
                bool[,] mat = x.MovimentosPossiveis();
                if (mat[R.Posicao.Linha, R.Posicao.Coluna])
                {
                    return true;
                }
            }
            return false;
        }

        public bool TesteXequeMate(Cor cor)
        {
            if (!EstaEmXeque(cor))
            {
                return false;
            }

            foreach (Peca x in PecasEmJogo(cor))
            {
                bool[,] mat = x.MovimentosPossiveis();
                for (int i = 0; i < Tab.Linhas; i++)
                {
                    for (int j = 0; j < Tab.Colunas; j++)
                    {
                        if (mat[i, j])
                        {
                            Posicao origem = x.Posicao;
                            Posicao destino = new Posicao(i, j);
                            Peca pecaCapturada = ExecutaMovimento(origem, destino);
                            bool testeXeque = EstaEmXeque(cor);
                            DesfazMovimento(origem, destino, pecaCapturada);
                            
                            if (!testeXeque)
                            {
                                return false;
                            }                            
                        }
                    }
                }
            }
            return true;
        }
        public void ColocarNovaPeca(char coluna, int linha, Peca peca)
        {
            Tab.ColocarPeca(peca, new PosicaoXadrez(coluna, linha).ToPosicao());
            Pecas.Add(peca);
        }

        private void ColocarPecas()
        {
            ColocarNovaPeca('a', 1, new Torre(Tab, Cor.branca));
            ColocarNovaPeca('b', 1, new Cavalo(Tab, Cor.branca));
            ColocarNovaPeca('c', 1, new Bispo(Tab, Cor.branca));
            ColocarNovaPeca('d', 1, new Dama(Tab, Cor.branca));
            ColocarNovaPeca('e', 1, new Rei(Tab, Cor.branca, this));
            ColocarNovaPeca('f', 1, new Bispo(Tab, Cor.branca));
            ColocarNovaPeca('g', 1, new Cavalo(Tab, Cor.branca));
            ColocarNovaPeca('h', 1, new Torre(Tab, Cor.branca));

            ColocarNovaPeca('a', 2, new Peao(Tab, Cor.branca, this));
            ColocarNovaPeca('b', 2, new Peao(Tab, Cor.branca, this));
            ColocarNovaPeca('c', 2, new Peao(Tab, Cor.branca, this));
            ColocarNovaPeca('d', 2, new Peao(Tab, Cor.branca, this));
            ColocarNovaPeca('e', 2, new Peao(Tab, Cor.branca, this));
            ColocarNovaPeca('f', 2, new Peao(Tab, Cor.branca, this));
            ColocarNovaPeca('g', 2, new Peao(Tab, Cor.branca, this));
            ColocarNovaPeca('h', 2, new Peao(Tab, Cor.branca, this));

            ColocarNovaPeca('a', 8, new Torre(Tab, Cor.preta));
            ColocarNovaPeca('b', 8, new Cavalo(Tab, Cor.preta));
            ColocarNovaPeca('c', 8, new Bispo(Tab, Cor.preta));
            ColocarNovaPeca('d', 8, new Dama(Tab, Cor.preta));
            ColocarNovaPeca('e', 8, new Rei(Tab, Cor.preta, this));
            ColocarNovaPeca('f', 8, new Bispo(Tab, Cor.preta));
            ColocarNovaPeca('g', 8, new Cavalo(Tab, Cor.preta));
            ColocarNovaPeca('h', 8, new Torre(Tab, Cor.preta));

            ColocarNovaPeca('a', 7, new Peao(Tab, Cor.preta, this));
            ColocarNovaPeca('b', 7, new Peao(Tab, Cor.preta, this));
            ColocarNovaPeca('c', 7, new Peao(Tab, Cor.preta, this));
            ColocarNovaPeca('d', 7, new Peao(Tab, Cor.preta, this));
            ColocarNovaPeca('e', 7, new Peao(Tab, Cor.preta, this));
            ColocarNovaPeca('f', 7, new Peao(Tab, Cor.preta, this));
            ColocarNovaPeca('g', 7, new Peao(Tab, Cor.preta, this));
            ColocarNovaPeca('h', 7, new Peao(Tab, Cor.preta, this));                   
            

        }
    }
}
