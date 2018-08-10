﻿using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Dawnx.AspNetCore
{
    public static class EnumUtility
    {
        public static SelectList GetSelectList<TEnum>()
            where TEnum : struct
        {
            var fields = typeof(TEnum).GetFields().Where(x => !x.IsSpecialName);
            return new SelectList(fields.Select(field => new
            {
                Value = field.Name,
                Text = field.For(_ =>
                {
                    var displayNameAttr = _.GetCustomAttribute<DisplayNameAttribute>();
                    if (displayNameAttr != null) return displayNameAttr.DisplayName;

                    var displayAttr = _.GetCustomAttribute<DisplayAttribute>();
                    if (displayAttr != null) return displayAttr.Name;

                    return field.Name;
                }),
            }), "Value", "Text");
        }

        public static SelectList GetSelectList(Enum @enum)
        {
            var fields = @enum.GetType().GetFields().Where(x => !x.IsSpecialName);
            return new SelectList(fields.Select(field => new
            {
                Value = field.Name,
                Text = field.For(_ =>
                {
                    var displayNameAttr = _.GetCustomAttribute<DisplayNameAttribute>();
                    if (displayNameAttr != null) return displayNameAttr.DisplayName;

                    var displayAttr = _.GetCustomAttribute<DisplayAttribute>();
                    if (displayAttr != null) return displayAttr.Name;

                    return field.Name;
                }),
            }), "Value", "Text", @enum.ToString());
        }

        public static HtmlString GetOptionTags(Enum @enum)
        {
            return new HtmlString(GetSelectList(@enum).Select(
                x => $@"<option value=""{x.Value}"" {(@enum.ToString() == x.Value ? "selected" : "")}>{x.Text}</option>")
                .Join(Environment.NewLine));
        }

    }
}