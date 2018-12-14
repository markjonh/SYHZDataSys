using System;
using System.IO;
using System.Web.Mvc;

//HTMLConvertPDF
namespace WebApplication1.Models
{
    public class FakeView : IView
    {
        #region IView Members

        public void Render(ViewContext viewContext, TextWriter writer)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}