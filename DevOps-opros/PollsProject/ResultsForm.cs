using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PollsProject
{
    public partial class ResultsForm : Form
    {
        List<Question> questions = new List<Question>();                // список вопросов
        List<List<List<int>>> answers = new List<List<List<int>>>();    // список номеров ответов по каждому из списка вопросов по каждому из списка пользователей
        int num = 0;                                                    // номер текущего вопроса

        public ResultsForm()
        {
            InitializeComponent();
            textBox1.Text = Program.fnamepath;
        }

        // метод для построения круговой диаграммы со статистикой ответов на вопрос с номером num
        void GetStatistics(int num)
        {
            // вывод текста вопроса на невидимую метку (используется далее в качестве заголовка диаграммы)
            label2.Text = questions[num].text;

            // подсчет количества ответов по всем пользователям:
            List<int> counts = new List<int>();
            for (int j = 0; j < questions[num].vars.Count; j++)
            {
                int cnt = 0;
                for (int k = 0; k < answers.Count; k++)
                    if (answers[k].ElementAt(num).Count(x => x == j + 1) > 0)
                        cnt++;
                counts.Add(cnt);
            }

            // удалить варианты с нулевым количеством ответов
            var X = questions[num].vars.Select(x => x).Where(x => counts[questions[num].vars.IndexOf(x)] != 0).ToArray();
            var Y = counts.Select(x => x).Where(x => x != 0).ToArray(); 

            chart1.Series.Clear();

            // форматировать диаграмму
            chart1.BackColor = Color.Gray;
            chart1.BackSecondaryColor = Color.WhiteSmoke;
            chart1.BackGradientStyle = GradientStyle.DiagonalRight;

            chart1.BorderlineDashStyle = ChartDashStyle.Solid;
            chart1.BorderlineColor = Color.Gray;
            chart1.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;

            // форматировать область диаграммы
            chart1.ChartAreas[0].BackColor = Color.Wheat;

            // добавить и форматировать заголовок
            chart1.Titles.Clear();
            chart1.Titles.Add(label2.Text);

            chart1.Series.Add(new Series("ColumnSeries")
            {
                ChartType = SeriesChartType.Pie
            });

            // отобразить данные
            chart1.Series["ColumnSeries"].XValueMember = "Name";
            chart1.Series["ColumnSeries"].YValueMembers = "Count";
            chart1.Series["ColumnSeries"].IsValueShownAsLabel = true;
            chart1.Series["ColumnSeries"].Points.DataBindXY(X, Y);
            chart1.ChartAreas[0].Area3DStyle.Enable3D = true;
        }

        // событие, возникающее при нажатии на кнопку "Выбрать опрос"
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Выберите текстовый файл";
            theDialog.Filter = "TXT files|*.txt";
            theDialog.InitialDirectory = Path.Combine(Directory.GetCurrentDirectory(), "..");
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // загрузить вопросы
                    string filename = theDialog.FileName;
                    string[] quests = File.ReadAllLines(filename);
                    textBox1.Text = Path.Combine(theDialog.InitialDirectory,
                        Path.GetFileNameWithoutExtension(filename));
                    // загрузить информацию о директории с сохраненными ответами
                    DirectoryInfo dirInfo = new DirectoryInfo(textBox1.Text);
                    // обработать строки и добавить вопросы в список
                    foreach (string str in quests)
                    {
                        string[] fields = str.Split(';').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                        List<string> vars = new List<string>();
                        for (int i = 1; i < fields.Length - 2; i++)
                            vars.Add(fields[i]);
                        Question q = new Question(fields[0], vars,
                            bool.Parse(fields[fields.Length - 2]),
                            bool.Parse(fields[fields.Length - 1]));
                        questions.Add(q);
                    }
                    // по каждому файлу в директории ответов (каждый текстовый файл - список ответов одного пользователя) 
                    foreach (FileInfo fi in dirInfo.GetFiles())
                    {
                        // если это текстовый файл, то
                        if (fi.Extension.ToUpper().Equals(".TXT"))
                        {
                            List<List<int>> user = new List<List<int>>();       // список списков номеров вариантов ответов пользователя по каждому вопросу
                            List<string> answs = new List<string>();            // список ответов в строковом виде
                            // считать информацию об ответах из файла и добавить в список ответов-строк:
                            string line = "";
                            StreamReader sr = new StreamReader(fi.FullName);
                            while ((line = sr.ReadLine()) != null)
                                answs.Add(line);
                            sr.Close();
                            // обаботать каждую строку:
                            foreach (var str in answs)
                            {
                                // если строка не пустая, то получить из нее номера вариантов ответов на текущий вопрос по текущему пользователю
                                if (!string.IsNullOrWhiteSpace(str))
                                {
                                    List<int> ans = new List<int>();
                                    string[] nums = str.Split(' ');
                                    foreach (var n in nums)
                                        if (!string.IsNullOrEmpty(n))
                                            ans.Add(int.Parse(n));
                                    user.Add(ans);
                                }
                                // для пустых строк добавить 0
                                else
                                {
                                    var nil = new List<int>();
                                    nil.Add(0);
                                    user.Add(nil);
                                }
                            }
                            // добавить список из списков ответов пользователя по всем вопросам в список ответов по всем пользователям
                            answers.Add(user);
                        }
                    }
                    // показать диаграмму
                    chart1.Visible = true;
                    // сделать доступной кнопку перехода к следующему вопросу
                    button2.Enabled = true;
                    // отобразить диаграмму для первого вопроса
                    GetStatistics(num);
                    // увеличить номер текущего вопроса на 1 
                    num++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка! Не удалось прочитать файл. Описание ошибки: " + ex.Message);
                }
            }
        }

        // событие, возникающее при нажатии на кнопку "Далее"
        private void button2_Click(object sender, EventArgs e)
        {
            if (num == questions.Count)
            {
                MessageBox.Show("Просмотр завершен!", "Сообщение",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                button2.Enabled = false;
                return;
            }
            else
            {
                GetStatistics(num);
                num++;
            }
        }
    }
}
