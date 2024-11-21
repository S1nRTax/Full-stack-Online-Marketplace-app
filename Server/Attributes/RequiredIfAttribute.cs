using System;
using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        public string ConditionalProperty { get; set; }
        public string ConditionalValue { get; set; }

        public RequiredIfAttribute(string conditionalProperty, string conditionalValue)
        {
            ConditionalProperty = conditionalProperty;
            ConditionalValue = conditionalValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var conditionalProperty = validationContext.ObjectType.GetProperty(ConditionalProperty);
            if (conditionalProperty == null)
                return new ValidationResult($"Unknown property: {ConditionalProperty}");

            var conditionalValue = conditionalProperty.GetValue(validationContext.ObjectInstance)?.ToString();
            if (conditionalValue == ConditionalValue && value == null)
            {
                return new ValidationResult($"{validationContext.DisplayName} is required when {ConditionalProperty} is {ConditionalValue}");
            }

            return ValidationResult.Success;
        }
    }
}
