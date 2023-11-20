using AJWPFAdmin.Core.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using static AJWPFAdmin.Core.Validation.FormChecker;

namespace AJWPFAdmin.Core.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AJFormIDCardFieldAttribute : ValidationAttribute
    {
        
        public AJFormIDCardFieldAttribute()
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

            if (required != null )
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return new FormValidateMessage
                    {
                        Success = false,
                        Message = required.ErrorMessage ?? $"{prop.Name} 必须填写"
                    };
                }
                return FormChecker.MatchIDCardNo(value, "身份证");
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return FormChecker.MatchIDCardNo(value, "身份证");
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
