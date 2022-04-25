namespace KursShmakovAKITP
{
    public partial class Form1 : Form
    {
        string sizeFieldCell; // ����������� ������ ����
        const int HEIGHT_CELLS = 23; // ��������(�����������) ������ ������ ����
        const int CELL_HEIGHT_INC = 20; // ���������� ������ ������ ���� 
        int sizeFieldCellPixels; // ����������� ������ � ��������
        public static int sizeField; // ������ ����, ������ ������������
        public int Ai, Aj, Bi, Bj; // ��������������� ���������� ����� A(������) � B(�����)
        public static int[,] map = new int[1000, 1000]; // ����� ����
        private List<int> AiTemp = new(); // ������ ��������� ��������� �����
        private List<int> AjTemp = new();// ������ ��������� ��������� ��������
        private List<int> BiTemp = new(); // ������ ��������� ��������� �����
        private List<int> BjTemp = new(); // ������ ��������� ��������� ��������
        readonly int[] posl = { 1, 1, 2, 2 }; // ������������������ ��������������� �����
        int totalCellsViewed = 0; // ����� ������������� �����
        int pathLength = 1; // ����� ����
        public static bool readfile = false;
        string filename = "[" + DateTime.Now.ToString().Replace('/', '-').Replace(':', '-') + "]Map.txt";

        // �������� ���������� ��� ������ ���������� ���������� �������� �� ����
        RadioButton addA = new RadioButton();
        RadioButton addB = new RadioButton();
        RadioButton addX = new RadioButton();

        /*
         * ������� ���������� ���� � ����������� �� ���������� �����
        */
        private void BuildingWindow() {
            // ��������� ������� ���������
            this.Width = (sizeField * sizeFieldCellPixels) + 18;
            this.Height = (sizeField * sizeFieldCellPixels) + 47;
        }

        /*
         * ���������� ����
       */
        private void BuildingField() 
        {

            // ��������� ����
            Field.RowCount = sizeField;
            Field.ColumnCount = sizeField;
            Field.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            Field.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            // ����� ������� ������ � ����������� �� �� ����������
            if(sizeField <= 5){
                sizeFieldCell = "\n\n\n";
            }
            else if (sizeField > 5 && sizeField <= 10) {
                sizeFieldCell = "\n\n";
            }
            else if (sizeField > 10 && sizeField <= 20) {
                sizeFieldCell = "\n";
            }
            else if(sizeField > 20) {
                sizeFieldCell = "";
            }

            // ������������ �������� ������� �����
            sizeFieldCellPixels = (HEIGHT_CELLS + CELL_HEIGHT_INC * sizeFieldCell.Length);

            if (readfile)
            {
                for (int i = 0; i < sizeField; ++i)
                {
                    for (int j = 0; j < sizeField; ++j)
                    {
                        if(map[i, j] == 9)
                        {
                            Ai = i; Aj = j;
                        }
                        if(map[i, j] == 8)
                        {
                            Bi = i; Bj = j;
                        }
                        Field.Rows[i].Cells[j].Value = sizeFieldCell + map[i, j];                
                    }
                }

            }
            else
            {
                // ��������� ���� ������� ��������
                for (int i = 0; i < sizeField; i++)
                    for (int j = 0; j < sizeField; j++)
                        Field.Rows[i].Cells[j].Value = sizeFieldCell;
            }

        }
        public Form1()
        {
            InitializeComponent();
            BuildingField();
            BuildingWindow();
            Field.DefaultCellStyle.SelectionBackColor = Color.White;
        }

        /*
         * ��������� ������� ������� �� ������� ��� ������ ��������
         */
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.F))
            {
                if((!addA.Checked && !addB.Checked) || readfile) 
                {
                    addX.Checked = false;
                    Field.DefaultCellStyle.SelectionBackColor = Color.Transparent;
                    WavePropagation();                   

                } else 
                {
                    MessageBox.Show("���������� ��������� ����� (�) � �������� ����� (�)!");
                }
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // ��������� ������� ��������� ������ ����� ������
            addA.Checked = true;
        }

        /*
         * ������� ������� ������ �� ������
         */
        private void MarkingField(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            int j = e.ColumnIndex;

            // ���� ������ �� ����� ��������� ������ �����������
            if(readfile)
            {
                addA.Checked = false;
                addB.Checked = false;
                addX.Checked = true;
            }

            if (addX.Checked)
            {
                Field.DefaultCellStyle.SelectionBackColor = Color.Black;
                Field.Rows[i].Cells[j].Style.BackColor = Color.Black;
                Field.Rows[i].Cells[j].Selected = false;
                map[i, j] = 3;
            }
            if (addB.Checked)
            {
                Field.DefaultCellStyle.SelectionBackColor = Color.Red;
                Field.Rows[i].Cells[j].Style.BackColor = Color.Red;
                Field.Rows[i].Cells[j].Selected = false;
                Field.DefaultCellStyle.SelectionBackColor = Color.Black;
                addB.Checked = false;
                addX.Checked = true;
                map[i, j] = 8;
                Bi = i;
                Bj = j;

            }
            if (addA.Checked)
            {
                Field.Rows[i].Cells[j].Style.BackColor = Color.Green;
                Field.DefaultCellStyle.SelectionBackColor = Color.Green;
                Field.Rows[i].Cells[j].Selected = false;
                Field.DefaultCellStyle.SelectionBackColor = Color.Red;
                addA.Checked = false;
                addB.Checked = true;
                map[i, j] = 9;
                Ai = i;
                Aj = j;
            }
        }

        /* 
         * ��������������� ����� 
         * ����� ���������������� �� 2 ����, 11,22...
         */
        public void WavePropagation()
        {   
            int waveSequenceCounter = 0; // ������� ������������������ ��������������� �����
            bool stop = false; // ���� ��������� ��������������� �����
            // ��������� ���������� ��������������� ����� ������ A
            AiTemp.Add(Ai);
            AjTemp.Add(Aj);

            // ���� �� ������������� ����� �� ����� ��� �� ������ �� ������
            while (!stop)
            {
                // ��������������� ����� �����
                for (int i = 0; i < AiTemp.Count; i++)
                {
                    // ��� �����
                    if (Check�ell(AiTemp[i] - 1, AjTemp[i], false))
                    {
                        // ���� ������� � ������ ����������� ���������������
                        if (map[AiTemp[i] - 1, AjTemp[i]] == 8)
                        {
                            stop = true;
                            break;
                        }
                        // ����������� ������ ����� � ������������ � �������������������
                        map[AiTemp[i] - 1, AjTemp[i]] = posl[waveSequenceCounter];
                        // ��������� ���������� ������ �� ��������� ������, ��� ��������������� �� ��� �����
                        // �� ��������� ����
                        BiTemp.Add(AiTemp[i] - 1);
                        BjTemp.Add(AjTemp[i]);
                        // ������������ ���������� ���������� �����
                        totalCellsViewed++;
                    }
                    // ��� ������
                    if (Check�ell(AiTemp[i], AjTemp[i] + 1, false))
                    {
                        if (map[AiTemp[i], AjTemp[i] + 1] == 8)
                        {
                            stop = true;
                            break;
                        }
                        map[AiTemp[i], AjTemp[i] + 1] = posl[waveSequenceCounter];
                        BiTemp.Add(AiTemp[i]);
                        BjTemp.Add(AjTemp[i] + 1);
                        totalCellsViewed++;
                    }
                    // ��� ����
                    if (Check�ell(AiTemp[i] + 1, AjTemp[i], false))
                    {
                        if (map[AiTemp[i] + 1, AjTemp[i]] == 8)
                        {
                            stop = true;
                            break;
                        }
                        map[AiTemp[i] + 1, AjTemp[i]] = posl[waveSequenceCounter];
                        BiTemp.Add(AiTemp[i] + 1);
                        BjTemp.Add(AjTemp[i]);
                        totalCellsViewed++;
                    }
                    // ��� �����
                    if (Check�ell(AiTemp[i], AjTemp[i] - 1, false))
                    {
                        if (map[AiTemp[i], AjTemp[i] - 1] == 8)
                        {
                            stop = true;
                            break;
                        }
                        map[AiTemp[i], AjTemp[i] - 1] = posl[waveSequenceCounter];
                        BiTemp.Add(AiTemp[i]);
                        BjTemp.Add(AjTemp[i] - 1);
                        totalCellsViewed++;
                    }
                }
                // ���� ���� ������ ������ �������, ������ ������ � �����
                if (AiTemp.Count == 0) {
                    MessageBox.Show("�����!");
                    break;                 
                }
                // ������ ������ ��������� ��������� ��������������� ����� ����������, �� �����
                FlippingLists(AiTemp, AjTemp, BiTemp, BjTemp);

                // ���� ������� �������������� ����� � ����� �� �������� ������ ��������� ������� �������������� ����
                if (stop)
                {
                    AiTemp.Clear();
                    AjTemp.Clear();
                    RestoringPath(waveSequenceCounter);
                }

                waveSequenceCounter++; // ����������� �������� ������� ������������������ ��������������� �����
                // ���� ������ = 4, ������ ���� ����� �������������� � �������� ������
                if (waveSequenceCounter == 4) {waveSequenceCounter = 0;}          
            }
        }

        /*
         * ������� ����� ��������� ������� ���������
         */
        public void FlippingLists(List<int> AiTemp, List<int> AjTemp, List<int> BiTemp, List<int> BjTemp)
        {
            AiTemp.Clear();
            AjTemp.Clear();
            AiTemp.AddRange(BiTemp);
            AjTemp.AddRange(BjTemp);
            BiTemp.Clear();
            BjTemp.Clear();
        }

        /*
         * ������� �������������� ���� � �������� ��������� ��������� �������� �������� ��������������� �����
         * �� ������� ������������ ��������� �����
         */
        public void RestoringPath(int waveSequenceCounter)
        {
            // ��������� ���������� �������������� ���� �� �������� ������ B
            AiTemp.Add(Bi);
            AjTemp.Add(Bj);
            bool stop = false; // ���� ��������� �������������� ����

            // ���� ����� ��������� ���������������� �� ������ ������� [1]122, �� ���������� ������ ���������� � [2]
            if (waveSequenceCounter == 0)
            {
                waveSequenceCounter = 2;
            }
            // ���� ����� ��������� ���������������� �� ������ ������ 11[2]2, �� ���������� ������ ���������� � [1]
            else if (waveSequenceCounter == 2)
            {
                waveSequenceCounter = 0;
            }
            
            // ��������� ���� ���������� � ������������ � ��������
            // 0 - ������ 1 - ������ ����� ����� 2 - ������ ����� ����� 3 - �����������
            // 8 - ����� 9 - ������
            for (int i = 0; i < sizeField; ++i)
            {
                for (int j = 0; j < sizeField; ++j)
                {
                    Field.Rows[i].Cells[j].Value = sizeFieldCell + map[i, j];

                    if (map[i, j] == 1)
                    {
                        Field.Rows[i].Cells[j].Style.BackColor = Color.Olive;
                    }
                    if (map[i, j] == 2)
                    {
                        Field.Rows[i].Cells[j].Style.BackColor = Color.Olive;
                    }
                    if (map[i, j] == 3)
                    {
                        Field.Rows[i].Cells[j].Style.BackColor = Color.Black;
                    }
                    if (map[i, j] == 8)
                    {
                        Field.Rows[i].Cells[j].Selected = false;
                        Field.Rows[i].Cells[j].Value = sizeFieldCell + "B";
                        Field.Rows[i].Cells[j].Style.BackColor = Color.Orange;
                    }
                    if (map[i, j] == 9)
                    {
                        Field.Rows[i].Cells[j].Value = sizeFieldCell + "A";
                        Field.Rows[i].Cells[j].Style.BackColor = Color.Orange;
                    }
                }
            }
            // ���� �� ����� �� ������
            while (!stop)
            {             
                pathLength++; // ������������ ����

                for (int i = 0; i < 1; i++)
                {
                    // ��������� ������ ������
                    if (Check�ell(AiTemp[i] - 1, AjTemp[i], true))
                    {
                        // ��� ����� �� ������ ���������������
                        if (map[AiTemp[i] - 1, AjTemp[i]] == 9)
                        {
                            stop = true;
                            break;
                        }
                        // ���������� ��������������� ������ � �������� �������������������
                        if (map[AiTemp[i] - 1, AjTemp[i]] == posl[waveSequenceCounter])
                        {
                            Field.Rows[AiTemp[i] - 1].Cells[AjTemp[i]].Style.BackColor = Color.Orange;
                            BiTemp.Add(AiTemp[i] - 1);
                            BjTemp.Add(AjTemp[i]);

                            FlippingLists(AiTemp, AjTemp, BiTemp, BjTemp);
                            waveSequenceCounter++; // ���������� ������� ������������������

                            // ������� ������������������ � ������ ���� ��� ������
                            if (waveSequenceCounter == 4)
                            {
                                waveSequenceCounter = 0;
                            }
                            break;                          
                        }
                    }

                    // ��������� ������ ������
                    if (Check�ell(AiTemp[i], AjTemp[i] + 1, true))
                    {
                        if (map[AiTemp[i], AjTemp[i] + 1] == 9)
                        {
                            stop = true;
                            break;
                        }

                        if (map[AiTemp[i], AjTemp[i] + 1] == posl[waveSequenceCounter])
                        {
                            Field.Rows[AiTemp[i]].Cells[AjTemp[i] + 1].Style.BackColor = Color.Orange;
                            BiTemp.Add(AiTemp[i]);
                            BjTemp.Add(AjTemp[i] + 1);

                            FlippingLists(AiTemp, AjTemp, BiTemp, BjTemp);
                            waveSequenceCounter++;

                            if (waveSequenceCounter == 4)
                            {
                                waveSequenceCounter = 0;
                            }
                            break;                   
                        }
                    }

                    // ��������� ������ �����
                    if (Check�ell(AiTemp[i] + 1, AjTemp[i], true))
                    {
                        if (map[AiTemp[i] + 1, AjTemp[i]] == 9)
                        {
                            stop = true;
                            break;
                        }

                        if (map[AiTemp[i] + 1, AjTemp[i]] == posl[waveSequenceCounter])
                        {
                            Field.Rows[AiTemp[i]  + 1].Cells[AjTemp[i]].Style.BackColor = Color.Orange;
                            BiTemp.Add(AiTemp[i] + 1);
                            BjTemp.Add(AjTemp[i]);

                            FlippingLists(AiTemp, AjTemp, BiTemp, BjTemp);
                            waveSequenceCounter++;

                            if (waveSequenceCounter == 4)
                            {
                                waveSequenceCounter = 0;
                            }
                            break;                         
                        }
                    }

                    // ��������� ������ �����
                    if (Check�ell(AiTemp[i], AjTemp[i] - 1, true))
                    {
                        if (map[AiTemp[i], AjTemp[i] - 1] == 9)
                        {
                            stop = true;
                            break;
                        }

                        if (map[AiTemp[i], AjTemp[i] - 1] == posl[waveSequenceCounter])
                        {                         
                            Field.Rows[AiTemp[i]].Cells[AjTemp[i] - 1].Style.BackColor = Color.Orange;
                            BiTemp.Add(AiTemp[i]);
                            BjTemp.Add(AjTemp[i] - 1);

                            FlippingLists(AiTemp, AjTemp, BiTemp, BjTemp);
                            waveSequenceCounter++;

                            if (waveSequenceCounter == 4)
                            {
                                waveSequenceCounter = 0;
                            }
                            break;                      
                        }
                    }
                }
            }

            if(stop)
            {
                MessageBox.Show("���� ������!\n����� ����: " + pathLength + 
                    "\n����������� �����: " + totalCellsViewed + "\n����� ��������� � data/" + filename);
                SaveMapFile();
            }
        }

        private void SaveMapFile()
        {      
            using StreamWriter sw = new("data/" + filename);

            for (int i = 0; i < sizeField; i++)
            {
                for (int j = 0; j < sizeField; j++)
                {
                    if (map[i,j] != 2 && map[i, j] != 1)
                    {
                        sw.WriteLine(String.Format("{0}|{1}|{2}|", i, j, map[i, j]));
                    }
                }
            }
            sw.Close();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Ai = 0;
            Aj = 0;
            Bi = 0;
            Bj = 0;
            for (int i = 0; i < sizeField; i++)
                for (int j = 0; j < sizeField; j++)
                    map[i, j] = 0;
        }

        private void Form1_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBox.Show("��� ��������������� ����� ������� F.\n" +
                "��� ���������� ������ ���� �������� ����.");
        }

        /*
         * ������� �������� �� ������� �� ��������������� ������ �� ������� ����
         */
        public static bool FieldBound(int i, int j)
        {
            bool flag = false;
            if (j >= 0 && j < sizeField & i >= 0 && i < sizeField)
            {
                flag = true;
            }

            return flag;
        }
        /*
         * ������� �������� �� ����������� �������� � ��������� ������ ��� ��������������� �����
         * � �������������� ���� (reverseDirection true).
         */
        public bool Check�ell(int i, int j, bool reverseDirection)
        {
            bool flag = false;

            if (reverseDirection)
            {
                if (FieldBound(i, j))
                {
                    // ��� �������������� ���� ������ �� ������ ���� ������,������ � ��������
                    if (map[i, j] != 3 && map[i, j] != 0 && map[i, j] != 8)
                    {
                        flag = true;
                    }
                }
            }
            else
            {
                if (FieldBound(i, j))
                {
                    // ��� ��������������� ����� ������ �� ������ ���� ������,������� � �������������
                    if (map[i, j] != 3 && map[i, j] != 9 && map[i, j] != 1
                           && map[i, j] != 2)
                    {
                        flag = true;
                    }
                }
            }
            return flag;
        }
    }
}
