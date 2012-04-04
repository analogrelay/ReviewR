using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using VibrantUtils;

namespace ReviewR.Web.Infrastructure
{
    // From MVC code :)
    public class CompareAttribute : ValidationAttribute
    {
        public string OtherProperty { get; private set; }
        public string OtherPropertyDisplayName { get; private set; }

        public CompareAttribute(string otherProperty)
            : base("The {0} and {1} must match")
        {
            Requires.NotNull(otherProperty, "otherProperty");
            OtherProperty = otherProperty;
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, OtherPropertyDisplayName ?? OtherProperty);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyInfo property = validationContext.ObjectType.GetProperty(OtherProperty);
            if (property == null)
            {
                return new ValidationResult(String.Format(CultureInfo.CurrentCulture, "Unknown Property {0}", OtherProperty));
            }
            object otherValue = property.GetValue(validationContext.ObjectInstance, null);
            if (!Equals(value, otherValue))
            {
                if (OtherPropertyDisplayName == null)
                {
                    // Not sure if this is a good way to get this but...
                    var provider = GlobalConfiguration.Configuration.ServiceResolver.GetModelMetadataProvider();
                    OtherPropertyDisplayName = 
                        provider.GetMetadataForProperty(
                            () => validationContext.ObjectInstance, 
                            validationContext.ObjectType, 
                            OtherProperty).GetDisplayName();
                }
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            return null;
        }
    }
}