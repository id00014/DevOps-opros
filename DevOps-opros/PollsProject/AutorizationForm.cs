using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PollsProject
{
    public partial class AutorizationForm : Form
    {
        public AutorizationForm()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0; // по умолчанию выбирается режим Администратора
        }

        // захардкоженный пароль админа
        const string ADMINPASS = "ADMIN";

        // объекты классов форм для создания, прохождения опроса и просмотра результатов:
        CreatePollForm cpf;
        PassPollForm ppf;
        ResultsForm rfm;

        // событие, возникающее при изменении типа доступа
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // если выбран Пользователь, сделать видимыми метку и текстовое поле для ввода логина 
            if (comboBox1.SelectedIndex == 1)
            {
                label2.Visible = true;
                textBox1.Visible = true;
            }
            // иначе сделать видимыми метку и текстовое поле для ввода пароля
            else
            {
                label2.Visible = false;
                textBox1.Visible = false;
            }
        }

        // событие, возникающее при нажатии на кнопку "Вход"
        private void button1_Click(object sender, EventArgs e)
        {
            string login, pass; // логин и пароль 
            // если выбран Пользователь, то
            if (comboBox1.SelectedIndex == 1)
            {
                // убедиться, что заданы логин и пароль
                if (string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    MessageBox.Show("Логин не может быть пустым!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (string.IsNullOrWhiteSpace(textBox2.Text))
                {
                    MessageBox.Show("Пароль не может быть пустым!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // считать логин и пароль
                login = textBox1.Text;
                pass = textBox2.Text;
                // открыть форму прохождения опроса, передав ей логин и пароль пользователя
                ppf = new PassPollForm(login, pass);
                ppf.Owner = this;
                ppf.ShowDialog();
            }
            // иначе (выбран Администратор)
            else
            {
                // убедиться, что задан пароль
                if (string.IsNullOrWhiteSpace(textBox2.Text))
                {
                    MessageBox.Show("Пароль не может быть пустым!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // убедиться, что заданный пароль совпадает с паролем Администратора
                pass = textBox2.Text.ToUpper();
                if (!pass.Equals(ADMINPASS))
                {
                    MessageBox.Show("Введен неверный пароль!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // открыть форму создания опроса
                cpf = new CreatePollForm();
                cpf.Owner = this;
                cpf.ShowDialog();
            }
        }

        // событие, возникающее при нажатии на кнопку "Результаты"
        private void button2_Click(object sender, EventArgs e)
        {
            // если выбран Администратор, то
            if (comboBox1.SelectedIndex == 0)
            {
                // убедиться, что заданный пароль совпадает с паролем Администратора
                string pass = textBox2.Text.ToUpper();
                if (!pass.Equals(ADMINPASS))
                {
                    MessageBox.Show("Введен неверный пароль!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // открыть форму просмотра результатов
                rfm = new ResultsForm();
                rfm.Owner = this;
                rfm.ShowDialog();
            }
            // иначе (выбран Пользователь) сообщить, что просмотр результатов невозможен
            else
            {
                MessageBox.Show("Доступ запрещен!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
    }
}
