namespace SensorSomSerialCSharp
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.button1 = new System.Windows.Forms.Button();
            this.bntTreinaTorneiraFechada = new System.Windows.Forms.Button();
            this.bntTreinaTorneiraAberta = new System.Windows.Forms.Button();
            this.bntClassifica = new System.Windows.Forms.Button();
            this.lblEstado = new System.Windows.Forms.Label();
            this.bntSair = new System.Windows.Forms.Button();
            this.lblMediaFechada = new System.Windows.Forms.Label();
            this.lblMediaAberta = new System.Windows.Forms.Label();
            this.lblMediaAtual = new System.Windows.Forms.Label();
            this.lblGasto = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // chart1
            // 
            chartArea2.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chart1.Legends.Add(legend2);
            this.chart1.Location = new System.Drawing.Point(12, 28);
            this.chart1.Name = "chart1";
            series4.ChartArea = "ChartArea1";
            series4.Legend = "Legend1";
            series4.Name = "TheData";
            series4.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series5.ChartArea = "ChartArea1";
            series5.Legend = "Legend1";
            series5.Name = "Kalman Data";
            series6.ChartArea = "ChartArea1";
            series6.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series6.Legend = "Legend1";
            series6.Name = "Media";
            this.chart1.Series.Add(series4);
            this.chart1.Series.Add(series5);
            this.chart1.Series.Add(series6);
            this.chart1.Size = new System.Drawing.Size(777, 582);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(433, 642);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(114, 47);
            this.button1.TabIndex = 1;
            this.button1.Text = "Get data";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // bntTreinaTorneiraFechada
            // 
            this.bntTreinaTorneiraFechada.Location = new System.Drawing.Point(847, 45);
            this.bntTreinaTorneiraFechada.Name = "bntTreinaTorneiraFechada";
            this.bntTreinaTorneiraFechada.Size = new System.Drawing.Size(138, 61);
            this.bntTreinaTorneiraFechada.TabIndex = 2;
            this.bntTreinaTorneiraFechada.Text = "TreinarTorneiraFechada";
            this.bntTreinaTorneiraFechada.UseVisualStyleBackColor = true;
            this.bntTreinaTorneiraFechada.Click += new System.EventHandler(this.bntTreinaTorneiraFechada_Click);
            // 
            // bntTreinaTorneiraAberta
            // 
            this.bntTreinaTorneiraAberta.Location = new System.Drawing.Point(847, 141);
            this.bntTreinaTorneiraAberta.Name = "bntTreinaTorneiraAberta";
            this.bntTreinaTorneiraAberta.Size = new System.Drawing.Size(138, 53);
            this.bntTreinaTorneiraAberta.TabIndex = 3;
            this.bntTreinaTorneiraAberta.Text = "TreinarTorneiraAberta";
            this.bntTreinaTorneiraAberta.UseVisualStyleBackColor = true;
            this.bntTreinaTorneiraAberta.Visible = false;
            this.bntTreinaTorneiraAberta.Click += new System.EventHandler(this.bntTreinaTorneiraAberta_Click);
            // 
            // bntClassifica
            // 
            this.bntClassifica.Location = new System.Drawing.Point(847, 238);
            this.bntClassifica.Name = "bntClassifica";
            this.bntClassifica.Size = new System.Drawing.Size(138, 62);
            this.bntClassifica.TabIndex = 4;
            this.bntClassifica.Text = "Classificar";
            this.bntClassifica.UseVisualStyleBackColor = true;
            this.bntClassifica.Visible = false;
            this.bntClassifica.Click += new System.EventHandler(this.bntClassifica_Click);
            // 
            // lblEstado
            // 
            this.lblEstado.AutoSize = true;
            this.lblEstado.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEstado.ForeColor = System.Drawing.Color.Red;
            this.lblEstado.Location = new System.Drawing.Point(811, 367);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(261, 31);
            this.lblEstado.TabIndex = 5;
            this.lblEstado.Text = "Torneira: FECHADA";
            // 
            // bntSair
            // 
            this.bntSair.Location = new System.Drawing.Point(802, 473);
            this.bntSair.Name = "bntSair";
            this.bntSair.Size = new System.Drawing.Size(207, 139);
            this.bntSair.TabIndex = 6;
            this.bntSair.Text = "SAIR";
            this.bntSair.UseVisualStyleBackColor = true;
            this.bntSair.Click += new System.EventHandler(this.bntSair_Click);
            // 
            // lblMediaFechada
            // 
            this.lblMediaFechada.AutoSize = true;
            this.lblMediaFechada.Location = new System.Drawing.Point(847, 113);
            this.lblMediaFechada.Name = "lblMediaFechada";
            this.lblMediaFechada.Size = new System.Drawing.Size(0, 13);
            this.lblMediaFechada.TabIndex = 7;
            // 
            // lblMediaAberta
            // 
            this.lblMediaAberta.AutoSize = true;
            this.lblMediaAberta.Location = new System.Drawing.Point(850, 206);
            this.lblMediaAberta.Name = "lblMediaAberta";
            this.lblMediaAberta.Size = new System.Drawing.Size(0, 13);
            this.lblMediaAberta.TabIndex = 8;
            // 
            // lblMediaAtual
            // 
            this.lblMediaAtual.AutoSize = true;
            this.lblMediaAtual.Location = new System.Drawing.Point(850, 317);
            this.lblMediaAtual.Name = "lblMediaAtual";
            this.lblMediaAtual.Size = new System.Drawing.Size(0, 13);
            this.lblMediaAtual.TabIndex = 9;
            // 
            // lblGasto
            // 
            this.lblGasto.AutoSize = true;
            this.lblGasto.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGasto.ForeColor = System.Drawing.Color.Black;
            this.lblGasto.Location = new System.Drawing.Point(811, 419);
            this.lblGasto.Name = "lblGasto";
            this.lblGasto.Size = new System.Drawing.Size(0, 31);
            this.lblGasto.TabIndex = 10;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1075, 701);
            this.Controls.Add(this.lblGasto);
            this.Controls.Add(this.lblMediaAtual);
            this.Controls.Add(this.lblMediaAberta);
            this.Controls.Add(this.lblMediaFechada);
            this.Controls.Add(this.bntSair);
            this.Controls.Add(this.lblEstado);
            this.Controls.Add(this.bntClassifica);
            this.Controls.Add(this.bntTreinaTorneiraAberta);
            this.Controls.Add(this.bntTreinaTorneiraFechada);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.chart1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button bntTreinaTorneiraFechada;
        private System.Windows.Forms.Button bntTreinaTorneiraAberta;
        private System.Windows.Forms.Button bntClassifica;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.Button bntSair;
        private System.Windows.Forms.Label lblMediaFechada;
        private System.Windows.Forms.Label lblMediaAberta;
        private System.Windows.Forms.Label lblMediaAtual;
        private System.Windows.Forms.Label lblGasto;
    }
}

