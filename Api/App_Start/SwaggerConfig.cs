using System.Web.Http;
using WebActivatorEx;
using Api;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace Api
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v2", "Api v2 H2H Poste Italiane");
                    c.IncludeXmlComments(string.Format(@"{0}\bin\Api.XML", System.AppDomain.CurrentDomain.BaseDirectory));
                    c.UseFullTypeNameInSchemaIds();
                })
                .EnableSwaggerUi(c =>
                {
                    c.InjectStylesheet(thisAssembly, "Api.Content.swagger-editor.css");
                });
        }
    }
}
