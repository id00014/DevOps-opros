using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PollsProject
{
    public partial class CreatePollForm : Form
    {
        int countVars = 1;                                  // номер варианта ответа
        List<Control> vars = new List<Control>();           // список элементов управления, отвечающих за варианты ответов
        List<Question> questions = new List<Question>();    // список вопросов
            
        public CreatePollForm()
        {
            InitializeComponent();
        }

        // метод для добавления нового текстового поля на форму (вариант ответа)
        public TextBox AddNewTextBox()
        {
            TextBox txt = new TextBox();

            this.Controls.Add(txt);

            vars.Add(txt);

            txt.Location = new Point(120, countVars * txt.Size.Height + countVars * 5 + 50);
            txt.Width = 400;
            txt.Text = "Вариант " + countVars.ToString();

            return txt;
        }

        // событие, возникающее при нажатии на кнопку "Добавить вопрос"
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Введите текст вопроса!", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (vars.Count < 2)
            {
                MessageBox.Show("Добавьте хотя бы пару вариантов ответа!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Question q = new Question(textBox1.Text, vars.Select(x => x.Text).ToList(),
                checkBox1.Checked, checkBox2.Checked);
            questions.Add(q);

            countVars = 1;
            this.Controls
              .OfType<TextBox>()
              .Where(c => vars.Contains(c))
              .ToList()
              .ForEach(c => this.Controls.Remove(c));
            vars.Clear();
            textBox1.Clear();
        }

        // событие, возникающее при нажатии на кнопку "Добавить вариант ответа"
        private void button1_Click(object sender, EventArgs e)
        {
            if (countVars == 7)
            {
                MessageBox.Show("Достигнуто максимальное количество вариантов ответа!", "Сообщение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            AddNewTextBox();
            countVars = countVars + 1;
        }

        // метод для возврата к форме авторизации
        void BackToParent()
        {
            this.Hide();
            AutorizationForm auf = new AutorizationForm();
            auf.Show();
        }

        // событие, возникающее при нажатии на кнопку "Создать опрос"
        private void button3_Click(object sender, EventArgs e)
        {
            if (questions.Count < 3)
            {
                MessageBox.Show("Создайте хотя бы три вопроса!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
                sfd.FilterIndex = 2;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string text = "";
                    foreach (var q in questions)
                        text += q.ToString() + Environment.NewLine;
                    File.WriteAllText(sfd.FileName, text);
                    MessageBox.Show(string.Format("Опрос успешно создан и записан в файл {0}!", sfd.FileName),
                        "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    BackToParent();
                }
            }
        }

        // событие, возникающее при нажатии на кнопку "Назад"
        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
