using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace WebApi_1
{
	public class SwaggerFile : IOperationFilter
	{
		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			var fileParams = operation.Parameters
				.Where(p => p.In == ParameterLocation.Query && p.Schema.Type == "file")
				.ToList();

			foreach (var param in fileParams)
			{
				param.In = ParameterLocation.Query;
				param.Schema.Type = "string";
				param.Schema.Format = "binary";
			}
		}
	}
}
