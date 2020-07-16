using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Parser = Microsoft.OData.UriParser;

namespace TestODataCore
{
	public class RelatedEntityRoutingConvention : IODataRoutingConvention
	{
		public IEnumerable<ControllerActionDescriptor> SelectAction(RouteContext routeContext)
		{
			var request = routeContext.HttpContext.Request;
			var odataPath = routeContext.HttpContext.ODataFeature().Path;

			var actionCollectionProvider = routeContext.HttpContext.RequestServices.GetRequiredService<IActionDescriptorCollectionProvider>();

			// Web API OData 5.9.1 started calling parameterless GET() action when querying with composite key. https://github.com/OData/WebApi/issues/884
			if (odataPath.PathTemplate == "~/entityset/key")
			{
				var keyValueSegment = odataPath.Segments[1] as Parser.KeySegment;
				routeContext.RouteData.Values[ODataRouteConstants.Key] = string.Join(",", keyValueSegment.Keys.Select(p => p.Key + "=" + p.Value));
			}

			if (odataPath.PathTemplate.StartsWith("~/entityset/key/navigation"))
			{
				var controllerName = string.Empty;

				var segment = odataPath.Segments[odataPath.Segments.Count - 1] as Parser.NavigationPropertySegment;
				if (segment != null)
					controllerName = segment.NavigationSource.Name;

				var actionName = request.Method;

				if (odataPath.PathTemplate.EndsWith("$ref"))
					actionName += "Ref";
				else
					actionName += "FromRelatedEntity";

				var actionDescriptors = actionCollectionProvider
					.ActionDescriptors.Items.OfType<ControllerActionDescriptor>()
					.Where(c => c.ControllerName == controllerName);

				var matchingActions = actionDescriptors
						.Where(c => string.Equals(c.ActionName, actionName, StringComparison.OrdinalIgnoreCase) && c.Parameters.Count == 1)
						.ToList();

				return matchingActions;
			}

			return null;
		}
	}
}