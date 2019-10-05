using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;


namespace WindowsFormsApp3
{
    public partial class Form2 : Form


    {
        
        Label l1, l2;
        TextBox t1;
        Button bt1;
        ComboBox box;
     
       
        public Form2()
        {

            Form1 P = (Form1)this.Parent;

            this.Size = new Size(300, 150);
            this.Name = "Поиск";

            InitializeComponent();


            #region UI elements
            l1 = new Label()
            {
                Text = "Введите слово: ",
                Location = new Point(10, 10),
                AutoSize = true,
                Size = new Size(150, 30)

            };
            Controls.Add(l1);

            t1 = new TextBox()
            {
                Left = l1.Left,
                Top = l1.Bottom + 10,
                Size = l1.Size
            };
            Controls.Add(t1);

      

            l2 = new Label()
            {
                Text = "Выберите параметр:",
                Top = l1.Top,
                Left = l1.Right +20 ,
                AutoSize = true,
                Size = new Size(150, 30)

            };
            Controls.Add(l2);

          box  = new ComboBox()
            {
                Left = l2.Left,
                Top = l2.Bottom + 10,
                Size = new Size(110, 25)
            };
            
            this.Controls.Add(box);

           box.Items.Add("Название");
            box.Items.Add("Максимальная t");
            box.Items.Add("Минимальная t");
            box.Items.Add("Тип");
            box.Items.Add("Расстояние");
            box.SelectedIndex = 0;
            bt1 = new Button()
            {
                Text = "Поиск",
                Left = 70,
                Top = box.Bottom + 20,
                Size = new Size(60, 25)
            };
            this.Controls.Add(bt1);
            bt1.Click += btnSearch_Click;



            #endregion





            void btnSearch_Click(object sender, EventArgs e)
            {
                int choice = box.SelectedIndex;
               
                string s = t1.Text;
                if(!(this.Owner as Form1).search(s, choice))  MessageBox.Show("Ничего не найдено"); 
               else Close();
               
            }

            

        }

    }
}