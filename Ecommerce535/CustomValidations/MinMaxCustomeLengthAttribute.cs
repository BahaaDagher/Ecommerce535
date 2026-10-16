using System.ComponentModel.DataAnnotations;

namespace Ecommerce535.CustomValidations
{
    public class MinMaxCustomeLengthAttribute : ValidationAttribute
    {
        private readonly int _minCustomeLength;
        private readonly int _maxCustomeLength;
        public MinMaxCustomeLengthAttribute(int mnValue   , int mxValue)
        {
            this._minCustomeLength = mnValue;
            this._maxCustomeLength = mxValue;
        }
        public override string FormatErrorMessage(string name)
        {
            return $"the Length of {name} must be between [{_minCustomeLength} , {_maxCustomeLength}]"; 
        }
        public override bool IsValid(object? value)
        {
            if(value is string name)
            {
                if(name.Length >= _minCustomeLength && name.Length <= _maxCustomeLength)
                {
                    return true; 
                }
            }
            return false; 
        }
    }
}
