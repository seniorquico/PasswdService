using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace PasswdService.Utilities
{
    /// <summary>
    ///     A custom validation attribute to assert items of an enumerable string property, field, or parameter does not
    ///     exceed a maximum length.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class ListStringLengthAttribute : ValidationAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ListStringLengthAttribute"/> that accepts the maximum length
        ///     of the string items in the list.
        /// </summary>
        /// <param name="maximumLength">The maximum length, inclusive. It may not be negative.</param>
        public ListStringLengthAttribute(int maximumLength)
            : base("The length of the items in the list must be less than or equal to {1} and greater than or equal to {2}.") =>
            this.MaximumLength = maximumLength;

        /// <summary>Gets the maximum acceptable length of the string items in the list.</summary>
        public int MaximumLength { get; private set; }

        /// <summary>Gets or sets the minimum acceptable length of the string items in the list.</summary>
        public int MinimumLength { get; set; }

        /// <summary>Override of <see cref="ValidationAttribute.FormatErrorMessage"/>.</summary>
        /// <param name="name">The name to include in the formatted string.</param>
        /// <returns>A string to describe the maximum acceptable length.</returns>
        public override string FormatErrorMessage(string name) =>
            string.Format(CultureInfo.CurrentCulture, this.ErrorMessage, name, this.MaximumLength, this.MinimumLength);

        /// <summary>Override of <see cref="ValidationAttribute.IsValid(object)"/>.</summary>
        /// <remarks>
        ///     <para>This method returns <c>true</c> if the <paramref name="value"/> is null.</para>
        ///     <para>It is assumed the <see cref="RequiredAttribute"/> is used if the value may not be null.</para>
        /// </remarks>
        /// <param name="value">The value to test.</param>
        /// <returns>
        ///     <c>true</c> if the value is null or each item is less than or equal to the set maximum length.
        /// </returns>
        public override bool IsValid(object value)
        {
            // Automatically pass if value is null. RequiredAttribute should be used to assert a value is not null.
            if (value == null)
            {
                return true;
            }

            // We expect a cast exception if a non-enumerable was passed.
            var listValue = (IEnumerable)value;
            foreach (var item in listValue)
            {
                // We expect a cast exception if a non-string was passed.
                var stringItem = (string)item;
                var stringLength = stringItem == null ? 0 : ((string)stringItem).Length;
                if (stringLength < this.MinimumLength || stringLength > this.MaximumLength)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
