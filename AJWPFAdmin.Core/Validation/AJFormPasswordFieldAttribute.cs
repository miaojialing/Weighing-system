using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System;
using static AJWPFAdmin.Core.Validation.FormChecker;
using AJWPFAdmin.Core.Validation;

namespace AJWPFAdmin.Core.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AJFormPasswordFieldAttribute : ValidationAttribute
    {
        public AJFormPasswordFieldAttribute()
        {

        }

        public override bool RequiresValidationContext => true;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var ins = validationContext.ObjectInstance;
            var msg = Validate(ins, ins.GetType().GetProperty(validationContext.MemberName));

            if (!msg.Success)
            {
                return new ValidationResult(msg.Message);
            }

            return ValidationResult.Success;
        }

        public FormValidateMessage Validate(object formObj, PropertyInfo prop)
        {
            var value = prop.GetValue(formObj)?.ToString();

            var required = prop.GetCustomAttribute<RequiredAttribute>();

            if (required != null)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return new FormValidateMessage
                    {
                        Success = false,
                        Message = required.ErrorMessage ?? $"{prop.Name} 必须填写"
                    };
                }
                return FormChecker.MatchPassword(value);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return FormChecker.MatchPassword(value);
                }
            }

            return new FormValidateMessage
            {
                Success = true,
                Message = ""
            };
        }
    }
}
