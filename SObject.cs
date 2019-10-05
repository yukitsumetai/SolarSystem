using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;

namespace WindowsFormsApp3
{
    public enum stypes { planet, star, other };

    [Serializable]
    public class SObject
    {
        private String name ;
        private int? tmin;
        private stypes? stype;
        private int tmax;
        private float? distance;
      private Image pic;
        private string picPath;
        


        [DisplayName("Название"), Category("Общее")]
        public string Name { get => name; set => name = value; }
        
        [DisplayName("Минимальная температура"), Category("Информация")]
        [Description("В градусах цельсия")]
        public int? Tmin { get => tmin; set => tmin = value; }
        [DisplayName("Тип объекта"), Category("Общее")]
        public stypes? Stype { get => stype; set => stype = value; }
        [DisplayName("Расстояние до солнца"), Category("Информация")]
        [Description("В астраномических единицах")]
        public float? Distance { get => distance; set => distance = value; }
        
       
        [DisplayName("Максимальная температура"), Category("Информация")]
        [Description("В градусах цельсия")]
        public int Tmax { get => tmax; set => tmax = value; }


        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("picPath")]
        public String PicPath
        {
            get
            { // serialize
                if (picPath == null) return null;
                else return picPath;

            }
            
            set
            { // deserialize
                if (value == null)
                {
                    picPath = "black.jpg";
                }
                else picPath = value;
               
            }
        }
        [DisplayName("Изображение"), Category("Картинка")]
        [XmlIgnore]
        public Image Pic { get => pic; set =>  pic = Image.FromFile(picPath); }



        public SObject()
        {
            //this.Name = "";
            //this.Tmax = 0;
            this.Tmin = null;
            this.Stype = stypes.other;
            this.Distance = null;
            this.picPath = "black.jpg";
            this.Pic = Image.FromFile(picPath);
            



        }
       



        public SObject(String p, int t1, int? t2, stypes? t, float? d, String s="black.jpg")
        {
            this.Name = p;
            this.Tmax = t1;
            this.Tmin = t2;
            this.Stype = t;
            this.Distance = d;
            this.picPath = s;
            this.Pic = Image.FromFile(s);//exception
            
        }
    }
}
