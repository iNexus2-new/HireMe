using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace HireMe.Core.Helpers
{
    [HtmlTargetElement(Attributes = "asp-checklistboxasync, asp-modelname, asp-for")]
    public class CheckListBoxTagHelperAsync : TagHelper
    {
        [HtmlAttributeName("asp-checklistboxasync")]
        public IAsyncEnumerable<SelectListItem> Items { get; set; }

        [HtmlAttributeName("asp-modelname")]
        public string ModelName { get; set; }

        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var i = 0;
            await foreach (var item in Items)
            {
                var selected = item.Selected ? @"checked=""checked""" : "";
                var disabled = item.Disabled ? @"disabled=""disabled""" : "";

                var html = $@"<label><input type=""checkbox"" {selected} {disabled} id=""{ModelName}_{i}__Selected"" name=""{ModelName}[{i}].Selected"" value=""true"" /> {item.Text}</label>";
                html += $@"<input type=""hidden"" id=""{ModelName}_{i}__Value"" name=""{ModelName}[{i}].Value"" data-val=""true"" value=""{item.Value}"">";
                html += $@"<input type=""hidden"" id=""{ModelName}_{i}__Text"" name=""{ModelName}[{i}].Text"" data-val=""true"" value=""{item.Text}"">";

                output.Content.AppendHtml(html);

                i++;
            }

            output.Attributes.SetAttribute("class", "th-chklstbx");
        }

    }
}