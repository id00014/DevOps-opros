using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PollsProject
{
    public partial class PassPollForm : Form
    {
        string login, pass;  // логин и пароль пользователя
        int num = 0; // номер текущего вопроса
        string filename = "";  // имя текстового файла с опросом
        List<Control> tbs = new List<Control>(), chbs = new List<Control>(), rbs = new List<Control>(); // списки контролов, описывающих вопрос на форме (список текстовых полей, чекбоксов, радиокнопок)
        Dictionary<Question, List<int>> questions = new Dictionary<Question, List<int>>();              // словарь с парами (вопрос, список ответов пользователя)

        public PassPollForm(string log, string password)
        {
            InitializeComponent();
            login = log;
            pass = password;
        }

        // метод для добавления нового текстового поля на форму
        public TextBox AddNewTextBox(Question q, int countVars)
        {
            TextBox txt = new TextBox();

            this.Controls.Add(txt);

            tbs.Add(txt);

            txt.Location = new Point(120, countVars * txt.Size.Height + countVars * 5 + 50);
            txt.BackColor = Color.White;
            txt.Width = 400;
            txt.ReadOnly = true;
            txt.Text = q.vars[countVars - 1].ToString();
            return txt;
        }

        // метод для добавления нового флажка (чекбокса) на форму
        public CheckBox AddCheckBox(int countVars)
        {
            CheckBox chb = new CheckBox();

            this.Controls.Add(chb);

            chbs.Add(chb);
            chb.Tag = countVars;

            chb.Location = new Point(100, countVars * tbs.ElementAt(countVars - 1).Size.Height + countVars * 5 + 50);
            return chb;
        }

        // метод для добавления новой радиокнопки на форму
        public RadioButton AddRadioButton(int countVars)
        {
            RadioButton rb = new RadioButton();

            this.Controls.Add(rb);

            rbs.Add(rb);
            rb.Tag = countVars;

            rb.Location = new Point(100, countVars * tbs.ElementAt(countVars - 1).Size.Height + countVars * 5 + 50);
            return rb;
        }

        // метод для вывода результатов прохождения опроса в файл
        void WriteResultsToFile()
        {
            List<string> answers = new List<string>();
            for (int i = 0; i < questions.Count; i++)
            {
                string answer = "";
                for (int j = 0; j < questions.Values.ElementAt(i).Count; j++)
                    answer += questions.Values.ElementAt(i).ElementAt(j) + " ";
                answers.Add(answer);
            }
            string path = Path.Combine(Directory.GetCurrentDirectory(), "..");
            Program.fnamepath = Path.Combine(path, Path.GetFileNameWithoutExtension(filename));
            Directory.CreateDirectory(Program.fnamepath);
            string outdir = Path.Combine(Program.fnamepath, login);
            string outpath = outdir + "_.txt";
            File.WriteAllLines(outpath, answers);
        }

        // событие, возникающее при нажатии на кнопку "Следующий вопрос"
        private void button2_Click(object sender, EventArgs e)
        {
            int count1 = this.Controls.OfType<CheckBox>().Count(x => x.Checked);
            int count2 = this.Controls.OfType<RadioButton>().Count(x => x.Checked);

            if (count1 == 0 && count2 == 0 && !questions.Keys.ElementAt(num - 1).optional)
            {
                MessageBox.Show("Вы должны выбрать хотя бы один вариант ответа!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            questions.Values.ElementAt(num - 1).Clear();

            if (questions.Keys.ElementAt(num - 1).multiply)
            {
                this.Controls.OfType<CheckBox>()
                    .Where(c => c.Checked)
                    .ToList()
                    .ForEach(c => questions.Values.ElementAt(num - 1).Add((int)c.Tag));
            }
            else
            {
                this.Controls.OfType<RadioButton>()
                    .Where(c => c.Checked)
                    .ToList()
                    .ForEach(c => questions.Values.ElementAt(num - 1).Add((int)c.Tag));
            }

            if (num == questions.Count)
            {
                MessageBox.Show("Опрос завершен!", "Сообщение",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                WriteResultsToFile();
                this.Hide();
                return;
            }

            this.Controls.OfType<TextBox>()
                .Where(c => tbs.Contains(c))
                .ToList()
                .ForEach(c => this.Controls.Remove(c));

            this.Controls.OfType<CheckBox>()
                .Where(c => chbs.Contains(c))
                .ToList()
                .ForEach(c => this.Controls.Remove(c));

            this.Controls.OfType<RadioButton>()
                .Where(c => rbs.Contains(c))
                .ToList()
                .ForEach(c => this.Controls.Remove(c));

            AddQuestion(questions.Keys.ElementAt(num));
            num++;
        }

        // событие, возникающее при нажатии на кнопку "Назад"
        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        // событие, возникающее при нажатии на кнопку "Предыдущий вопрос"
        private void button3_Click(object sender, EventArgs e)
        {
            if (num < 2)
            {
                MessageBox.Show("Вы на первом вопросе!", "Сообщение",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            num--;

            this.Controls.OfType<TextBox>()
                .Where(c => tbs.Contains(c))
                .ToList()
                .ForEach(c => this.Controls.Remove(c));

            this.Controls.OfType<CheckBox>()
                .Where(c => chbs.Contains(c))
                .ToList()
                .ForEach(c => this.Controls.Remove(c));

            this.Controls.OfType<RadioButton>()
                .Where(c => rbs.Contains(c))
                .ToList()
                .ForEach(c => this.Controls.Remove(c));

            AddQuestion(questions.Keys.ElementAt(num - 1));
        }

        // метод для добавления вопроса на форму
        public void AddQuestion(Question q)
        {
            textBox1.Text = q.text;
            for (int i = 0; i < q.vars.Count; i++)
            {
                AddNewTextBox(q, i + 1);
                if (q.multiply)
                    AddCheckBox(i + 1);
                else
                    AddRadioButton(i + 1);
            }
        }

        // событие, возникающее при нажатии на кнопку "Загрузить опрос"
        private void button1_Click(object sender, System.EventArgs e)
        {
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Выберите текстовый файл";
            theDialog.Filter = "TXT files|*.txt";
            theDialog.InitialDirectory = Path.Combine(Directory.GetCurrentDirectory(), "..");
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    filename = theDialog.FileName;
                    string[] filelines = File.ReadAllLines(filename);
                    foreach (string str in filelines)
                    {
                        string[] fields = str.Split(';').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                        List<string> vars = new List<string>();
                        for (int i = 1; i < fields.Length - 2; i++)
                            vars.Add(fields[i]);
                        Question q = new Question(fields[0], vars,
                            bool.Parse(fields[fields.Length - 2]),
                            bool.Parse(fields[fields.Length - 1]));
                        questions.Add(q, new List<int>());
                    }
                    MessageBox.Show("Опрос загружен!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    button1.Enabled = false;
                    button2.Enabled = true;
                    button3.Enabled = true;
                    AddQuestion(questions.Keys.ElementAt(num));
                    num++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка! Не удалось прочитать файл. Описание ошибки: " + ex.Message);
                }
            }
        }
    }
}
