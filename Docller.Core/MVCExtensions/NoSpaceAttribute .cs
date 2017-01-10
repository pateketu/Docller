using System.ComponentModel.DataAnnotations;

namespace Docller.Core.MVCExtensions
{
    public class NoSpaceAttribute : ValidationAttribute
    {
        public NoSpaceAttribute()
        {
            
        }

        public NoSpaceAttribute(string errorMessage):base(errorMessage)
        {
            
        }
        public override bool IsValid(object value)
        {
            if(value == null) return true;
            return !value.ToString().Trim().Contains(" ");
        }
    }
}