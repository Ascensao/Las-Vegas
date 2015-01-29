using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Las_Vegas
{
    public partial class frmMain : Form
    {
        string path = null; //Contêm caminho do ficheiro actual de registo.
        int currentNumber = -1; //Contêm o número selecionado.
        int currentScale = 0; //Contêm a escala dos tipos de saída selecionada.
        int[] wleavingArray = new int[38] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,  0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,};

        public frmMain()
        {
            InitializeComponent();
            comboBoxWLeaving.SelectedIndex = 0;
        }

        //Adiciona um dado (número) ao ficheiro de registo.
        private void addReg(int number)
        {
            securedPath();
            FileStream stream = new FileStream(path, FileMode.Append);
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine(number);
            writer.Close();
            stream.Close();
        }

        //Actualiza os dados estatisticos.
        private void updateStats(Func<string, bool> predicate, int scale)
        {
            securedPath();
            FileStream stream = new FileStream(path, FileMode.Open);
            StreamReader reader = new StreamReader(stream);
            int[] med = new int[10]; //Guarda valor de 10 tipos de saída.
            int countNumber = 0; //Conta o número de saídas do numero selecionado.
            int countTotal = 0; //Conta o total dos números registados.
            int countScale = scale; //Número da escala. Ex: 0
            int percentage = 0; //Calcula a percerntagem de saídas do número selecionado.
            int wleaving = 0; //Verifica a quantidade de vezes que o número selecionado não saí.
            int wleavingMax = 0; //Verifica a máxima quantidade de vezes que o número selecionado não saí.
            int median = 0; //Guarda valor da mediana actual.
            int medianMax = 0;//Guarda o valor da mediana mácima.
            int leavingMedian = 0; //Guarda valor da saída da mediana.
            string line = null; ;

            do
            {
                line = reader.ReadLine();
                if (line!=null)
                {
                    countTotal++;

                    if (predicate(line))
                    {
                        if (wleaving > wleavingMax)
                        {
                            wleavingMax = wleaving;
                        }

                        for (int i = 0; i < med.Length; i++)
                        {
                            if (wleaving == countScale)
                            {
                                med[i]++;
                            }
                            countScale++;
                        }

                        countNumber++;
                        wleaving = 0;
                    }
                    else
                    {
                        wleaving++;
                    }
                }
                countScale = scale;
            } while (line != null);
            reader.Close();
            stream.Close();

            if (countTotal != 0)
            {
                percentage = (countNumber * 100) / countTotal;
            }

            for (int x = 0; x < med.Length; x++)
            {
                lstBoxLine.Items.Add((countScale).ToString());
                lstBoxMed.Items.Add(med[x].ToString());
                if (countNumber != 0)
                {
                    median = (med[x] * 100) / countNumber;
                }
                lstBoxPer.Items.Add(median.ToString()+"%");
                if (median > medianMax)
                {
                    medianMax = median;
                    leavingMedian = x;
                }
                countScale++;
            }

            fileLenght();
            updateStats2(countTotal);

            lblNbrReg.Text = countTotal.ToString();
            txtNbrPlay.Text = countNumber.ToString();
            txtPerPlay.Text = percentage.ToString() + "%";
            txtWLeaving.Text = wleaving.ToString();
            txtWLeavingMax.Text = wleavingMax.ToString();
            txtMed.Text = "[" + (leavingMedian).ToString() + "] " + medianMax.ToString() + "%";
        }

        //Actualiza o Histórico
        private void updateStats2(int localCountTotal)
        {
            securedPath();
            FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
            StreamReader reader = new StreamReader(stream);
            int countLine = 0;
            int y = 0;
            string[] history = new string[27] { "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*",
                "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*" };
            int[] historyNumber = new int[27];
            string line = null;

            do
            {
                line = reader.ReadLine();
                if (line!=null)
                {
                    countLine++;
                    if (countLine >= (localCountTotal - 26))
                    {
                        if (line != "37")
                        {
                            history[y] = line;
                        }
                        else
                        {
                            history[y] = "00";
                        }
                        y++;
                    }
                }
            } while (line != null);

            reader.Close();
            stream.Close();

            lblh0.Text = history[26];
            lblh1.Text = history[25];
            lblh2.Text = history[24];
            lblh3.Text = history[23];
            lblh4.Text = history[22];
            lblh5.Text = history[21];
            lblh6.Text = history[20];
            lblh7.Text = history[19];
            lblh8.Text = history[18];
            lblh9.Text = history[17];
            lblh10.Text = history[16];
            lblh11.Text = history[15];
            lblh12.Text = history[14];
            lblh13.Text = history[13];
            lblh14.Text = history[12];
            lblh15.Text = history[11];
            lblh16.Text = history[10];
            lblh17.Text = history[9];
            lblh18.Text = history[8];
            lblh19.Text = history[7];
            lblh20.Text = history[6];
            lblh21.Text = history[5];
            lblh22.Text = history[4];
            lblh23.Text = history[3];
            lblh24.Text = history[2];
            lblh25.Text = history[1];
            lblh26.Text = history[0];

            //History Group ======================================
            if (countLine >= 27)
            {
                for (int i = 0; i < history.Length; i++)
                {
                    historyNumber[i] = Convert.ToInt32(history[i]);
                }
                
            }
            //Fim de History Grou ===============================
            historyGroup();

            //Actualização de Gráficos
            if (radBtnDuz.Checked)
            {
                graphDozen();
            }
            if (radBtnLine.Checked)
            {
                graphLine();
            }
            if (radBtnColor.Checked)
            {
                graphColor();
            }
            if (radBtnOdd.Checked)
            {
                graphEven();
            }
            if (radBtnNum.Checked)
            {
                graphNumber();
            }

        }

        //Verifica tamnho no registo
        private void fileLenght()
        {
            FileInfo info = new FileInfo(path);
            if (info.Length > 1024)
            {
                if (info.Length > 1048576)
                {
                    if (info.Length > 1073741824)
                    {
                        lblLenght.Text = ((info.Length / 1024)/1024).ToString() + " MB";
                        //Tamanho superiro a 4MB o indicador retorna 0 devido a erro no sistema
                    }
                }
                else
                {
                    lblLenght.Text = (info.Length/1024).ToString() + " KB";
                }
            }
            else
            {
                lblLenght.Text = info.Length.ToString() + " B";
            }

        }

        private void graphDozen()
        {
            lblRad3.Visible = true;
            progressBar3.Visible = true;

            FileStream stream = new FileStream(path, FileMode.Open);
            StreamReader reader = new StreamReader(stream);
            string line = null;
            int number = 0;
            int countTotal = 0;
            int[] count = new int[3] { 0, 0, 0 };
            int[] percentage = new int[3] { 0, 0, 0 };

            do
            {
                line = reader.ReadLine(); 
                if (int.TryParse(line, out number) == true)
                {
                    if ((number >= 1) && (number <= 12))
                    {
                        count[0]++;
                    }
                    if ((number >= 13) && (number <= 24))
                    {
                        count[1]++;
                    }
                    if ((number >= 25) && (number <= 36))
                    {
                        count[2]++;
                    }
                    countTotal++;
                }
                    
            } while (line != null);

            reader.Close();
            stream.Close();

            if (countTotal != 0)
            {
                percentage[0] = (count[0] * 100) / countTotal;
                percentage[1] = (count[1] * 100) / countTotal;
                percentage[2] = (count[2] * 100) / countTotal;
            }

            lblRad1.Text = "1ª Duzia";
            lblRad2.Text = "2ª Duzia";
            lblRad3.Text = "3ª Duzia";

            progressBar1.Value = percentage[0];
            progressBar2.Value = percentage[1];
            progressBar3.Value = percentage[2];   
        }

        private void graphLine()
        {
            lblRad3.Visible = true;
            progressBar3.Visible = true;

            FileStream stream = new FileStream(path, FileMode.Open);
            StreamReader reader = new StreamReader(stream);
            string line = null;
            int number=0;
            int countTotal = 0;
            int[] increment = new int[3] {3,2,1};
            int[] count = new int[3] { 0, 0, 0 };
            int[] percentage = new int[3] { 0, 0, 0 };

            do
            {
                line = reader.ReadLine();
                if (int.TryParse(line,out number)==true)
                {
                    for (int i = 0; i < 12; i++)
                    {
                        if (number == increment[0])
                        {
                            count[0]++;
                        }
                        if (number == increment[1])
                        {
                            count[1]++;
                        }
                        if (number == increment[2])
                        {
                            count[2]++;
                        }
                        increment[0] += 3;
                        increment[1] += 3;
                        increment[2] += 3;    
                    }
                    increment[0] = 3;
                    increment[1] = 2;
                    increment[2] = 1;
                    countTotal++;
                }

            } while (line != null);

            reader.Close();
            stream.Close();

            if (countTotal != 0)
            {
                percentage[0] = (count[0] * 100) / countTotal;
                percentage[1] = (count[1] * 100) / countTotal;
                percentage[2] = (count[2] * 100) / countTotal;
            }

            lblRad1.Text = "1ª Linha";
            lblRad2.Text = "2ª Linha";
            lblRad3.Text = "3ª Linha";

            progressBar1.Value = percentage[0];
            progressBar2.Value = percentage[1];
            progressBar3.Value = percentage[2];   
           
        }

        private void graphColor()
        {
            lblRad3.Visible = false;
            progressBar3.Visible = false;

            FileStream stream = new FileStream(path, FileMode.Open);
            StreamReader reader = new StreamReader(stream);
            string line = null;
            int number = 0;
            int countTotal = 0;
            int[] count = new int[2] { 0, 0};
            int[] percentage = new int[2] { 0, 0 };

            do
            {
                line = reader.ReadLine();
                if (int.TryParse(line,out number)==true)
                {
                    if (checkColor(number) == true)
                    {
                        count[0]++;
                    }
                    if (checkColor(number) == false)
                    {
                        count[1]++;
                    }
                    countTotal++;
                }

            } while (line != null);

            reader.Close();
            stream.Close();

            if (countTotal != 0)
            {
                percentage[0] = (count[0] * 100) / countTotal;
                percentage[1] = (count[1] * 100) / countTotal;
            }

            lblRad1.Text = "Preto";
            lblRad2.Text = "Vermelho";

            progressBar1.Value = percentage[0];
            progressBar2.Value = percentage[1];  
        }

        private void graphEven()
        {
            lblRad3.Visible = false;
            progressBar3.Visible = false;

            FileStream stream = new FileStream(path, FileMode.Open);
            StreamReader reader = new StreamReader(stream);
            string line = null;
            int number = 0;
            int countTotal = 0;
            int[] count = new int[2] { 0, 0 };
            int[] percentage = new int[2] { 0, 0 };

            do
            {
                line = reader.ReadLine();
                if (int.TryParse(line, out number) == true)
                {
                    if (number % 2 == 0)
                    {
                        count[0]++;
                    }
                    else
                    {
                        count[1]++;
                    }
                    countTotal++;
                }

            } while (line != null);

            reader.Close();
            stream.Close();

            if (countTotal != 0)
            {
                percentage[0] = (count[0] * 100) / countTotal;
                percentage[1] = (count[1] * 100) / countTotal;
            }

            lblRad1.Text = "Par";
            lblRad2.Text = "Ímpar";

            progressBar1.Value = percentage[0];
            progressBar2.Value = percentage[1]; 
        }

        private void graphNumber()
        {
            lblRad3.Visible = false;
            progressBar3.Visible = false;

            FileStream stream = new FileStream(path, FileMode.Open);
            StreamReader reader = new StreamReader(stream);
            string line = null;
            int number = 0;
            int countTotal = 0;
            int[] count = new int[2] { 0, 0 };
            int[] percentage = new int[2] { 0, 0 };

            do
            {
                line = reader.ReadLine();
                if (int.TryParse(line, out number) == true)
                {
                    if ((number>=1)&&(number<=18)) 
                    {
                        count[0]++;
                    }
                    if((number>=19)&&(number<=36))
                    {
                        count[1]++;
                    }
                    countTotal++;
                }

            } while (line != null);

            reader.Close();
            stream.Close();

            if (countTotal != 0)
            {
                percentage[0] = (count[0] * 100) / countTotal;
                percentage[1] = (count[1] * 100) / countTotal;
            }

            lblRad1.Text = "1/18";
            lblRad2.Text = "19/36";

            progressBar1.Value = percentage[0];
            progressBar2.Value = percentage[1]; 
        }

        //Este evento assegura que existe um "caminho de ficheiro"
        private void securedPath()
        {
            if (File.Exists(path)==false)
            {
                DialogResult result =
                MessageBox.Show("Deseja criar um novo registo?", "Novo Ficheiro",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    btnAddDB.PerformClick();
                }
                else
                {
                    btnLoadDB.PerformClick();
                }
            }
        }
          
        //Métodos Auxiliares ======================================================================

        public char checkDozen(int number)
        {
            char dozen = ' ';
            if ((number >= 1) && (number <= 12))
            {
                dozen = '1';
            }
            if ((number >= 13) && (number <= 24))
            {
                dozen = '2';
            }
            if ((number >= 25) && (number <= 36))
            {
                dozen = '3';
            }
            return dozen;
        }

        public bool checkColor(int number)
        {
            bool color=false;
            switch (number)
            {
                case 1:
                    color = false;
                    break;
                case 2:
                    color = true;
                    break;
                case 3:
                    color = false;
                    break;
                case 4:
                    color = true;
                    break;
                case 5:
                    color = false;
                    break;
                case 6:
                    color = true;
                    break;
                case 7:
                    color = false;
                    break;
                case 8:
                    color = true;
                    break;
                case 9:
                    color = false;
                    break;
                case 10:
                    color = true;
                    break;
                case 11:
                    color = true;
                    break;
                case 12:
                    color = false;
                    break;
                case 13:
                    color = true;
                    break;
                case 14:
                    color = false;
                    break;
                case 15:
                    color = true;
                    break;
                case 16:
                    color = false;
                    break;
                case 17:
                    color = true;
                    break;
                case 18:
                    color = false;
                    break;
                case 19:
                    color = false;
                    break;
                case 20:
                    color = true;
                    break;
                case 21:
                    color = false;
                    break;
                case 22:
                    color = true;
                    break;
                case 23:
                    color = false;
                    break;
                case 24:
                    color = true;
                    break;
                case 25:
                    color = false;
                    break;
                case 26:
                    color = true;
                    break;
                case 27:
                    color = false;
                    break;
                case 28:
                    color = true;
                    break;
                case 29:
                    color = true;
                    break;
                case 30:
                    color = false;
                    break;
                case 31:
                    color = true;
                    break;
                case 32:
                    color = false;
                    break;
                case 33:
                    color = true;
                    break;
                case 34:
                    color = false;
                    break;
                case 35:
                    color = true;
                    break;
                case 36:
                    color = false;
                    break;
            }
            return color;

        }

        public bool checkEvenOdd(int number) //Par=true Impar=false
        {
            bool odd;
            if (number % 2 == 0)
            {
                odd = true;
            }
            else
            {
                odd = false;
            }
            return odd;
        }

        public bool checkGreaterOrLesser(int number)
        {
            bool greater;
            if((number>0)&&(number<19))
            {
                greater = true;
            }
            else
            {
                greater = false;
            }
            return greater;
        }

        public char checkColumn(int number)
        {
            char col=' ';
            for (int x = 1; x < 36; x=x+3)
            {
                if (number==x)
                {
                    col = '1';
                }
            }
            for (int y = 2; y < 36; y = y + 3)
            {
                if (number == y)
                {
                    col = '2';
                }
            }
            for (int z = 3; z < 36; z = z + 3)
            {
                if (number == z)
                {
                    col = '3';
                }
            }
            return col;
        }

        public void historyGroup()
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            StreamReader reader = new StreamReader(stream);
            string line=null;
            int[] dozen = new int[3]; //Guarda valores de saidas das duzias
            int[] colun = new int[3]; //Guarda valores de saidas das colunas
            int[] color = new int[2]; //Guarda valores de saidas das cores
            int[] even = new int[2]; //Guarda valores de saidas das pares e impares
            int[] great = new int[2]; //Guarda valores de saidas das maiores e menores

            do
            {
                                line = reader.ReadLine();
                                if (line != null)
                                {
                                    //Duzias
                                    if (checkDozen(Convert.ToInt32(line)) == '1')
                                    {
                                        dozen[0] = 0;
                                        dozen[1]++;
                                        dozen[2]++;
                                    }
                                    if (checkDozen(Convert.ToInt32(line)) == '2')
                                    {
                                        dozen[1] = 0;
                                        dozen[0]++;
                                        dozen[2]++;
                                    }
                                    if (checkDozen(Convert.ToInt32(line)) == '3')
                                    {
                                        dozen[2] = 0;
                                        dozen[1]++;
                                        dozen[2]++;
                                    }

                                    //Colunas
                                    if (checkColumn(Convert.ToInt32(line)) == '1')
                                    {
                                        colun[0] = 0;
                                        colun[1]++;
                                        colun[2]++;
                                    }
                                    if (checkColumn(Convert.ToInt32(line)) == '2')
                                    {
                                        colun[1] = 0;
                                        colun[0]++;
                                        colun[2]++;
                                    }
                                    if (checkColumn(Convert.ToInt32(line)) == '3')
                                    {
                                        colun[2] = 0;
                                        colun[1]++;
                                        colun[2]++;
                                    }
                                    //Cores
                                    if (checkColor(Convert.ToInt32(line)) == true)
                                    {
                                        color[0] = 0;
                                        color[1]++;
                                    }
                                    if (checkColor(Convert.ToInt32(line)) == false)
                                    {
                                        color[1] = 0;
                                        color[0]++;
                                    }
                                    //Pares
                                    if (checkEvenOdd(Convert.ToInt32(line)) == true)
                                    {
                                        even[0] = 0;
                                        even[1]++;
                                    }
                                    if (checkEvenOdd(Convert.ToInt32(line)) == false)
                                    {
                                        even[1] = 0;
                                        even[0]++;
                                    }
                                    //Maiores
                                    if (checkGreaterOrLesser(Convert.ToInt32(line)) == true)
                                    {
                                        great[0] = 0;
                                        great[1]++;
                                    }
                                    if (checkGreaterOrLesser(Convert.ToInt32(line)) == false)
                                    {
                                        great[1] = 0;
                                        great[0]++;
                                    }
                                }
            } while (line != null);
            reader.Close();
            stream.Close();


            //Duzias
            if ((dozen[0] > dozen[1]) && (dozen[0] > dozen[2]))
            {
                lblDozen.Text = "(" + dozen[0].ToString() + ")" + " 1º Duzia";
            }
            if ((dozen[1] > dozen[0]) && (dozen[1] > dozen[2]))
            {
                lblDozen.Text = "(" + dozen[1].ToString() + ")" + " 2º Duzia";
            }
            if ((dozen[2] > dozen[0]) && (dozen[2] > dozen[1]))
            {
                lblDozen.Text = "(" + dozen[2].ToString() + ")" + " 3º Duzia";
            }

            //Colunas
            if ((colun[0] > colun[1]) && (colun[0] > colun[2]))
            {
                lblLine.Text = "(" + colun[0].ToString() + ")" + " 1º Coluna";
            }
            if ((colun[1] > colun[0]) && (colun[1] > colun[2]))
            {
                lblLine.Text = "(" + colun[1].ToString() + ")" + " 2º Coluna";
            }
            if ((colun[2] > colun[0]) && (colun[2] > colun[1]))
            {
                lblLine.Text = "(" + colun[2].ToString() + ")" + " 3º Coluna";
            }

            //Cores
            if (color[0] > color[1])
            {
                lblColor.Text = "(" + color[0].ToString() + ")" + "Preto";
            }
            else
            {
                lblColor.Text = "(" + color[1].ToString() + ")" + "Vermelho";
            }

            //Pares
            if (even[0] > even[1])
            {
                lblEven.Text = "(" + even[0].ToString() + ")" + "Par";
            }
            else
            {
                lblEven.Text = "(" + even[1].ToString() + ")" + "ímpar";
            }


            //Maiores
            if (great[0] > great[1])
            {
                lblNumber.Text = "(" + great[0].ToString() + ")" + "1-18";
            }
            else
            {
                lblNumber.Text = "(" + great[1].ToString() + ")" + "19-36";
            }
        }

        public static decimal GetMedian(int[] array)
        {
            int[] tempArray = array;
            int count = tempArray.Length;

            Array.Sort(tempArray);

            decimal medianValue = 0;

            if (count % 2 == 0)
            {
                // count is even, need to get the middle two elements, add them together, then divide by 2
                int middleElement1 = tempArray[(count / 2) - 1];
                int middleElement2 = tempArray[(count / 2)];
                medianValue = (middleElement1 + middleElement2) / 2;
            }
            else
            {
                // count is odd, simply get the middle element.
                medianValue = tempArray[(count / 2)];
            }

            return medianValue;
        }

        private void updateWithoutLeavingArray(int num)
        {
            if (num == 0)
            {
                wleavingArray[0] = 0;
            }
            else
            {
                wleavingArray[0]++;
            }

            if (num == 1)
            {
                wleavingArray[1] = 0;
            }
            else
            {
                wleavingArray[1]++;
            }

            if (num == 2)
            {
                wleavingArray[2] = 0;
            }
            else
            {
                wleavingArray[2]++;
            }

            if (num == 3)
            {
                wleavingArray[3] = 0;
            }
            else
            {
                wleavingArray[3]++;
            }

            if (num == 4)
            {
                wleavingArray[4] = 0;
            }
            else
            {
                wleavingArray[4]++;
            }

            if (num == 5)
            {
                wleavingArray[5] = 0;
            }
            else
            {
                wleavingArray[5]++;
            }

            if (num == 6)
            {
                wleavingArray[6] = 0;
            }
            else
            {
                wleavingArray[6]++;
            }

            if (num == 7)
            {
                wleavingArray[7] = 0;
            }
            else
            {
                wleavingArray[7]++;
            }

            if (num == 8)
            {
                wleavingArray[8] = 0;
            }
            else
            {
                wleavingArray[8]++;
            }

            if (num == 9)
            {
                wleavingArray[9] = 0;
            }
            else
            {
                wleavingArray[9]++;
            }

            if (num == 10)
            {
                wleavingArray[10] = 0;
            }
            else
            {
                wleavingArray[10]++;
            }

            if (num == 11)
            {
                wleavingArray[11] = 0;
            }
            else
            {
                wleavingArray[11]++;
            }

            if (num == 12)
            {
                wleavingArray[12] = 0;
            }
            else
            {
                wleavingArray[12]++;
            }

            if (num == 13)
            {
                wleavingArray[13] = 0;
            }
            else
            {
                wleavingArray[13]++;
            }

            if (num == 14)
            {
                wleavingArray[14] = 0;
            }
            else
            {
                wleavingArray[14]++;
            }

            if (num == 15)
            {
                wleavingArray[15] = 0;
            }
            else
            {
                wleavingArray[15]++;
            }

            if (num == 16)
            {
                wleavingArray[16] = 0;
            }
            else
            {
                wleavingArray[16]++;
            }

            if (num == 17)
            {
                wleavingArray[17] = 0;
            }
            else
            {
                wleavingArray[17]++;
            }

            if (num == 18)
            {
                wleavingArray[18] = 0;
            }
            else
            {
                wleavingArray[18]++;
            }

            if (num == 19)
            {
                wleavingArray[19] = 0;
            }
            else
            {
                wleavingArray[19]++;
            }

            if (num == 20)
            {
                wleavingArray[20] = 0;
            }
            else
            {
                wleavingArray[20]++;
            }

            if (num == 21)
            {
                wleavingArray[21] = 0;
            }
            else
            {
                wleavingArray[21]++;
            }

            if (num == 22)
            {
                wleavingArray[22] = 0;
            }
            else
            {
                wleavingArray[22]++;
            }

            if (num == 23)
            {
                wleavingArray[23] = 0;
            }
            else
            {
                wleavingArray[23]++;
            }

            if (num == 24)
            {
                wleavingArray[24] = 0;
            }
            else
            {
                wleavingArray[24]++;
            }

            if (num == 25)
            {
                wleavingArray[25] = 0;
            }
            else
            {
                wleavingArray[25]++;
            }

            if (num == 26)
            {
                wleavingArray[26] = 0;
            }
            else
            {
                wleavingArray[26]++;
            }

            if (num == 27)
            {
                wleavingArray[27] = 0;
            }
            else
            {
                wleavingArray[27]++;
            }

            if (num == 28)
            {
                wleavingArray[28] = 0;
            }
            else
            {
                wleavingArray[28]++;
            }

            if (num == 29)
            {
                wleavingArray[29] = 0;
            }
            else
            {
                wleavingArray[29]++;
            }

            if (num == 30)
            {
                wleavingArray[30] = 0;
            }
            else
            {
                wleavingArray[30]++;
            }

            if (num == 31)
            {
                wleavingArray[31] = 0;
            }
            else
            {
                wleavingArray[31]++;
            }

            if (num == 32)
            {
                wleavingArray[32] = 0;
            }
            else
            {
                wleavingArray[32]++;
            }

            if (num == 33)
            {
                wleavingArray[33] = 0;
            }
            else
            {
                wleavingArray[33]++;
            }

            if (num == 34)
            {
                wleavingArray[34] = 0;
            }
            else
            {
                wleavingArray[34]++;
            }

            if (num == 35)
            {
                wleavingArray[35] = 0;
            }
            else
            {
                wleavingArray[35]++;
            }

            if (num == 36)
            {
                wleavingArray[36] = 0;
            }
            else
            {
                wleavingArray[36]++;
            }

            if (num == 37)
            {
                wleavingArray[37] = 0;
            }
            else
            {
                wleavingArray[37]++;
            }
        }

        private void arrayWithoutLeavingToLabel() //preeche array wleavingArray
        {
            
            lblN0.Text = wleavingArray[0].ToString();
            lblN1.Text = wleavingArray[1].ToString();
            lblN2.Text = wleavingArray[2].ToString();
            lblN3.Text = wleavingArray[3].ToString();
            lblN4.Text = wleavingArray[4].ToString();
            lblN5.Text = wleavingArray[5].ToString();
            lblN6.Text = wleavingArray[6].ToString();
            lblN7.Text = wleavingArray[7].ToString();
            lblN8.Text = wleavingArray[8].ToString();
            lblN9.Text = wleavingArray[9].ToString();
            lblN10.Text = wleavingArray[10].ToString();
            lblN11.Text = wleavingArray[11].ToString();
            lblN12.Text = wleavingArray[12].ToString();
            lblN13.Text = wleavingArray[13].ToString();
            lblN14.Text = wleavingArray[14].ToString();
            lblN15.Text = wleavingArray[15].ToString();
            lblN16.Text = wleavingArray[16].ToString();
            lblN17.Text = wleavingArray[17].ToString();
            lblN18.Text = wleavingArray[18].ToString();
            lblN19.Text = wleavingArray[19].ToString();
            lblN20.Text = wleavingArray[20].ToString();
            lblN21.Text = wleavingArray[21].ToString();
            lblN22.Text = wleavingArray[22].ToString();
            lblN23.Text = wleavingArray[23].ToString();
            lblN24.Text = wleavingArray[24].ToString();
            lblN25.Text = wleavingArray[25].ToString();
            lblN26.Text = wleavingArray[26].ToString();
            lblN27.Text = wleavingArray[27].ToString();
            lblN28.Text = wleavingArray[28].ToString();
            lblN29.Text = wleavingArray[29].ToString();
            lblN30.Text = wleavingArray[30].ToString();
            lblN31.Text = wleavingArray[31].ToString();
            lblN32.Text = wleavingArray[32].ToString();
            lblN33.Text = wleavingArray[33].ToString();
            lblN34.Text = wleavingArray[34].ToString();
            lblN35.Text = wleavingArray[35].ToString();
            lblN36.Text = wleavingArray[36].ToString();
            lblN00.Text = wleavingArray[37].ToString();
        }

        static public class ArraySorter<T>
    where T : IComparable
        {
            static public void SortDescending(T[] array)
            {
                Array.Sort<T>(array, s_Comparer);
            }
            static private ReverseComparer s_Comparer = new ReverseComparer();
            private class ReverseComparer : IComparer<T>
            {
                public int Compare(T object1, T object2)
                {
                    return -((IComparable)object1).CompareTo(object2);
                }
            }
        }

        private static bool hasNumberGreatherThanZero(int[] arr) //verifica se algum item do array é maior que zero se sim então "true".
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void withoutLeavingColorLabel()
        {
            if (hasNumberGreatherThanZero(wleavingArray) == true)
            {
                int[] wleavingArrayTemp = (int[])wleavingArray.Clone();

                ArraySorter<int>.SortDescending(wleavingArrayTemp); 

                int lastX = wleavingArrayTemp[Convert.ToInt16(comboBoxWLeaving.SelectedItem)-1];


                if (Convert.ToInt16(lblN00.Text) >= lastX)
                {
                    lblN00.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN00.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN0.Text) >= lastX)
                {
                    lblN0.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN0.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN1.Text) >= lastX)
                {
                    lblN1.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN1.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN1.Text) >= lastX)
                {
                    lblN1.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN1.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN2.Text) >= lastX)
                {
                    lblN2.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN2.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN3.Text) >= lastX)
                {
                    lblN3.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN3.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN4.Text) >= lastX)
                {
                    lblN4.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN4.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN5.Text) >= lastX)
                {
                    lblN5.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN5.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN6.Text) >= lastX)
                {
                    lblN6.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN6.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN7.Text) >= lastX)
                {
                    lblN7.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN7.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN8.Text) >= lastX)
                {
                    lblN8.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN8.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN9.Text) >= lastX)
                {
                    lblN9.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN9.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN10.Text) >= lastX)
                {
                    lblN10.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN10.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN11.Text) >= lastX)
                {
                    lblN11.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN11.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN12.Text) >= lastX)
                {
                    lblN12.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN12.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN13.Text) >= lastX)
                {
                    lblN13.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN13.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN14.Text) >= lastX)
                {
                    lblN14.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN14.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN15.Text) >= lastX)
                {
                    lblN15.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN15.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN16.Text) >= lastX)
                {
                    lblN16.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN16.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN17.Text) >= lastX)
                {
                    lblN17.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN17.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN18.Text) >= lastX)
                {
                    lblN18.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN18.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN19.Text) >= lastX)
                {
                    lblN19.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN19.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN20.Text) >= lastX)
                {
                    lblN20.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN20.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN21.Text) >= lastX)
                {
                    lblN21.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN21.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN20.Text) >= lastX)
                {
                    lblN22.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN22.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN23.Text) >= lastX)
                {
                    lblN23.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN23.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN24.Text) >= lastX)
                {
                    lblN24.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN24.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN25.Text) >= lastX)
                {
                    lblN25.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN25.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN26.Text) >= lastX)
                {
                    lblN26.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN26.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN27.Text) >= lastX)
                {
                    lblN27.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN27.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN28.Text) >= lastX)
                {
                    lblN28.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN28.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN29.Text) >= lastX)
                {
                    lblN29.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN29.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN30.Text) >= lastX)
                {
                    lblN30.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN30.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN31.Text) >= lastX)
                {
                    lblN31.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN31.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN32.Text) >= lastX)
                {
                    lblN32.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN32.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN33.Text) >= lastX)
                {
                    lblN33.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN33.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN34.Text) >= lastX)
                {
                    lblN34.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN34.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN35.Text) >= lastX)
                {
                    lblN35.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN35.BackColor = System.Drawing.Color.Transparent;
                }

                if (Convert.ToInt16(lblN36.Text) >= lastX)
                {
                    lblN36.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblN36.BackColor = System.Drawing.Color.Transparent;
                }
            }
        }

        private void withoutLeavingFileReader()
        {
            securedPath();
            FileStream stream = new FileStream(path, FileMode.Open);
            StreamReader reader = new StreamReader(stream);
            string line = null; ;

            do
            {
                line = reader.ReadLine();
                if (line != null)
                {
                    updateWithoutLeavingArray(Convert.ToInt16(line));
                }
            } while (line != null);
            reader.Close();
            stream.Close();

        }

        private void clearGUI()
        {
            lstBoxLine.Items.Clear();
            lstBoxMed.Items.Clear();
            lstBoxPer.Items.Clear();
            txtMed.Clear();
            txtNbrPlay.Clear();
            txtPerPlay.Clear();
            txtWLeaving.Clear();
            txtWLeavingMax.Clear();
            txtMed.Clear();
            lblh0.Text = "*";
            lblh1.Text = "*";
            lblh2.Text = "*";
            lblh3.Text = "*";
            lblh4.Text = "*";
            lblh5.Text = "*";
            lblh6.Text = "*";
            lblh7.Text = "*";
            lblh8.Text = "*";
            lblh9.Text = "*";
            lblh10.Text = "*";
            lblh11.Text = "*";
            lblh12.Text = "*";
            lblh13.Text = "*";
            lblh14.Text = "*";
            lblh15.Text = "*";
            lblh16.Text = "*";
            lblh17.Text = "*";
            lblh18.Text = "*";
            lblh19.Text = "*";
            lblh20.Text = "*";
            lblh21.Text = "*";
            lblh22.Text = "*";
            lblh23.Text = "*";
            lblh24.Text = "*";
            lblh25.Text = "*";
            lblh26.Text = "*";
            lblDozen.Text = "Empate";
            lblLine.Text = "Empate";
            lblColor.Text = "Empate";
            lblEven.Text = "Empate";
            lblNumber.Text = "Empate";
            lblNbrReg.Text = "0";
            lblLenght.Text = "0";
        }

        private void tableAction(int number)
        {
            clearGUI();
            currentNumber = number;
            if (checkRead.Checked)
            {
                lblSelectedNumber.Text = number.ToString();
                updateStats(line => Convert.ToInt32(line) == currentNumber, currentScale);
            }
            else
            {
                lblSelectedNumber.Text = number.ToString();
                addReg(currentNumber);
                updateStats(line => Convert.ToInt32(line) == currentNumber, currentScale);

                updateWithoutLeavingArray(currentNumber);
                arrayWithoutLeavingToLabel();
                withoutLeavingColorLabel();
            }
        }

        private void dozenAction(char doz)
        {
            clearGUI();
            updateStats(line => checkDozen(Convert.ToInt32(line)) == doz, currentScale);

        }

        private void colorAction(bool col)
        {
            clearGUI();
            updateStats(line => checkColor(Convert.ToInt32(line)) == col, currentScale);
        }

        private void evenOddAction(bool even)
        {
            clearGUI();
            updateStats(line => checkEvenOdd(Convert.ToInt32(line)) == even,currentScale);
        }

        private void greaterLesserAction(bool great)
        {
            clearGUI();
            updateStats(line => checkGreaterOrLesser(Convert.ToInt32(line)) == great, currentScale); 
        }

        private void columnAction(char coln)
        {
            clearGUI();
            updateStats(line => checkColumn(Convert.ToInt32(line)) == coln, currentScale); 
        }
         

        //Eventos =================================================================================
        private void tabControl_KeyDown(object sender, KeyEventArgs e)
        {
            //Controlo da List Box
            if (e.KeyCode == Keys.Left && e.Control)
            {
                if (currentNumber > -1)
                {
                    if (currentScale > 0)
                    {
                        currentScale = currentScale - 10;
                        clearGUI();
                        updateStats(line => Convert.ToInt32(line) == currentNumber, currentScale);
                    }
                }
                else
                {
                    MessageBox.Show("Por favor selecione um número !");
                }
            }
            if (e.KeyCode == Keys.Right && e.Control)
            {
                if (currentNumber > -1)
                {
                    currentScale = currentScale + 10;
                    clearGUI();
                    updateStats(line => Convert.ToInt32(line) == currentNumber, currentScale);
                }
                else
                {
                    MessageBox.Show("Por favor selecione um número !");
                }
            }
            if (e.KeyCode == Keys.Up && e.Control)
            {
                if (currentNumber > -1)
                {
                    currentScale = 40;
                    clearGUI();
                    updateStats(line => Convert.ToInt32(line) == currentNumber, currentScale);
                }
                else
                {
                    MessageBox.Show("Tem que Selecionar Um Numero Primeiro");
                }
            }
            if (e.KeyCode == Keys.Down && e.Control)
            {
                if (currentNumber > -1)
                {
                    currentScale = 0;
                    clearGUI();
                    updateStats(line => Convert.ToInt32(line) == currentNumber, currentScale);
                }
                else
                {
                    MessageBox.Show("Tem que Selecionar Um Numero Primeiro");
                }
            }


            //Controlo dos Botões Radio
            if (e.KeyCode == Keys.Z && e.Control)
            {
                radBtnDuz.Checked = true;
            }
            if (e.KeyCode == Keys.X && e.Control)
            {
                radBtnLine.Checked = true;
            }
            if (e.KeyCode == Keys.C && e.Control)
            {
                radBtnColor.Checked = true;
            }
            if (e.KeyCode == Keys.V && e.Control)
            {
                radBtnOdd.Checked = true;
            }
            if (e.KeyCode == Keys.B && e.Control)
            {
                radBtnNum.Checked = true;
            }

            //Control Modo Leitura
            if (e.KeyCode == Keys.A && e.Control)
            {
                if (checkRead.Checked)
                {
                    checkRead.Checked = false;
                }
                else
                {
                    checkRead.Checked = true;
                }
            }
        }

        private void radBtnDuz_CheckedChanged(object sender, EventArgs e)
        {
            graphDozen();
        }

        private void radBtnLine_CheckedChanged(object sender, EventArgs e)
        {
            graphLine();
        }

        private void radBtnColor_CheckedChanged(object sender, EventArgs e)
        {
            graphColor();
        }

        private void radBtnOdd_CheckedChanged(object sender, EventArgs e)
        {
            graphEven();
        }

        private void radBtnNum_CheckedChanged(object sender, EventArgs e)
        {
            graphNumber();
        }

        //EVENTOS DE BOTÕES DE MESAS
        private void btnDouble0_Click(object sender, EventArgs e)
        {
            tableAction(37);
        }

        private void btn0_Click(object sender, EventArgs e)
        {
            tableAction(0);
        }

        private void btn1_Click(object sender, EventArgs e)
        {
            tableAction(1);
        }

        private void btn2_Click(object sender, EventArgs e)
        {
            tableAction(2);
        }

        private void btn3_Click(object sender, EventArgs e)
        {
            tableAction(3);
        }

        private void btn4_Click(object sender, EventArgs e)
        {
            tableAction(4);
        }

        private void btn5_Click(object sender, EventArgs e)
        {
            tableAction(5);
        }

        private void btn6_Click(object sender, EventArgs e)
        {
            tableAction(6);
        }

        private void btn7_Click(object sender, EventArgs e)
        {
            tableAction(7);
        }

        private void btn8_Click(object sender, EventArgs e)
        {
            tableAction(8);
        }

        private void btn9_Click(object sender, EventArgs e)
        {
            tableAction(9);
        }

        private void btn10_Click(object sender, EventArgs e)
        {
            tableAction(10);
        }

        private void btn11_Click(object sender, EventArgs e)
        {
            tableAction(11);
        }

        private void btn12_Click(object sender, EventArgs e)
        {
            tableAction(12);
        }

        private void btn13_Click(object sender, EventArgs e)
        {
            tableAction(13);
        }

        private void btn14_Click(object sender, EventArgs e)
        {
            tableAction(14);
        }

        private void btn15_Click(object sender, EventArgs e)
        {
            tableAction(15);
        }

        private void btn16_Click(object sender, EventArgs e)
        {
            tableAction(16);
        }

        private void btn17_Click(object sender, EventArgs e)
        {
            tableAction(17);
        }

        private void btn18_Click(object sender, EventArgs e)
        {
            tableAction(18);
        }

        private void btn19_Click(object sender, EventArgs e)
        {
            tableAction(19);
        }

        private void btn20_Click(object sender, EventArgs e)
        {
            tableAction(20);
        }

        private void btn21_Click(object sender, EventArgs e)
        {
            tableAction(21);
        }

        private void btn22_Click(object sender, EventArgs e)
        {
            tableAction(22);
        }

        private void btn23_Click(object sender, EventArgs e)
        {
            tableAction(23);
        }

        private void btn24_Click(object sender, EventArgs e)
        {
            tableAction(24);
        }

        private void btn25_Click(object sender, EventArgs e)
        {
            tableAction(25);
        }

        private void btn26_Click(object sender, EventArgs e)
        {
            tableAction(26);
        }

        private void btn27_Click(object sender, EventArgs e)
        {
            tableAction(27);
        }

        private void btn28_Click(object sender, EventArgs e)
        {
            tableAction(28);
        }

        private void btn29_Click(object sender, EventArgs e)
        {
            tableAction(29);
        }

        private void btn30_Click(object sender, EventArgs e)
        {
            tableAction(30);
        }

        private void btn31_Click(object sender, EventArgs e)
        {
            tableAction(31);
        }

        private void btn32_Click(object sender, EventArgs e)
        {
            tableAction(32);
        }

        private void btn33_Click(object sender, EventArgs e)
        {
            tableAction(33);
        }

        private void btn34_Click(object sender, EventArgs e)
        {
            tableAction(34);
        }

        private void btn35_Click(object sender, EventArgs e)
        {
            tableAction(35);
        }

        private void btn36_Click(object sender, EventArgs e)
        {
            tableAction(36);
        }

        private void btn1st_Click(object sender, EventArgs e)
        {
            dozenAction('1'); 
        }

        private void btn2st_Click(object sender, EventArgs e)
        {
            dozenAction('2'); 
        }

        private void btn3st_Click(object sender, EventArgs e)
        {
            dozenAction('3'); 
        }

        private void btnAddDB_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Title = "Novo Registo";
            saveFile.AddExtension = false;
            saveFile.Filter = "Las Vegas File | *sav";
            saveFile.DefaultExt = ".sav";
            saveFile.InitialDirectory = Path.Combine(Application.StartupPath, "data");
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                if (saveFile.FileName != null)
                {
                    FileStream stream = new FileStream(saveFile.FileName.ToString(), FileMode.Create);
                    stream.Close();
                    path = saveFile.FileName;
                    clearGUI();
                }
            }
        }

        private void btnLoadDB_Click(object sender, EventArgs e)
        {
                OpenFileDialog openFile = new OpenFileDialog();
                openFile.Title = "Abrir Registo";
                openFile.Filter = "Las Vegas File | *.sav";
                openFile.InitialDirectory = Path.Combine(Application.StartupPath,"data");

                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    if (openFile.FileName != null)
                    {
                        path = openFile.FileName;
                        withoutLeavingFileReader();
                        arrayWithoutLeavingToLabel();
                    }
                }
        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            frmAboutBox about = new frmAboutBox();
            about.Show();
        }

        private void btnEven_Click(object sender, EventArgs e)
        {
            evenOddAction(true);
        }

        private void btnOdd_Click(object sender, EventArgs e)
        {
            evenOddAction(false);
        }

        private void btn1_18_Click(object sender, EventArgs e)
        {
            greaterLesserAction(true);
        }

        private void btn19_36_Click(object sender, EventArgs e)
        {
            greaterLesserAction(false);
        }

        private void btnRed_Click(object sender, EventArgs e)
        {
            colorAction(false);
        }

        private void btnBlack_Click(object sender, EventArgs e)
        {
            colorAction(true);
        }

        private void btn1line_Click(object sender, EventArgs e)
        {
            columnAction('1');
        }

        private void btn2line_Click(object sender, EventArgs e)
        {
            columnAction('2');
        }

        private void btn3line_Click(object sender, EventArgs e)
        {
            columnAction('3');
        }

        private void comboBoxWLeaving_SelectedIndexChanged(object sender, EventArgs e)
        {
            withoutLeavingColorLabel();
        }

    }
}
