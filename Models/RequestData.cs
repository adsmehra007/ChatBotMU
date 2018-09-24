using ChatBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper.Models
{
   public class RequestData
    {
        public string matchParameter { get; set; }
        public int sequence { get; set; }
        public string  requestToken { get; set; }
        public List<Patient> patList { get; set; }
    }
}
