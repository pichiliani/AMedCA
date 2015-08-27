using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports; 	// necessário para ter acesso as portas  
using System.Diagnostics;


namespace SensorSomSerialCSharp
{
    public partial class Form1 : Form
    {
        #region Variables
        float ix = 100;
        #endregion

        
        #region Plot Variables
        double[] data_array_X = new double[100];
        double[] data_array_Y = new double[100];
        double[] Kalman_array_Y = new double[100];
        double[] Kalman_array_X = new double[100];

        double[] Media_Y = new double[100];
        double[] Media_X = new double[100];
        
        
        Random rand = new Random();
        #endregion

        #region VariaviesModeloClassificacao


        const int MAX_TREINAMENTO = 3; // Qtd de janelas no treinamento
        const int MAX_MEDIDAS =  20; //  (40 medidas = 4 segundos de gravação de dados)

        const int MAX_MEDIDAS_CLASSIFICACAO = 20; //  (20 medidas = 2 segundos de gravação de dados PARA CLASSIFICAÇÃO)
        

        // Janela de treinamento: 2 segundos ( = 20 medidas, pois 1 medida a cada 100ms)
        //  5 janelas de treinamento para pegar a media e depois comparar
        double[,] TreinamentoTornFechada = new double[MAX_TREINAMENTO, MAX_MEDIDAS];
        double[,] TreinamentoTornAberta = new double[MAX_TREINAMENTO, MAX_MEDIDAS];

        double media_fechada = 0;          // Aqui vou guardar o valor considerando a partir da segunda casa decimal
        double media_fechada_completa = 0; // Aqui vou guardar o valor da media real

        double media_aberta = 0;
        


        // Somente uma janela para classificar
        double[] ClassificaEstadoTorneira = new double[MAX_MEDIDAS];

        int contador_treinamento = 0; // vai de 1 a MAX_MEDIDAS
        int qtd_treinamento = 1; // vai de 1 a MAX_TREINAMENTO

        int contador_classifica; // vai de 1 a MAX_MEDIDAS

        #endregion

        #region Timers

        Timer GraphTimer = new Timer();
        
        #endregion

        SerialPort serialPort1;

        Stopwatch stopwatch = new Stopwatch();

        // Construtor da classe
        #region Form1()
        public Form1()
        {
            InitializeComponent();

            SetupDataChart();
            SetupChart();
            SetupArduino("COM7");

            SetupDataClassificacao();
            
            InitialiseTimers();

        }
        #endregion

        // Abridnoa conexão com o Arduino/placa da FRDM
        #region SetupArduino(String porta)
        private void SetupArduino(String porta)
        {
            try
            {
                serialPort1 = new SerialPort();
                serialPort1.PortName = porta;
                serialPort1.BaudRate = 9600;
                serialPort1.ReadTimeout = 2000;
                serialPort1.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }


            /*
            while (true)
            {

                string POT = serialPort1.ReadExisting();

                Console.WriteLine(POT);

                Thread.Sleep(100);

            }
             */
        }
        #endregion

        // Setando os arrays dos dados para o gráfico
        #region SetupDataChart()
        private void SetupDataChart()
        {
            //set up x data for initial condition and scrolling
            data_array_X[0] = 100;
            for (int i = 1; i < data_array_X.Length; i++)
            {
                data_array_X[i] = i;
                Kalman_array_X[i] = i;
                Media_X[i] = i;
            }
        }
        #endregion

        // Zerando os arrays de treinamento (torneira fechada/aberta)
        #region SetupDataClassificacao()
        private void SetupDataClassificacao()
        {
            for (int i = 0; i < MAX_TREINAMENTO; i++)
            {
                
                for (int j = 0; j < MAX_MEDIDAS; j++)
                {
                    TreinamentoTornFechada[i, j] = 0;
                    TreinamentoTornAberta[i, j] = 0;
                }
            }
        }
        #endregion


        // Inicializo os eixo e seto os valores máximos e mínimos
        #region SetupChart()
        public void SetupChart()
        {
            //Data 
            //Type and colour

            chart1.Series[0].Label = "TheData";
            chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            chart1.Series[0].Color = Color.Blue;

            //Axis
            chart1.ChartAreas[0].AxisX.Title = "X axis";
            chart1.ChartAreas[0].AxisY.Title = "Y axis";
            // chart1.ChartAreas[0].AxisY.Minimum = 0;
            // chart1.ChartAreas[0].AxisY.Maximum = 3500;
            
            // O intervalo do sensor está indo de 0.0 a 1.0
            // Um intervalo bom para pegar os dados do sensor piezo e ver a variação  no cano é 0.71 a 0.73
            // isso dependete onde está o sensor! Estes dados foram perto do segundo cotovelo

            // chart1.ChartAreas[0].AxisY.Minimum = 0.7;
            // chart1.ChartAreas[0].AxisY.Maximum = 0.75;

            chart1.ChartAreas[0].AxisY.Minimum = 0.68;
            chart1.ChartAreas[0].AxisY.Maximum = 0.76;


            //chart1.ChartAreas[0].AxisY


            // chart1.ChartAreas[0].AxisX.Minimum = 0;
            //chart1.ChartAreas[0].AxisX.Maximum = data_array_X.Length - 1;


            //Kalman
            //Type and colour
            chart1.Series[1].Label = "Kalman Data";
            chart1.Series[1].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            chart1.Series[1].Color = Color.Red;

            chart1.Series[2].Label = "Media";
            chart1.Series[2].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            chart1.Series[2].Color = Color.Green;


            UpdateData();
        }
        #endregion

        // Aqui indico a frequencia do Timer e seto o evento de callback
        #region InitialiseTimers()
        private void InitialiseTimers(int Timer_Interval = 100)
        {
            // Para o sensor de som
            // Melhor configuração: arduino com 100ms e gráfico com 50ms
            // Em geral, sensor do arduíno lendo no dobro do tempo do computador (serial)

            // Para o sensor piezo: 100ms no arduino e 100ms no intervalo de leitura!


            GraphTimer.Interval = 50;
            GraphTimer.Tick += new EventHandler(SineWave_Timer_Tick);


        }
        #endregion

        // É neste evento que obtenho o dado do sensor
        // calculo o filtro de Kalman e mando atualizar os gráficos
        #region SineWave_Timer_Tick()
        public void SineWave_Timer_Tick(object sender, EventArgs e)
        {

            float f_valor_sensor = ObtemDadoSensor();
            // Caso os valores esteja fora da área de valores do eixo Y do gráfico 
            // não mostra nada para evitar o problema do gráfico não mostrar nada
            if ((f_valor_sensor < chart1.ChartAreas[0].AxisY.Minimum) || (f_valor_sensor > chart1.ChartAreas[0].AxisY.Maximum))
                   return;

            // Calculando filtro de Kalman

            // Primeiro filtro não é muito bom
            // double f_valor_kalman = FiltroKalman1(f_valor_sensor);

            // Este segundo filtro é bem melhor
            // double f_valor_kalman = FiltroKalman2(f_valor_sensor);

            // Grava os treinamentos para torneira fechada e torneira aberta
            GravaTreinamento(f_valor_sensor);  // Baseado na média
            
            
            // Classifica a amostra com indice de confianca de 80%
            // TODO: O modo classificar deve rodar de forma contínua (sem botão)
            // durante a demonstração
            ClassificaEstado(f_valor_sensor, 0.01); // Baseado na média
            
            // Atualiza o contator global do exito X do gráfico
            ix++;

            // Coloca o valor na primeira linha do gráfico (azul)
           ColocaValorGrafico(f_valor_sensor, 1);

            // Coloca o valor na segunda linha do gráfico (vermelho)
          //  ColocaValorGrafico(f_valor_kalman, 2);

           // Coloca o valor na terceira linha do gráfico (preto)

          /*
          if (media_fechada != 0)
          {
            double valor_treinamento = Convert.ToDouble(Convert.ToString(f_valor_sensor).Substring(0, 3)) + media_fechada / 1000000;

           ColocaValorGrafico(valor_treinamento + (valor_treinamento*0.006) , 3);
           
          }
          */

            UpdateData();
            this.Refresh();

            // System.Threading.Thread.Sleep(10);
            
        }
        #endregion


        // Aqui vou classificar o estado com base no dado e tambem no indice de confianca
        // Aqui é interessante criar um thread separada quando for comparar os valores com o 
        // modelo para evitar a perda de dados do sensor
        #region ClassificaEstado()
        private void ClassificaEstado(double valor_atual, double indice_confianca)
        {
            if ((valor_atual <= 0.0) || (valor_atual >= 1.0))
                return;

            if (stopwatch.IsRunning)
            {
                TimeSpan TempoGasto = stopwatch.Elapsed + new TimeSpan(0, 0, 2);

                double x = TempoGasto.Seconds + TempoGasto.Milliseconds / 1000.0;

                // lblMediaAtual.Text = TempoGasto.ToString() + " - " +  x.ToString();
                // lblGasto.Text = "Consumo: " +((x / 3.8) * 100).ToString() + "ml";

                lblGasto.Text = "Consumo aprox.: " + String.Format("{0:0}", ((x / 3.8) * 100)) + "ml";
            }
            
            
            //  if (!bntClassifica.Text.Equals("Classificar"))
           // {
                contador_classifica++;

                // Como o sensor eh muito preciso, vou pegar a partir da segunda casa decimal
                // Exemplo: 0.7253458 vai virar 253458
                double valor_atual_convertido = Convert.ToDouble(Convert.ToString(valor_atual).Substring(3, 5));

                ClassificaEstadoTorneira[contador_classifica - 1] = valor_atual_convertido;

                // Finalizou o treinamento de uma sessao (2 segundos, MAX_MEDIDAS amostras)
                if (contador_classifica == MAX_MEDIDAS_CLASSIFICACAO)
                {
                    // Agora que jah gravei tudo vou compara com o modelo de acordo
                    // com a margem de confiança

                    // TODO: Chamar a thread que vai classificar esta janela de dados
                    // E também indicar na tela o estado

                    // ClassificaJanelaMediaGeral(ClassificaEstadoTorneira, indice_confianca);
                    // ClassificaJanelaPontos(ClassificaEstadoTorneira, indice_confianca);
                    ClassificaJanelaGiroTorneira(ClassificaEstadoTorneira, valor_atual, indice_confianca);

                    // Zerando o contador das amostras
                    contador_classifica = 0;

                   // lblEstado.Text = "-";

                    // Volta do o botão ao normal

                    // bntClassifica.Text = "Classificar";
                    // bntClassifica.Enabled = true;
                }

            // }
        }
        #endregion


        // Aqui faco a classificacao da janela atual baseado na diferença entre pontos!
        // TODO: Pensar em colocar este metodo em uma Thread
        #region ClassificaJanelaGiroTorneira()
        private void ClassificaJanelaGiroTorneira(double[] aDadosJanelaAtual, double valor_atual, double confianca)
        {
            // Muda o estado se algum dos pontos estiver mais ou menos <confianca>% acima ou abaixo da media treinada

            if (media_fechada != 0)
            {
                double media_completa = Convert.ToDouble(Convert.ToString(valor_atual).Substring(0, 3)) + media_fechada / 1000000;


                double limite_media_fechada = media_completa * confianca;
                bool mudou = false;

                for (int j = 0; j < MAX_MEDIDAS; j++)
                {
                    double medida = Convert.ToDouble(Convert.ToString(valor_atual).Substring(0, 3)) + aDadosJanelaAtual[j] / 1000000;
                    
                    if ((medida  <= (media_completa - limite_media_fechada)) || (medida >= (media_completa + limite_media_fechada)))
                        mudou = true;
                        
                }

                if (mudou)
                    InverteEstado();
                
            }
        }
        #endregion

        // Muda o estado dos labels!
        #region InverteEstado()
        private void InverteEstado()
        {
            if (lblEstado.Text.Equals("Torneira: FECHADA"))
            {
                lblEstado.ForeColor = Color.Green;
                lblEstado.Text = "Torneira: ABERTA";
                stopwatch.Reset();
                stopwatch.Start();
            
            }
            else
            {
                lblEstado.ForeColor = Color.Red;
                lblEstado.Text = "Torneira: FECHADA";
                // Stop timing.
                stopwatch.Stop();
            }
        }
        #endregion

        // Aqui faco a classificacao da janela atual baseado na diferença entre pontos!
        // TODO: Pensar em colocar este metodo em uma Thread
        #region ClassificaJanelaPontos()
        private void ClassificaJanelaPontos(double[] aDadosJanelaAtual, double confianca)
        {
            // Comparando os dados da JanelaAtual com os dados do Treinamento
            // Mas primeiro calculando as média para cada valor dos conjuntos de treinamento

            double[] aTornFechada = new double[MAX_MEDIDAS];
            double[] aTornAberta = new double[MAX_MEDIDAS];

            for (int j = 0; j < MAX_MEDIDAS; j++)
            {
                aTornFechada[j] = 0;
                aTornAberta[j] = 0;
            }

            for (int i = 0; i < MAX_TREINAMENTO; i++)
            {

                for (int j = 0; j < MAX_MEDIDAS; j++)
                {
                    aTornFechada[j] += TreinamentoTornFechada[i, j];
                    aTornAberta[j] += TreinamentoTornAberta[i, j];
                }
            }

            for (int j = 0; j < MAX_MEDIDAS; j++)
            {
                aTornFechada[j] = aTornFechada[j]/2;
                aTornAberta[j] = aTornAberta[j]/2;
            }

            // Agora comparando os valores


            double fechadaDiff = 0;
            double abertaDiff = 0;

            for (int j = 0; j < MAX_MEDIDAS; j++)
            {
                fechadaDiff += Math.Abs(aTornFechada[j] - aDadosJanelaAtual[j]);
                abertaDiff += Math.Abs(aTornAberta[j] - aDadosJanelaAtual[j]);
            }

            if(fechadaDiff > abertaDiff)
                lblEstado.Text = "Torneira: ABERTA";
            else
                lblEstado.Text = "Torneira: FECHADA";

        }
        #endregion

        // Aqui faco a classificacao da janela atual baseado na média
        // TODO: Pensar em colocar este metodo em uma Thread
        #region ClassificaJanelaMediaGeral()
        private void ClassificaJanelaMediaGeral(double[] aDadosJanelaAtual, double confianca)
        {
            // Primeiro calculando a media da janela atual
            double media_janela_atual = 0;


            // NOTA: A quantidade de dados lidos para a classificação (MAX_MEDIDAS_CLASSIFICACAO)
            // pode ser diferente da quantidade de dados lidos no treinamento (MAX_MEDIDAS)
            for (int i = 0; i < MAX_MEDIDAS_CLASSIFICACAO; i++)
            {
                media_janela_atual += aDadosJanelaAtual[i];
            }

            media_janela_atual = media_janela_atual / MAX_MEDIDAS_CLASSIFICACAO;

            lblMediaAtual.Text = Convert.ToString(media_janela_atual);

            // Agora fazendo a classificacao com base na media e na confianca
            double limite_media_fechada = media_fechada  * (1.0-confianca);
            double limite_media_aberta = media_aberta * (1.0 - confianca);

            if ((media_janela_atual >= (media_fechada - limite_media_fechada)) && (media_janela_atual <= (media_fechada + limite_media_fechada)))
                lblEstado.Text = "Torneira: FECHADA";
            else
            {
                if ((media_janela_atual >= (media_aberta - limite_media_aberta)) && (media_janela_atual <= (media_aberta + limite_media_aberta)))
                    lblEstado.Text = "Torneira: ABERTA";
                else
                    lblEstado.Text = "-";
            }
        }
        #endregion

        // Aqui vou fazer o treinamento de acordo com o botao abertado
        #region GravaTreinamento()
        private void GravaTreinamento(double f_valor_sensor)
        {

            if ((f_valor_sensor <= 0.0) || (f_valor_sensor >= 1.0))
                return;

            // Primeiro gravando apenas o treinamento da torneira fechada

            if (bntTreinaTorneiraFechada.Text.Equals("TreinarTorneiraFechada") && bntTreinaTorneiraFechada.Text.Equals("TreinarTorneiraAberta"))
            {

            }
            else
            {
                // Como o sensor eh muito preciso, vou pegar a partir da segunda casa decimal
                // Exemplo: 0.7253458 vai virar 253458
                double valor_treinamento = Convert.ToDouble(Convert.ToString(f_valor_sensor).Substring(3, 5));

                // TORNEIRA_FECHADA
                #region TorneiraFechada

                if(bntTreinaTorneiraFechada.Text.StartsWith("Treinando..."))
                {
                    TreinaTorneiraMedia(bntTreinaTorneiraFechada, TreinamentoTornFechada, "TreinarTorneiraFechada",valor_treinamento );
                }
                #endregion

                // TORNEIRA_ABERTA
                #region TorneiraAberta
                if (bntTreinaTorneiraAberta.Text.StartsWith("Treinando..."))
                {
                    TreinaTorneiraMedia(bntTreinaTorneiraAberta, TreinamentoTornAberta, "TreinarTorneiraAberta", valor_treinamento);
                }
                #endregion
            }
        }
        #endregion


        // Aqui é onde gravo os dados do estado sendo treinado (MEDIA)
        #region TreinaTorneiraMedia()
        private void TreinaTorneiraMedia(Button b, double[,] aDados, String textoBotao, Double valor)
        {
            contador_treinamento++;

            b.Text = "Treinando..." + Convert.ToString(qtd_treinamento);

            aDados[qtd_treinamento - 1, contador_treinamento - 1] = valor;


            // Finalizou o treinamento de uma sessao (MAX_MEDIDAS amostras)
            if (contador_treinamento == MAX_MEDIDAS)
            {
                // Zerando o contador das amostras
                contador_treinamento = 0;

                qtd_treinamento++;

                if (qtd_treinamento == MAX_TREINAMENTO + 1)
                {
                    qtd_treinamento = 1;

                    double media_por_sessao;

                    // Agora que jah gravei tudo,  calcular a media de cada janela e a media de todo o conjunto de treinamento
                    if (textoBotao.Equals("TreinarTorneiraFechada"))
                    {
                        media_fechada = 0;

                        for(int i = 0; i<MAX_TREINAMENTO;i++)
                        {
                            media_por_sessao = 0;
                            for (int j = 0; j < MAX_MEDIDAS; j++)
                            {
                                media_por_sessao += aDados[i, j];
                            }

                            media_fechada += (media_por_sessao / MAX_MEDIDAS);
                        }

                        media_fechada = media_fechada / MAX_TREINAMENTO;

                        // lblMediaFechada.Text = Convert.ToString(media_fechada);

                    }

                    if (textoBotao.Equals("TreinarTorneiraAberta"))
                    {
                        media_aberta = 0;

                        for (int i = 0; i < MAX_TREINAMENTO; i++)
                        {
                            media_por_sessao = 0;
                            for (int j = 0; j < MAX_MEDIDAS; j++)
                            {
                                media_por_sessao += aDados[i, j];
                            }

                            media_aberta += (media_por_sessao / MAX_MEDIDAS);
                        }

                        media_aberta = media_aberta / MAX_TREINAMENTO;

                        lblMediaAberta.Text = Convert.ToString(media_aberta);
                    }

                    b.Text = textoBotao;
                    b.Enabled = true;

                }
            }


        }
        #endregion


        // Preenche os dados dos arrays com os valores e deixa tudo pronto
        // para colocá-los no gráfico
        #region ColocaValorGrafico()
        private void ColocaValorGrafico(double f_valor_sensor, int grafico)
        {
            if(grafico == 1)
            {
                // Atualiza o gráfico com o dado do sensor
                Array.Copy(data_array_Y, 1, data_array_Y, 0, 99);
                data_array_Y[data_array_Y.Length - 1] = f_valor_sensor;
                Array.Copy(data_array_X, 1, data_array_X, 0, 99);
                data_array_X[data_array_X.Length - 1] = ix;
            }
            if (grafico == 2)
            {
                Array.Copy(Kalman_array_Y, 1, Kalman_array_Y, 0, 99);
                Kalman_array_Y[Kalman_array_Y.Length - 1] = f_valor_sensor;

                Array.Copy(Kalman_array_X, 1, Kalman_array_X, 0, 99);
                Kalman_array_X[Kalman_array_X.Length - 1] = ix;
            }
            if (grafico == 3)
            {
                Array.Copy(Media_Y, 1, Media_Y, 0, 99);
                Media_Y[Media_Y.Length - 1] = f_valor_sensor;

                Array.Copy(Media_X, 1, Media_X, 0, 99);
                Media_X[Media_X.Length - 1] = ix;
            }

        }
        #endregion

        // Obtendo o dado do sensor e fazendo as devidas conversões
        #region ObtemDadoSensor()
        private float ObtemDadoSensor()
        {
            float retorno;
            try
            {
                // Lê apenas um valor do sensor

                string data = serialPort1.ReadLine();
                retorno = (float)Convert.ToDouble(data) / 1000000;


                // Lendo dois valores float entre os sensores
                //  Intervalo 0.00 a 1.00
                /*
                 float f1;
                 float f2;

                 * 
                 * string data = serialPort1.ReadLine();
                
                string[] valores = data.Split(',');

                f1 = (float)Convert.ToDouble(valores[0])/1000000;
                f2 = (float)Convert.ToDouble(valores[1])/ 1000000;
                */
                // Console.WriteLine("Sensor 1:" + f1);
                // Console.WriteLine("Sensor 2:" + f2);


            }
            catch (Exception e1)
            {
                return -1;
            }

            return retorno;

        }
        #endregion

        // Primeiro filtro de Kalman (não é muito bom)
        #region FiltroKalman1(double valor)
        private double FiltroKalman1(double valor)
        {
            double retorno = Math.Round(SimpleKalman.update(valor));
            return retorno;
        }
        #endregion

        // Segundo Filtro de Kalman (um pouco melhor)
        #region FiltroKalman2(double valor)
        private double FiltroKalman2(double valor)
        {
            double retorno;

            Kalman1D k = new Kalman1D();
            k.Reset(
                (double)0.1,
                (double)0.1,
                (double)0.1,
                (double)400, 0);


            // Assume we get to see every other measurement we calculated, and use
            // the others as the points to compare for estimates.
            // Run the filter, note our time unit is 1.
            double[] kalman = new double[data_array_Y.Length];
            double[] vel = new double[data_array_Y.Length];
            double[] kGain = new double[data_array_Y.Length];

            for (int i = 0; i < data_array_Y.Length; i += 2)
            {
                if (i == 0)
                {
                    kalman[0] = 0;
                    vel[0] = k.Velocity;
                    kGain[0] = k.LastGain;
                    kalman[1] = k.Predicition(1);
                    vel[1] = k.Velocity;
                    kGain[1] = k.LastGain;
                }
                else
                {
                    kalman[i] = k.Update(data_array_Y[i], 1);
                    kalman[i + 1] = kalman[i];
                    vel[i] = k.Velocity;
                    kGain[i] = k.LastGain;
                    vel[i + 1] = vel[i];
                    kGain[i + 1] = kGain[i];
                    //     kalman[i + 1] = k.Predicition(1);
                }

            }

            retorno = kalman[kalman.Length - 1];

            return retorno;
        }
        #endregion

        // Atualizando os dados da tela  
        #region UpdateData()
        public void UpdateData()
        {
            chart1.ChartAreas[0].AxisX.Minimum = data_array_X[0];
            chart1.Series[0].Points.DataBindXY(data_array_X, data_array_Y);
            chart1.Series[1].Points.DataBindXY(Kalman_array_X, Kalman_array_Y);
            chart1.Series[2].Points.DataBindXY(Media_X, Media_Y);
        }
        #endregion

        // Este botão Inicia/Para o gráfico
        #region button1_Click()
        private void button1_Click(object sender, EventArgs e)
        {
            if (!GraphTimer.Enabled)
                GraphTimer.Start();
            else
                GraphTimer.Stop();
        }
        #endregion


        #region Form1_FormClosing()
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
           

        }
        #endregion


        #region Form1_FormClosed()
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                serialPort1.Close();
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
            }

        }
        #endregion


        // Sai do Form e manda fechar as conexões e tudo mais
        #region bntSair_Click
        private void bntSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        // Iniciando o treinamento da torneira fechada
        #region bntTreinaTorneiraFechada_Click()
        private void bntTreinaTorneiraFechada_Click(object sender, EventArgs e)
        {
            bntTreinaTorneiraFechada.Text = "Treinando...1";
            bntTreinaTorneiraFechada.Enabled = false;
        }
        #endregion

        // Iniciando o treinamento da torneira aberta
        #region bntTreinaTorneiraAberta_Click()
        private void bntTreinaTorneiraAberta_Click(object sender, EventArgs e)
        {

            bntTreinaTorneiraAberta.Text = "Treinando...1";
            bntTreinaTorneiraAberta.Enabled = false;
        }
        #endregion

        // Classificando o estado da torneira
        #region bntClassifica_Click()
        private void bntClassifica_Click(object sender, EventArgs e)
        {
            bntClassifica.Text = "Classificando....";
            bntClassifica.Enabled = false;
        }
        #endregion
    }



}
