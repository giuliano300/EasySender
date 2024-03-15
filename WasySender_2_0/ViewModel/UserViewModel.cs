using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WasySender_2_0.Models;

namespace WasySender_2_0.ViewModel
{
    public class UserViewModel
    {
        public Users user { get; set; }
        public List<Sender> senders { get; set; }
        public string[] selectedSender { get; set; }

        public UserViewModel()
        {
            user = new Users();
            senders = new List<Sender>();
            selectedSender = null;
        }
    }
}