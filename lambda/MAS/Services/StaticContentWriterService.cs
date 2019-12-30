﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace MAS.Services
{
    public interface IStaticContentWriter
    {
        Task<string> RenderViewAsync<TModel>(Controller controller, string viewName, TModel model, bool isPartial = false);
    }

    public class StaticContentWriterService : IStaticContentWriter
    {
        private readonly ILogger<StaticContentWriterService> _logger;

        public StaticContentWriterService(ILogger<StaticContentWriterService> logger)
        {
            _logger = logger;
        }

        public async Task<string> RenderViewAsync<TModel>(Controller controller, string viewName, TModel model, bool isPartial = false)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = controller.ControllerContext.ActionDescriptor.ActionName;

            controller.ViewData.Model = model;

            using (var writer = new StringWriter())
            {
                IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                ViewEngineResult viewResult = GetViewEngineResult(controller, viewName, isPartial, viewEngine);

                if (viewResult.Success == false)
                    _logger.LogError($"A view with the name {viewName} could not be found");

                ViewContext viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);

                return writer.GetStringBuilder().ToString();
            }
        }

        private static ViewEngineResult GetViewEngineResult(Controller controller, string viewName, bool isPartial, IViewEngine viewEngine)
        {
            if (viewName.StartsWith("~/"))
            {
                var hostingEnv = controller.HttpContext.RequestServices.GetService(typeof(IHostingEnvironment)) as IHostingEnvironment;
                return viewEngine.GetView(hostingEnv.WebRootPath, viewName, !isPartial);
            }
            else
            {
                return viewEngine.FindView(controller.ControllerContext, viewName, !isPartial);

            }
        }
    }
}
