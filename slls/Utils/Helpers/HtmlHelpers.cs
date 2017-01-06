﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Policy;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.WebPages;

namespace slls.Utils.Helpers
{
    public static class HtmlHelpers
    {
        public static string CurrentViewName(this HtmlHelper html)
        {
            return System.IO.Path.GetFileNameWithoutExtension(
                ((RazorView)html.ViewContext.View).ViewPath
            );
        }

        public static MvcHtmlString ResolveUrl(this HtmlHelper htmlHelper, string url)
        {
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            return MvcHtmlString.Create(urlHelper.Content(url));
        }

        public static HtmlString AlphabeticalPager(this HtmlHelper html, string selectedLetter, IEnumerable<string> firstLetters, Func<string, string> pageLink)
        {
            var sb = new StringBuilder();
            var numbers = Enumerable.Range(0, 10).Select(i => i.ToString());
            var alphabet = Enumerable.Range(65, 26).Select(i => ((char)i).ToString()).ToList();
            alphabet.Insert(0, "All");
            alphabet.Insert(alphabet.Count, "0-9");
            alphabet.Insert(alphabet.Count, "non alpha");

            var ul = new TagBuilder("ul");
            ul.AddCssClass("pagination");
            ul.AddCssClass("alpha");

            foreach (var letter in alphabet)
            {
                var li = new TagBuilder("li");
                if (firstLetters.Contains(letter) || (firstLetters.Intersect(numbers).Any() && letter == "0-9") || letter == "non alpha" || letter == "All")
                {
                    if (selectedLetter == letter || selectedLetter.IsEmpty() && letter == "All")
                    {
                        li.AddCssClass("active");
                        var span = new TagBuilder("span");
                        span.SetInnerText(letter);
                        li.InnerHtml = span.ToString();
                    }
                    else
                    {
                        var a = new TagBuilder("a");
                        a.MergeAttribute("href", pageLink(letter));
                        a.InnerHtml = letter;
                        li.InnerHtml = a.ToString();
                    }
                }
                else
                {
                    li.AddCssClass("disabled");
                    var span = new TagBuilder("span");
                    span.SetInnerText(letter);
                    li.InnerHtml = span.ToString();
                }
                sb.Append(li.ToString());
            }
            ul.InnerHtml = sb.ToString();
            return new HtmlString(ul.ToString());
        }

        /// <summary>
        /// Generates a GUID-based editor template, rather than the index-based template generated by Html.EditorFor()
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="html"></param>
        /// <param name="propertyExpression">An expression which points to the property on the model you wish to generate the editor for</param>
        /// <param name="indexResolverExpression">An expression which points to the property on the model which holds the GUID index (optional, but required to make Validation* methods to work on post-back)</param>
        /// <param name="includeIndexField">
        /// True if you want this helper to render the hidden &lt;input /&gt; for you (default). False if you do not want this behaviour, and are instead going to call Html.EditorForManyIndexField() within the Editor view. 
        /// The latter behaviour is desired in situations where the Editor is being rendered inside lists or tables, where the &lt;input /&gt; would be invalid.
        /// </param>
        /// <returns>Generated HTML</returns>
        public static MvcHtmlString EditorForMany<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, IEnumerable<TValue>>> propertyExpression, Expression<Func<TValue, string>> indexResolverExpression = null, bool includeIndexField = true) where TModel : class
        {
            var items = propertyExpression.Compile()(html.ViewData.Model);
            var htmlBuilder = new StringBuilder();
            var htmlFieldName = ExpressionHelper.GetExpressionText(propertyExpression);
            var htmlFieldNameWithPrefix = html.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName);
            Func<TValue, string> indexResolver = null;

            if (indexResolverExpression == null)
            {
                indexResolver = x => null;
            }
            else
            {
                indexResolver = indexResolverExpression.Compile();
            }

            foreach (var item in items)
            {
                var dummy = new { Item = item };
                var guid = indexResolver(item);
                var memberExp = Expression.MakeMemberAccess(Expression.Constant(dummy), dummy.GetType().GetProperty("Item"));
                var singleItemExp = Expression.Lambda<Func<TModel, TValue>>(memberExp, propertyExpression.Parameters);

                if (String.IsNullOrEmpty(guid))
                {
                    guid = Guid.NewGuid().ToString();
                }
                else
                {
                    guid = html.AttributeEncode(guid);
                }

                if (includeIndexField)
                {
                    htmlBuilder.Append(_EditorForManyIndexField<TValue>(htmlFieldNameWithPrefix, guid, indexResolverExpression));
                }

                htmlBuilder.Append(html.EditorFor(singleItemExp, null, String.Format("{0}[{1}]", htmlFieldName, guid)));
            }

            return new MvcHtmlString(htmlBuilder.ToString());
        }

        /// <summary>
        /// Used to manually generate the hidden &lt;input /&gt;. To be used in conjunction with EditorForMany(), when "false" was passed for includeIndexField. 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="html"></param>
        /// <param name="indexResolverExpression">An expression which points to the property on the model which holds the GUID index (optional, but required to make Validation* methods to work on post-back)</param>
        /// <returns>Generated HTML for hidden &lt;input /&gt;</returns>
        public static MvcHtmlString EditorForManyIndexField<TModel>(this HtmlHelper<TModel> html, Expression<Func<TModel, string>> indexResolverExpression = null)
        {
            var htmlPrefix = html.ViewData.TemplateInfo.HtmlFieldPrefix;
            var first = htmlPrefix.LastIndexOf('[');
            var last = htmlPrefix.IndexOf(']', first + 1);

            if (first == -1 || last == -1)
            {
                throw new InvalidOperationException("EditorForManyIndexField called when not in a EditorForMany context");
            }

            var htmlFieldNameWithPrefix = htmlPrefix.Substring(0, first);
            var guid = htmlPrefix.Substring(first + 1, last - first - 1);

            return _EditorForManyIndexField<TModel>(htmlFieldNameWithPrefix, guid, indexResolverExpression);
        }

        private static MvcHtmlString _EditorForManyIndexField<TModel>(string htmlFieldNameWithPrefix, string guid, Expression<Func<TModel, string>> indexResolverExpression)
        {
            var htmlBuilder = new StringBuilder();
            htmlBuilder.AppendFormat(@"<input type=""hidden"" name=""{0}.Index"" value=""{1}"" />", htmlFieldNameWithPrefix, guid);

            if (indexResolverExpression != null)
            {
                htmlBuilder.AppendFormat(@"<input type=""hidden"" name=""{0}[{1}].{2}"" value=""{1}"" />", htmlFieldNameWithPrefix, guid, ExpressionHelper.GetExpressionText(indexResolverExpression));
            }

            return new MvcHtmlString(htmlBuilder.ToString());
        }

        public static IHtmlString ReCaptcha(this HtmlHelper helper)
        {
            var sb = new StringBuilder();
            var publickey = WebConfigurationManager.AppSettings["RecaptchaPublicKey"];
            sb.AppendLine("<script type=\"text/javascript\" src='https://www.google.com/recaptcha/api.js'></script>");
            sb.AppendLine("");
            sb.AppendLine("<div class=\"g-recaptcha\" data-sitekey=\""+ publickey+"\"></div>");
            return MvcHtmlString.Create(sb.ToString()); 
        }

        public static IHtmlString HelpInline(this HtmlHelper helper, string helptext)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<span class=\"help-inline\">" + helptext + "</span>");
            return MvcHtmlString.Create(sb.ToString()); 
        }

        //Standard 'Save' Button Helper    
        public static MvcHtmlString SaveButton(this HtmlHelper helper, string buttonText)
        {
            string str =
                "<button type=\"submit\" id=\"btnSave\" value=\"Save\" class=\"btn btn-success\"><span class=\"glyphicon glyphicon-ok\"></span> " +
                buttonText + "</button>";
            return new MvcHtmlString(str);
        }

        //Standard Success 'Submit' Button Helper    
        public static MvcHtmlString SubmitButton(this HtmlHelper helper, string buttonText, string glyphicon = "glyphicon glyphicon-ok")
        {
            string str =
                "<button type=\"submit\" id=\"btnSubmit\" value=\"Submit\" class=\"btn btn-success\"><span class=\"" + glyphicon + "\"></span>&nbsp;&nbsp;" + buttonText + "</button>";
            return new MvcHtmlString(str);
        }

        //Primary 'Submit' Button Helper    
        public static MvcHtmlString SubmitButtonPrimary(this HtmlHelper helper, string buttonText, string glyphicon = "glyphicon glyphicon-ok")
        {
            string str =
                "<button type=\"submit\" id=\"btnSubmit\" value=\"Submit\" class=\"btn btn-primary\"><span class=\"" + glyphicon + "\"></span>&nbsp;&nbsp;" + buttonText + "</button>";
            return new MvcHtmlString(str);
        } 

        //Standard 'Back' button for form footers
        public static MvcHtmlString BackButton(this HtmlHelper helper, string buttonText = "Cancel")
        {
            string str =
                "<input type=\"button\" value=\"" + buttonText + "\" class=\"btn-link\" onclick=\"window.history.back();\" />";
            return new MvcHtmlString(str);
        }

        public static MvcHtmlString ResolvedLink(this HtmlHelper helper, string linkUrl, string linkTitle = "", string linkDisplayText = "")
        {
            if (string.IsNullOrEmpty(linkUrl)) return null;
            if (string.IsNullOrEmpty(linkTitle)) linkTitle = linkUrl;
            if (string.IsNullOrEmpty(linkDisplayText)) linkDisplayText = linkUrl;

            if (Uri.IsWellFormedUriString(linkUrl, UriKind.Absolute))
            {
                var link = "<a href=\"" + linkUrl + "\" title=\"" + linkTitle + "\" target=\"_blank\">" + linkDisplayText + "</a>";
                return new MvcHtmlString(link);
            }

            if (Path.GetFullPath(linkUrl) == "") return null;
            var actionLink = helper.ActionLink(linkDisplayText, "SendFileToBrowser", "sllsBase", new { filePath = linkUrl, area = "" }, null);
            return actionLink;
        }

    }
}