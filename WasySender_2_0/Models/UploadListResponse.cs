using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class UploadListResponse
    {
        public bool success { get; set; }
        public string errorMessage { get; set; }
        public int listId { get; set; }
        public int senderArId { get; set; }
        public int numberOfNames { get; set; }
        public int errors { get; set; }
        public List<NamesLists> NamesLists { get; set; }
        public List<Bulletins> Bulletins { get; set; }

        public UploadListResponse()
        {
            success = false;
            errorMessage = EnumHelper<ErrorMessageUploadCsv>.GetDisplayValue(ErrorMessageUploadCsv.NessunErrore);
            listId = 0;
            numberOfNames = 0;
            senderArId = 0;
            NamesLists = null;
            Bulletins = null;
        }
    }
}