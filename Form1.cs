using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Linq;
using ZedGraph;
using System.Windows.Forms.DataVisualization.Charting;
using WindowsFormsApp3.Properties;


namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {

        List<SObject> objects = new List<SObject>();

        ToolStripButton saveb, loadb, searchb;
        FileDialog files;
        public DataGridView grid;
        BindingSource bs;
        BindingNavigator nav;
        PictureBox pic;
        PropertyGrid prop;
        ZedGraphControl zg1;
        GraphPane myPane;
        BarItem myBar;

        public Form1()
        {


            
            
            InitializeComponent();
            this.Text = "Solar system";
            this.Size = new Size(800, 500);

            bs = new BindingSource(); // Инициализируем пустым списком 
            bs.DataSource = objects; // Добавляем новый объект непосредственно к bs
            bs.Add(new SObject("Солнце", 10, 10, stypes.star, 0, "sun.jpg"));
            bs.Add(new SObject("Меркурий", 480, -180, stypes.planet, 0.387f));
            bs.Add(new SObject("Венера", 480, null, stypes.planet, 0.723f, "venus.jpg"));
            bs.Add(new SObject("Земля", 58, -90, stypes.planet, 1f, "earth.jpg"));
            bs.Add(new SObject("Луна", 0, null, stypes.other, null, "moon.jpg"));
            bs.Add(new SObject("Марс", 0, -150, stypes.planet, 1.5f, "mars.jpg"));
            bs.Add(new SObject("Юпитер", -160, -160, stypes.planet, 5.2f, "jupiter.jpg"));
            bs.Add(new SObject("Сатурн", -150, -150, stypes.planet, 9.5f, "saturn.jpg"));
            bs.Add(new SObject("Уран", -220, -220, stypes.planet, 19.2f, "uran.jpg"));
            bs.Add(new SObject("Нептун", -213, -213, stypes.planet, 30f));
            bs.AllowNew = true;

            #region Bindingnav
            // Привязываем панель BindingNavigator  
            nav = new BindingNavigator(bs);
            Controls.Add(nav);

            saveb = new ToolStripButton();
            saveb.AccessibleName = "Save";
            saveb.DisplayStyle = ToolStripItemDisplayStyle.Image;
            saveb.Image = Resources.save1;
            saveb.Name = "Save";
            saveb.Size = new System.Drawing.Size(28, 28);
            saveb.Text = "Save to file";
            saveb.Click += new System.EventHandler(SaveButton_Click);
            nav.Items.Add(saveb);

            loadb = new ToolStripButton();
            loadb.AccessibleName = "Load";
            loadb.DisplayStyle = ToolStripItemDisplayStyle.Image;
            loadb.Image = Resources.load;
            loadb.Name = "Load";
            loadb.Size = new System.Drawing.Size(28, 28);
            loadb.Text = "Load from file";
            loadb.Click += new System.EventHandler(LoadButton_Click);
            nav.Items.Add(loadb);

            searchb = new ToolStripButton();
            searchb.AccessibleName = "Search";
            searchb.DisplayStyle = ToolStripItemDisplayStyle.Image;
            searchb.Image = Resources.search;
            searchb.Name = "Search";
            searchb.Size = new System.Drawing.Size(28, 28);
            searchb.Text = "Load from file";
            searchb.Click += new System.EventHandler(SearchButton_Click);
            nav.Items.Add(searchb);
            #endregion 

            pic = new PictureBox()
            {
                Size = new Size(250, 200),
                Location = new Point(510, 30),
            };

            pic.DataBindings.Add("Image", bs, "Pic", true);
            pic.SizeMode = PictureBoxSizeMode.StretchImage;
            Controls.Add(pic);

            prop = new PropertyGrid()
            {
                Size = new Size(250, 200),
                Location = new Point(510, 240),
            };
            prop.DataBindings.Add("SelectedObject", bs, "");
            Controls.Add(prop);

            #region Grid

            grid = new DataGridView()
            {
                Size = new Size(500, 200),
                Location = new Point(0, 30),

            };
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.DataSource = bs;
            grid.ColumnHeadersHeight = 40;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            grid.RowValidating += grid_RowValidating;
            grid.DataError += Grid_DataError;
            grid.AutoGenerateColumns = false;
            var column1 = new DataGridViewTextBoxColumn();
            column1.Name = "Name";
            column1.HeaderText = "Название объекта";
            column1.DataPropertyName = "Name";
            column1.Width = 70;

            grid.Columns.Add(column1);

            var column2 = new DataGridViewTextBoxColumn();
            column2.Name = "TMax";
            column2.HeaderText = "Максимальная температура";
            column2.DataPropertyName = "TMax";
            column2.Width = 90;
            grid.Columns.Add(column2);

            var column3 = new DataGridViewTextBoxColumn();
            column3.Name = "TMin";
            column3.HeaderText = "Минимальная температура";
            column3.DataPropertyName = "TMin";
            column3.Width = 90;
            grid.Columns.Add(column3);

            var column4 = new DataGridViewTextBoxColumn();
            column4.Name = "Distance";
            column4.HeaderText = "Расстояние до солнца";
            column4.DataPropertyName = "Distance";
            column4.Width = 80;
            grid.Columns.Add(column4);

            var column5 = new DataGridViewComboBoxColumn();
            column5.Name = "SType";
            column5.HeaderText = "Тип объекта";
            column5.DataPropertyName = "SType";
            column5.DataSource = Enum.GetValues(typeof(stypes));
            grid.Columns.Add(column5);


            Controls.Add(grid);

            #endregion

          
            // Настраиваем привязку к источнику данных (BindingSource)  
            zg1 = new ZedGraphControl();
            zg1.Location = new Point(1, 240);
            zg1.Size = new Size(500, 200);
            
            CreateGraph(zg1);
           
            // В случае изменений обновляем гистограмму  
            bs.CurrentChanged += (o, e) => createSeries(zg1);
            Controls.Add(zg1);
            

            void grid_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
            {  // Считываем значение в обязательной ячейке  
                var v = grid[0, e.RowIndex].Value;
                if (v == null)
                { e.Cancel = true;   // Переводим фокус в обязательную ячейку  
                    grid.CurrentCell = grid[0, e.RowIndex];   // Режим редактирования с выделением 
                    grid.BeginEdit(true); }
                


                var v2 = grid[1, e.RowIndex].Value;
                if (v2 == null)
                {
                    e.Cancel = true;   // Переводим фокус в обязательную ячейку  
                    grid.CurrentCell = grid[1, e.RowIndex];   // Режим редактирования с выделением 
                    grid.BeginEdit(true);
                }
            }


            bool xmlDeserealize(String file) {
                Stream stream = null;
                try
                {
                    stream = new FileStream(file, FileMode.Open);

                    XmlSerializer xmlSer = new XmlSerializer(typeof(List<SObject>));
                    List<SObject> tmp = (List<SObject>)xmlSer.Deserialize(stream);
                    foreach (SObject so in tmp)
                    {
                        //so.Pic;
                        bs.Add(so);
                    }
                    return true;
                }
                catch (Exception err)
                {
                    return false;
                }
                finally
                {
                    stream.Close();
                }
            }
            bool xmlSerealize(String file) {
                Stream stream = null;
                XmlSerializer xmlSer = null;
                try
                {
                    stream = new FileStream(file, FileMode.Create);
                    xmlSer = new XmlSerializer(typeof(List<SObject>));

                    xmlSer.Serialize(stream, bs.DataSource);
                    return true;
                }
                catch (Exception err)
                {
                    return false;
                }
                finally
                {
                    stream.Close();

                }
            }

             void SaveButton_Click(System.Object sender,
          System.EventArgs e)

            {

                files = new SaveFileDialog();

                files.FileName = "Solar System.xml";
                files.Filter = "XML File | *.xml";
                if (files.ShowDialog() == DialogResult.OK)
                {
                    xmlSerealize(files.FileName);
                }

                //  TableAdapterName.Update(DataSetName.TableName);
            }

            void LoadButton_Click(System.Object sender,
         System.EventArgs e)

            {

                files = new OpenFileDialog();

                //files.FileName = "Solar System.xml";
                files.Filter = "XML File | *.xml";
                if (files.ShowDialog() == DialogResult.OK)
                {
                    xmlDeserealize(files.FileName);
                }

                //  TableAdapterName.Update(DataSetName.TableName);
            }

            void SearchButton_Click(System.Object sender,
        System.EventArgs e)
            {
                Form2 form = new Form2();
                form.Show(this);
            }
            
             
        }

      

        private void Grid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Введите число");
        }

        public bool search(String s, int choice)
        {
            s = s.ToLower();
            bool flag = false;
            try
            {

                foreach (DataGridViewRow row in grid.Rows)
                {
                    row.Selected = false;
                    Object s2 = row.Cells[choice].Value;
                    if (s2 != null)
                    {
                        string s3 = s2.ToString().ToLower();
                        if (s3.Equals(s))
                        {
                            row.Selected = true;

                            flag = true;

                        }
                        else row.Selected = false;

                    }
                }


            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }

            return flag;
        }

        private void CreateGraph(ZedGraphControl zg1)
        {
            // get a reference to the GraphPane
            myPane = zg1.GraphPane;
            // Set the Titles
            myPane.Title = "Максимальная температура";
            myPane.XAxis.Title = "Название";
            myPane.YAxis.Title = "Температура в градусах цельсия";

            // Make up some random data points
            string[] labels = new string[bs.Count];
          double[] temperature = new double[bs.Count];
            for (int i=0; i<bs.Count; i++)
            {
                labels[i] = objects[i].Name;
                temperature[i] = objects[i].Tmax;
            }

            // Generate a red bar with "Curve 1" in the legend
            BarItem myBar = myPane.AddBar("Температура", null,temperature,
                                                        Color.Red);
            myBar.Bar.Fill = new Fill(Color.Red, Color.White, Color.Red);


            // Set the XAxis labels
            myPane.XAxis.TextLabels = labels;
            // Set the XAxis to Text type
           myPane.XAxis.Type = ZedGraph.AxisType.Text;

          
            // Tell ZedGraph to refigure the
            // axes since the data have changed
            zg1.AxisChange();
        }

        private void createSeries(ZedGraphControl z)
        {
            myPane.CurveList.Remove(0);
            string[] labels = new string[bs.Count];
            double[] temperature = new double[bs.Count];
            for (int i = 0; i < bs.Count; i++)
            {
                labels[i] = objects[i].Name;
                temperature[i] = objects[i].Tmax;
            }


                myBar = myPane.AddBar("Температура", null, temperature,
                                                        Color.Red);

                myPane.XAxis.TextLabels = labels;
               zg1.AxisChange();
                z.Invalidate();
                z.Refresh();
            
        }

    }
}

