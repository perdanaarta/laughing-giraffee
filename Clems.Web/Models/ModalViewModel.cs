using Microsoft.AspNetCore.Html;

namespace Clems.Web.Models;

public class ModalViewModel
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Controller { get; set; }
    public string Action { get; set; }
    public IHtmlContent Body { get; set; }  // 🔑 Here
}