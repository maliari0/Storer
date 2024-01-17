using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using TagBuilder = Microsoft.AspNetCore.Mvc.Rendering.TagBuilder;

public static class CustomHtmlHelpers
{
    public static IHtmlContent ColoredParagraph(this IHtmlHelper helper, string text, string color)
    {
        var tagBuilder = new TagBuilder("p");
        tagBuilder.InnerHtml.AppendHtml(text);
        tagBuilder.MergeAttribute("style", $"color:{color}");

        return tagBuilder;
    }

	public static IHtmlContent ColoredHeading3(this IHtmlHelper helper, string text, string color, string additionalClasses = "")
	{
		var tagBuilder = new TagBuilder("h3");
		tagBuilder.InnerHtml.AppendHtml(text);
		tagBuilder.MergeAttribute("style", $"color:{color}");

		if (!string.IsNullOrEmpty(additionalClasses))
		{
			tagBuilder.AddCssClass(additionalClasses);
		}

		return tagBuilder;
	}

	public static IHtmlContent ColoredHeading2(this IHtmlHelper helper, string text, string color, string additionalClasses = "")
	{
		var tagBuilder = new TagBuilder("h2");
		tagBuilder.InnerHtml.AppendHtml(text);
		tagBuilder.MergeAttribute("style", $"color:{color}");

		if (!string.IsNullOrEmpty(additionalClasses))
		{
			tagBuilder.AddCssClass(additionalClasses);
		}

		return tagBuilder;
	}
}
