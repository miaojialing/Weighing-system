using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System;
using static AJWPFAdmin.Core.Validation.FormChecker;

namespace AJWPFAdmin.Core.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AJFormEmailFieldAttribute : ValidationAttribute
    {

        public AJFormEmailFieldAttribute()
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

            if (required != null && string.IsNullOrWhiteSpace(value))
            {
                return new FormValidateMessage
                {
                    Success = false,
                    Message = required.ErrorMessage ?? $"邮箱必须填写"
                };
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                return new FormValidateMessage
                {
                    Success = true,
                    Message = ""
                };
            }

            return FormChecker.MatchEmailUrl(value);
        }
    }
}
