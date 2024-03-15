using Api.Dtos;
using Api.Models;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api.Controllers.Api
{
    [RoutePrefix("api/VisureDocumentType")]
    public class VisureDocumentTypeController : ApiController
    {
        private static Entities _context;

        public VisureDocumentTypeController()
        {
            _context = new Entities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        [Route("")]
        [SwaggerResponse(200, "List", typeof(IEnumerable<CompleteDocsVisure>))]
        public IEnumerable<CompleteDocsVisure> GetList(int documentType)
        {
            //MULTIPLE USERS
            var v = _context.VisureDocumentType
                .Where(a => a.enabled == true)
                .Where(a => a.documentType == documentType);

            if (v.Count() == 0)
                return null;

            var l = new List<CompleteDocsVisure>();
            foreach(var vis in v)
            {
                var c = new CompleteDocsVisure()
                {
                    attribute = vis.attribute,
                    description = vis.description,
                    value = vis.value,
                    documentType = vis.documentType,
                    documentTypeName = (vis.documentType == 0 ? "Certificato" : "Visura"),
                    chiusure = GetCodiciChiusure(vis.attribute),
                    chiusuraDefault = (vis.documentType == 0 ? "F" : null)
                };
                l.Add(c);
            }

            return l;
        }


        private string[] GetCodiciChiusure(string attribute)
        {
            var c = new List<string>();
            switch (attribute)
            {
                case "CART":
                    c.Add("A");
                    c.Add("B");
                    c.Add("C");
                    c.Add("L");
                    c.Add("F");
                    break;
                case "CRIA":
                case "CRIM":
                case "CRIS":
                    c.Add("A");
                    c.Add("B");
                    c.Add("C");
                    c.Add("D");
                    c.Add("E");
                    c.Add("F");
                    c.Add("G");
                    c.Add("H");
                    c.Add("I");
                    c.Add("L");
                    break;
            }

            return c.ToArray();
        }
    }
}
