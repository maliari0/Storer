using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Encodings.Web;

[HtmlTargetElement("custom-colored-heading3")]
public class CustomColoredHeading3TagHelper : TagHelper
{
	public string Text { get; set; }
	public string Color { get; set; }
	public string CssClass { get; set; }

	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		output.TagName = "h3";

		if (!string.IsNullOrEmpty(Color))
		{
			output.Attributes.Add("style", $"color: {Color}");
		}

		if (!string.IsNullOrEmpty(CssClass))
		{
			output.AddClass(CssClass, HtmlEncoder.Default);
		}

		output.Content.SetContent(Text);
	}
}
