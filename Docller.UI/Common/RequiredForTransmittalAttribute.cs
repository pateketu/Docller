using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Docller.Models;

namespace Docller.Common
{
    public class RequiredForTransmittalAttribute : ValidationAttribute, IClientValidatable
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            TransmittalViewModel viewModel = (TransmittalViewModel) validationContext.ObjectInstance;
            
            if (viewModel.Action.Equals(TransmittalButtons.Transmit))
            {
                int val;
                if (value == null || string.IsNullOrEmpty(value.ToString()) || (Int32.TryParse(value.ToString(), out val) && val == 0))
                {
                    return new ValidationResult(string.Format("{0} is required", validationContext.DisplayName));
                }
            }
            return ValidationResult.Success;

        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ModelClientValidationRule validationRule = new ModelClientValidationRule
                {
                    ErrorMessage = string.Format("{0} is required for transmittal", metadata.DisplayName),
                    ValidationType = "requiredfortransmittal"
                };
            yield return validationRule;
        }
    }
}