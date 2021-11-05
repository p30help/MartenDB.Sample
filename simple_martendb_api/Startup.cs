using System.Reflection;
using Marten;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using simple_martendb_api.Models;
using Weasel.Postgresql;

namespace simple_martendb_api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region Settings Marten

            var cn = Configuration.GetConnectionString("Postgres");

            var options = new StoreOptions()
            {
                AutoCreateSchemaObjects = AutoCreate.All,
                DatabaseSchemaName = "test",
            };

            //var serializer = new JsonNetSerializer();
            //serializer.Customize(c => c.ContractResolver = new ResolvePrivateSetters());
            //options.Serializer(serializer);
            //options.Events.UseAggregatorLookup(AggregationLookupStrategy.UsePrivateApply);
            //options.Events.InlineProjections.AggregateStreamsWith<AggregateWithPrivateEventApply>();

            // this is optional - if does not use this line Id is default key
            // read more https://martendb.io/documents/identity.html#overriding-the-choice-of-id-property-field
            options.Schema.For<Person>().Identity(x => x.Id);

            options.Connection(cn);

            services.AddMarten(options);

            #endregion


            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "postgres_marten_docker_api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "postgres_marten_docker_api v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    internal class ResolvePrivateSetters : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(
            MemberInfo member,
            MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);

            if (!prop.Writable)
            {
                var property = member as PropertyInfo;
                if (property != null)
                {
                    var hasPrivateSetter = property.GetSetMethod(true) != null;
                    prop.Writable = hasPrivateSetter;
                }
            }

            return prop;
        }
    }

}
