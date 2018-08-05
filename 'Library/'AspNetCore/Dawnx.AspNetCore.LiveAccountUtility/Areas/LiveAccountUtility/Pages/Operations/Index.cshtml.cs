﻿using Dawnx.AspNetCore.LiveAccount;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Dawnx.AspNetCore.LiveAccountUtility.Pages.Operations
{
    public class IndexModel : PageModel
    {
        private readonly ILiveAccountManager _liveAccountManager
            = DIUtility.GetEntryService<ILiveAccountManager>(LiveAccountDependencyInjection.LiveAccountService);
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            if (!LiveAccountUtility.IsUserAllowed(User))
                throw LiveAccountUtility.New_UnauthorizedAccessException;

            if (HttpContext.Request.Path.ToString().EndsWith("Operations/"))
                return Redirect("Index");
            if (HttpContext.Request.Path.ToString().EndsWith("Operations"))
                return Redirect("Operations/Index");

            ViewData["Items"] = _liveAccountManager.LiveOperations.ToArray();
            return Page();
        }

    }
}