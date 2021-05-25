using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace Manipulador
{
    public partial class Form1 : Form
    {
        SerialPort SerialCom;
        string recep = string.Empty; //string para recepção serial
        string trata_recep = string.Empty; //string para tratamento de recepção serial

        string comandoFecharGarra = "grab", comandoAbrirGarra = "drop", comandoLuzGarra = "luz!", stop = "stop", comandoLerPosicoes = "read", comandoPosicaoStandard = "stnd";
        byte[] posicaoStandard;
        byte[] posicaoAtual;
        bool garraIsOpened = true;
        //int[,] juntasLimitacoes;

        byte[] inteirosEnviar; //array para amazenar bytes configurados para envio
        byte[] trataInt;//array para receber os bytes da porta

        bool sincronizarScroll = false;

        public delegate void delegateF(byte[] a);
        ///
        public Form1()
        {
            InitializeComponent();
            
            this.SerialCom = new SerialPort();
            
            inteirosEnviar = new byte[4];
            trataInt = new byte[50];
            posicaoStandard = new byte[4] {115,145,115,135}; //POSIÇÃO STANDARD
            posicaoAtual = new byte[4];
            //juntasLimitacoes = new int[4,2] {{35,189},{80,239},{40,264},{80,194}};//Limitações das garras       MIN   MAX
                                                                                                       //    J0
                                                                                                       //    ...
                                                                                                       //    J3
            this.SerialCom.DataReceived += new SerialDataReceivedEventHandler(SerialCom_DataReceived);
        }

        void SerialCom_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (this.SerialCom.BytesToRead >= 4)
            {
                this.SerialCom.Read(trataInt, 0, 4);
                this.BeginInvoke(new delegateF(recebe_serial), new object[] { trataInt });
            }
        }

        public void recebe_serial(byte[] a)
        {
            this.label6.Text = Convert.ToString(a[0]);
            this.label7.Text = Convert.ToString(a[1]);
            this.label8.Text = Convert.ToString(a[2]);
            this.label9.Text = Convert.ToString(a[3]);
            this.posicaoAtual = a;
            try
            {
                this.hScrollBar1.Value = Convert.ToInt16(a[0]);
                this.hScrollBar2.Value = Convert.ToInt16(a[1]);
                this.hScrollBar3.Value = Convert.ToInt16(a[2]);
                this.hScrollBar4.Value = Convert.ToInt16(a[3]);
            }
            catch { }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (string str in SerialPort.GetPortNames())
                this.comboBox1.Items.Add(str);

            try
            {
                this.comboBox1.SelectedIndex = 0;
            }
            catch { }
            this.SerialCom.BaudRate = 9600;
            this.SerialCom.StopBits = StopBits.One;
            this.SerialCom.DataBits = 8;
            this.SerialCom.Parity = Parity.None;
         
            //POSIÇÃO STANDARD
            textBox1.Text = Convert.ToString(posicaoStandard[0]);
            textBox2.Text = Convert.ToString(posicaoStandard[1]);
            textBox3.Text = Convert.ToString(posicaoStandard[2]);
            textBox4.Text = Convert.ToString(posicaoStandard[3]);
            
            hScrollBar1.Value = posicaoStandard[0];
            hScrollBar2.Value = posicaoStandard[1];
            hScrollBar3.Value = posicaoStandard[2];
            hScrollBar4.Value = posicaoStandard[3];
            //carregando configurações das limitações das juntas
            /*
            hScrollBar1.Minimum = juntasLimitacoes[0, 0]; hScrollBar1.Maximum = juntasLimitacoes[0, 1];
            hScrollBar2.Minimum = juntasLimitacoes[1, 0]; hScrollBar2.Maximum = juntasLimitacoes[1, 1];
            hScrollBar3.Minimum = juntasLimitacoes[2, 0]; hScrollBar1.Maximum = juntasLimitacoes[2, 1];
            hScrollBar4.Minimum = juntasLimitacoes[3, 0]; hScrollBar1.Maximum = juntasLimitacoes[3, 1];
            */
         }


        private void button8_Click(object sender, EventArgs e)
        {
            this.comboBox1.Items.Clear();
            foreach (string str in SerialPort.GetPortNames())
                this.comboBox1.Items.Add(str);

            try
            {
                this.comboBox1.SelectedIndex = 0;
            }
            catch { }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //abrir porta
            if (!this.SerialCom.IsOpen)
            {
                this.SerialCom.PortName = comboBox1.Text;
                try
                {
                    this.SerialCom.Open();
                    this.button6.Enabled = false;
                    this.button7.Enabled = true;

                    //habilitando botões de comando
                    this.button1.Enabled = true;
                    this.button3.Enabled = true;
                    this.button5.Enabled = true;
                    this.button10.Enabled = true;
                    this.button4.Enabled = true;
                    this.button9.Enabled = true;
                    this.checkBox1.Enabled = true;
                }catch{
                    MessageBox.Show("Não foi possível abrir a porta!!");
                }
                
            }
            
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //fechar porta
            if (this.SerialCom.IsOpen)
            {
                this.SerialCom.Close();
                this.button6.Enabled = true;
                this.button7.Enabled = false;

                //desabilitando botões de comando
                this.button1.Enabled = false;
                this.button3.Enabled = false;
                this.button5.Enabled = false;
                this.button10.Enabled = false;
                this.button4.Enabled = false;
                this.button9.Enabled = false;
                this.checkBox1.Enabled = false;
                this.hScrollBar1.Enabled = false;
                this.hScrollBar2.Enabled = false;
                this.hScrollBar3.Enabled = false;
                this.hScrollBar4.Enabled = false;
                this.checkBox1.Checked = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //fechar garra
            if (this.SerialCom.IsOpen)
            {
                if (garraIsOpened)
                {
                    garraIsOpened = false;
                    this.SerialCom.Write(comandoFecharGarra);
                }
                else{
                    garraIsOpened = true;
                    this.SerialCom.Write(comandoAbrirGarra);
                }
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            //luz garra
            if (this.SerialCom.IsOpen)
            {
                this.SerialCom.Write(comandoLuzGarra);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //stop
            if (this.SerialCom.IsOpen)
            {
                this.SerialCom.Write(stop);
                this.SerialCom.Write(comandoLerPosicoes);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //ler posições
            if (this.SerialCom.IsOpen)
            {
                this.SerialCom.Write(comandoLerPosicoes);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //enviar posições

            if (configurarInteirosEnviar())
            {
                //label6.Text = Convert.ToString(inteirosEnviar);
                if (SerialCom.IsOpen)
                {
                    this.SerialCom.Write(inteirosEnviar, 0, 4);
                }
            }
        }

        private bool configurarInteirosEnviar() {
            //pegar numeros e colocar na forma de char
            try
            {
                inteirosEnviar[0] = Convert.ToByte(textBox1.Text);
                inteirosEnviar[1] = Convert.ToByte(textBox2.Text);
                inteirosEnviar[2] = Convert.ToByte(textBox3.Text);
                inteirosEnviar[3] = Convert.ToByte(textBox4.Text);
                return true;
            }
            catch {
                MessageBox.Show("Há valores inválidos para envio!!");
                return false; ;
            }
            
        }

        private bool configurarInteirosEnviarScrolls() {
            //pegar valores do scroll e colocar na forma de char
            try
            {
                inteirosEnviar[0] = Convert.ToByte(hScrollBar1.Value);
                inteirosEnviar[1] = Convert.ToByte(hScrollBar2.Value);
                inteirosEnviar[2] = Convert.ToByte(hScrollBar3.Value);
                inteirosEnviar[3] = Convert.ToByte(hScrollBar4.Value);
                return true;
            }
            catch
            {
                MessageBox.Show("Pare de mexer esse trem adoidado assim!!");
                return false; ;
            }
         
        }

        private void textBox1_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) {

                if (configurarInteirosEnviar())
                {
                    if (SerialCom.IsOpen)
                        this.SerialCom.Write(inteirosEnviar, 0, 4);
                }
            }
        }

        
        private void button10_Click(object sender, EventArgs e)
        {
            //enviar posição standard
            if (this.SerialCom.IsOpen)
            {
                this.SerialCom.Write(posicaoStandard,0,4);

                //os valores dos scrolls ficam iguais ao standard
                try
                {
                    this.hScrollBar1.Value = posicaoStandard[0];
                    this.hScrollBar2.Value = posicaoStandard[1];
                    this.hScrollBar3.Value = posicaoStandard[2];
                    this.hScrollBar4.Value = posicaoStandard[3];
                }
                catch { }
            }
        }

        void checkBox1_CheckedChanged(object sender, System.EventArgs e)
        {
            if (checkBox1.Checked)
            {
                try
                {
                    /*
                    this.hScrollBar1.Value = Convert.ToInt16(inteirosEnviar[0]);
                    this.hScrollBar2.Value = Convert.ToInt16(inteirosEnviar[1]);
                    this.hScrollBar3.Value = Convert.ToInt16(inteirosEnviar[2]);
                    this.hScrollBar4.Value = Convert.ToInt16(inteirosEnviar[3]);
                    */

                    //this.SerialCom.Write(comandoLerPosicoes);
                    /*
                    this.hScrollBar1.Value = Convert.ToInt16(posicaoAtual[0]);
                    this.hScrollBar2.Value = Convert.ToInt16(posicaoAtual[1]);
                    this.hScrollBar3.Value = Convert.ToInt16(posicaoAtual[2]);
                    this.hScrollBar4.Value = Convert.ToInt16(posicaoAtual[3]);
                     * */
                }
                catch { }

                //habilitando scrolls
                this.hScrollBar1.Enabled = true;
                this.hScrollBar2.Enabled = true;
                this.hScrollBar3.Enabled = true;
                this.hScrollBar4.Enabled = true;

                //desabilitando textboxes e botão de enviar
                this.textBox1.Enabled = false;
                this.textBox2.Enabled = false;
                this.textBox3.Enabled = false;
                this.textBox4.Enabled = false;
                this.button9.Enabled = false;
        
            }
            else {

                //mudancaScroll = false;
                //desabilitando scrolls
                this.hScrollBar1.Enabled = false;
                this.hScrollBar2.Enabled = false;
                this.hScrollBar3.Enabled = false;
                this.hScrollBar4.Enabled = false;

                //habilitando textboxes e botão de envio
                this.textBox1.Enabled = true;
                this.textBox2.Enabled = true;
                this.textBox3.Enabled = true;
                this.textBox4.Enabled = true;
                this.button9.Enabled = true;
        
            }
        }

        
        void hScrollBar1_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
        {
            //this.label6.Text = Convert.ToString(this.hScrollBar1.Value);
            if (checkBox1.Checked)
            {
                if (configurarInteirosEnviarScrolls())
                {
                    /*
                    label6.Text = Convert.ToString(inteirosEnviar[0]);
                    label7.Text = Convert.ToString(inteirosEnviar[1]);
                    label8.Text = Convert.ToString(inteirosEnviar[2]);
                    label9.Text = Convert.ToString(inteirosEnviar[3]);
                     * */
                    if (SerialCom.IsOpen)
                    {
                        this.SerialCom.Write(inteirosEnviar, 0, 4);

                        textBox1.Text = Convert.ToString(inteirosEnviar[0]);
                        textBox2.Text = Convert.ToString(inteirosEnviar[1]);
                        textBox3.Text = Convert.ToString(inteirosEnviar[2]);
                        textBox4.Text = Convert.ToString(inteirosEnviar[3]);
                    }
                }
            }
        }


    }
}
