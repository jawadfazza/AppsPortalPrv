using AppsPortal.Library;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;

namespace AppsPortal.Extensions
{
    public class DateTimeModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (string.IsNullOrWhiteSpace(value.AttemptedValue))
            {
                return null;
            }

            DateTime dateTime;

            string CleanedDate = Numbers.ArabicToEnglish(value.AttemptedValue);

            var isDate = DateTime.TryParse(CleanedDate, Thread.CurrentThread.CurrentUICulture, DateTimeStyles.None, out dateTime);

            if (!isDate)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Invalid DateTime Value"); //Get it from Resource. Ayas
                return null;
            }
            return dateTime;
        }
    }

    public class XXXXXXXXXXXDateTimeModelBinder : DefaultModelBinder
    {
        protected override void SetProperty(ControllerContext controllerContext,
          ModelBindingContext bindingContext,
          System.ComponentModel.PropertyDescriptor propertyDescriptor, object value)
        {
            var xxvalue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (string.IsNullOrWhiteSpace(xxvalue.AttemptedValue))
            {
                //return null;
            }

            DateTime dateTime;

            string CleanedDate = Numbers.ArabicToEnglish(xxvalue.AttemptedValue);

            var isDate = DateTime.TryParse(CleanedDate, Thread.CurrentThread.CurrentUICulture, DateTimeStyles.AssumeLocal, out dateTime);

            if (!isDate)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Invalid DateTime Value"); //Get it from Resource. Ayas
                //return null;
            }
            //return dateTime;

            base.SetProperty(controllerContext, bindingContext,
                                propertyDescriptor, xxvalue);
        }
    }


}