using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ASPControllerPC.Models
{
    internal class Rate
    {
        [Key]
        public int Cur_ID { get; set; }
        public DateTime Date { get; set; }
        //public string Cur_Abbreviation { get; set; }
        //public int Cur_Scale { get; set; }
        //public string Cur_Name { get; set; }
        public double Cur_OfficialRate { get; set; }


        public Rate[] Mass(string str)
        {
            WebClient client = new WebClient();
            var temp = client.DownloadString(str);
            var json = JsonConvert.DeserializeObject<Rate[]>(temp);
            return json;
        }
        public string Format(string str)
        {
            WebClient client = new WebClient();
            var temp = client.DownloadString(str);
            return temp;
        }
    }

}
