namespace KursShmakovAKITP
{
    public partial class Form1 : Form
    {
        string sizeFieldCell; // Размерность ячейки поля
        const int HEIGHT_CELLS = 23; // Исходная(минимальная) высота ячейки поля
        const int CELL_HEIGHT_INC = 20; // Прирощение высоты ячейки поля 
        int sizeFieldCellPixels; // Размерность ячейки в пикселях
        public static int sizeField; // Размер поля, задает пользователь
        public int Ai, Aj, Bi, Bj; // Соответствующие координаты точки A(начала) и B(конца)
        public static int[,] map = new int[1000, 1000]; // Карта поля
        private List<int> AiTemp = new(); // Список временных координат строк
        private List<int> AjTemp = new();// Список временных координат столбцов
        private List<int> BiTemp = new(); // Список временных координат строк
        private List<int> BjTemp = new(); // Список временных координат столбцов
        readonly int[] posl = { 1, 1, 2, 2 }; // Последовательность распространения волны
        int totalCellsViewed = 0; // Всего просмотренных ячеек
        int pathLength = 1; // Длина пути
        public static bool readfile = false;
        string filename = "[" + DateTime.Now.ToString().Replace('/', '-').Replace(':', '-') + "]Map.txt";

        // Элементы управления для выбора размещения требуемого элемента на поле
        RadioButton addA = new RadioButton();
        RadioButton addB = new RadioButton();
        RadioButton addX = new RadioButton();

        /*
         * Задание параметров окна в зависимости от количества ячеек
        */
        private void BuildingWindow() {
            // Установка размера программы
            this.Width = (sizeField * sizeFieldCellPixels) + 18;
            this.Height = (sizeField * sizeFieldCellPixels) + 47;
        }

        /*
         * Построение поля
       */
        private void BuildingField() 
        {

            // Настройки поля
            Field.RowCount = sizeField;
            Field.ColumnCount = sizeField;
            Field.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            Field.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            // Выбор размера ячейки в зависимости от их количества
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

            // Рассчитываем значение размера ячеек
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
                // Заполняем поле пустыми ячейками
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
         * Обработка событий нажатия на клавиши для выбора действия
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
                    MessageBox.Show("Установите начальную точку (А) и конечную точку (В)!");
                }
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Разрешаем ставить начальную ячейку самой первой
            addA.Checked = true;
        }

        /*
         * Событие нажатия мышкой на ячейку
         */
        private void MarkingField(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            int j = e.ColumnIndex;

            // Если читаем из файла добавляем только препядствия
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
         * Распространение волны 
         * Волна распространяется по 2 шага, 11,22...
         */
        public void WavePropagation()
        {   
            int waveSequenceCounter = 0; // Счетчик последовательности распространения волны
            bool stop = false; // Флаг остановки распространения волны
            // Начальные координаты распространения волны ячейки A
            AiTemp.Add(Ai);
            AjTemp.Add(Aj);

            // Пока не распростриним волну до конца или не дойдем до тупика
            while (!stop)
            {
                // Распространение одной волны
                for (int i = 0; i < AiTemp.Count; i++)
                {
                    // Шаг Вверх
                    if (CheckСell(AiTemp[i] - 1, AjTemp[i], false))
                    {
                        // Если подошли к финишу заканчиваем распространение
                        if (map[AiTemp[i] - 1, AjTemp[i]] == 8)
                        {
                            stop = true;
                            break;
                        }
                        // Присваиваем ячейке номер в соответствии с последовательностью
                        map[AiTemp[i] - 1, AjTemp[i]] = posl[waveSequenceCounter];
                        // Добавляем пройденную ячейку во временный список, для распространения от нее волны
                        // на следующем шаге
                        BiTemp.Add(AiTemp[i] - 1);
                        BjTemp.Add(AjTemp[i]);
                        // Подсчитываем количество пройденных ячеек
                        totalCellsViewed++;
                    }
                    // Шаг Вправо
                    if (CheckСell(AiTemp[i], AjTemp[i] + 1, false))
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
                    // Шаг Вниз
                    if (CheckСell(AiTemp[i] + 1, AjTemp[i], false))
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
                    // Шаг Влево
                    if (CheckСell(AiTemp[i], AjTemp[i] - 1, false))
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
                // Если шаги некуда больше сделать, значит попали в тупик
                if (AiTemp.Count == 0) {
                    MessageBox.Show("Тупик!");
                    break;                 
                }
                // Меняем списки временных координат распространения волны пройденной, на новую
                FlippingLists(AiTemp, AjTemp, BiTemp, BjTemp);

                // Если успешно распространили волну и дошли до финишной ячейки запускаем функцию восстановления пути
                if (stop)
                {
                    AiTemp.Clear();
                    AjTemp.Clear();
                    RestoringPath(waveSequenceCounter);
                }

                waveSequenceCounter++; // Увеличиваем значение индекса последовательности распространения волны
                // Если индекс = 4, значит одну волну распространили и начинаем заного
                if (waveSequenceCounter == 4) {waveSequenceCounter = 0;}          
            }
        }

        /*
         * Функция смены временных списков координат
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
         * Функция восстановления пути в качестве параметра принимает значение счетчика распространения волны
         * на котором остановилась последняя волна
         */
        public void RestoringPath(int waveSequenceCounter)
        {
            // Начальные координаты восстановления пути от финишной ячейки B
            AiTemp.Add(Bi);
            AjTemp.Add(Bj);
            bool stop = false; // Флаг остановки восстановления пути

            // Если волна закончила распространяться на первой единице [1]122, то предыдущая ячейка начинается с [2]
            if (waveSequenceCounter == 0)
            {
                waveSequenceCounter = 2;
            }
            // Если волна закончила распространяться на первой двойке 11[2]2, то предыдущая ячейка начинается с [1]
            else if (waveSequenceCounter == 2)
            {
                waveSequenceCounter = 0;
            }
            
            // Заполняем поле значениями в соответствии с ячейками
            // 0 - пустая 1 - первый фронт волны 2 - второй фронт волны 3 - препядствие
            // 8 - Финиш 9 - Начало
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
            // Пока не дошли до начала
            while (!stop)
            {             
                pathLength++; // Подсчитываем путь

                for (int i = 0; i < 1; i++)
                {
                    // Проверяем ячейку сверху
                    if (CheckСell(AiTemp[i] - 1, AjTemp[i], true))
                    {
                        // Уже дошли до начала останавливаемся
                        if (map[AiTemp[i] - 1, AjTemp[i]] == 9)
                        {
                            stop = true;
                            break;
                        }
                        // Сравниваем просматриваемую ячейку с заданной последовательностью
                        if (map[AiTemp[i] - 1, AjTemp[i]] == posl[waveSequenceCounter])
                        {
                            Field.Rows[AiTemp[i] - 1].Cells[AjTemp[i]].Style.BackColor = Color.Orange;
                            BiTemp.Add(AiTemp[i] - 1);
                            BjTemp.Add(AjTemp[i]);

                            FlippingLists(AiTemp, AjTemp, BiTemp, BjTemp);
                            waveSequenceCounter++; // Прибавляем счетчик последовательности

                            // Счетчик последовательности в начало если всю прошли
                            if (waveSequenceCounter == 4)
                            {
                                waveSequenceCounter = 0;
                            }
                            break;                          
                        }
                    }

                    // Проверяем ячейку справа
                    if (CheckСell(AiTemp[i], AjTemp[i] + 1, true))
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

                    // Проверяем ячейку снизу
                    if (CheckСell(AiTemp[i] + 1, AjTemp[i], true))
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

                    // Проверяем ячейку Слева
                    if (CheckСell(AiTemp[i], AjTemp[i] - 1, true))
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
                MessageBox.Show("Путь найден!\nДлина пути: " + pathLength + 
                    "\nПросмотрено ячеек: " + totalCellsViewed + "\nКарта сохранена в data/" + filename);
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
            MessageBox.Show("Для распространения волны нажмите F.\n" +
                "Для построение нового поля закройте окно.");
        }

        /*
         * Функция проверки не выходит ли рассматриваемая ячейка за границы поля
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
         * Функция проверки на возможность движения в следующую ячейку при распространении волны
         * и восстановлении пути (reverseDirection true).
         */
        public bool CheckСell(int i, int j, bool reverseDirection)
        {
            bool flag = false;

            if (reverseDirection)
            {
                if (FieldBound(i, j))
                {
                    // При восстановлении пути ячейка не должна быть стеной,пустой и конечной
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
                    // При распространении волны ячейка не должна быть стеной,началом и просмотренной
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
