﻿using Dawnx;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Sapling
{
    public abstract class SpUploadBoxController : Controller
    {
        public virtual JsonResult Config()
        {
            return Json(JSend.Success.Create(new SpUploadBox.Config
            {
                StatUrl = Url.Action(nameof(Stat)),
                PreviewUrl = Url.Action(nameof(Preview)),
                SubmitUrl = Url.Action(nameof(Submit)),
            }));
        }

        public abstract JsonResult Stat(string tag);
        public abstract IActionResult Preview(string tag);
        public abstract JsonResult Submit(string tag);
    }
}
